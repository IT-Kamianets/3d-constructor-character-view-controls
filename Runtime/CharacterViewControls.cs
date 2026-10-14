using ITKamianets.Engine.Character;
using ITKamianets.Engine.Input;
using UnityEngine;

namespace ITKamianets.Engine.CharacterView
{
    /// <summary>
    /// Turns player input into a <see cref="CharacterCommand"/> for the Character View
    /// constructor. Forward is where the camera looks. In first person the character always
    /// faces the camera and strafes; in third person it turns toward where it walks, unless the
    /// player is aiming or <see cref="FaceCamera"/> is set (e.g. while casting a spell).
    ///
    /// Put it next to the <see cref="CharacterMotor"/> of the local player only. Other players
    /// are driven by the network with the same commands. Runs after the camera has read look
    /// input and before the motor steps.
    /// </summary>
    [RequireComponent(typeof(CharacterMotor))]
    [DefaultExecutionOrder(-40)]
    public class CharacterViewControls : MonoBehaviour
    {
        [Tooltip("Leave empty to use the CharacterViewCamera on the main camera.")]
        [SerializeField] private CharacterViewCamera view;

        [SerializeField] private CharacterViewControlsSettings settings = new CharacterViewControlsSettings();

        public CharacterViewControlsSettings Settings => settings;

        public CharacterViewCamera View
        {
            get => view;
            set => view = value;
        }

        /// <summary>Holding Secondary in third person: the character faces the camera and strafes.</summary>
        public bool IsAiming { get; private set; }

        /// <summary>Set by gameplay (e.g. while casting) to make the character face the camera in third person.</summary>
        public bool FaceCamera { get; set; }

        /// <summary>The command sent to the motor this frame; the network sends the same one to the server.</summary>
        public CharacterCommand LastCommand { get; private set; }

        private CharacterMotor _motor;
        private bool _sprintToggled;
        private bool _crouchToggled;

        private void Awake()
        {
            _motor = GetComponent<CharacterMotor>();
        }

        private void Start()
        {
            if (view == null && UnityEngine.Camera.main != null)
            {
                view = UnityEngine.Camera.main.GetComponent<CharacterViewCamera>();
            }

            if (view != null && view.Target == null)
            {
                view.Target = _motor;
            }
        }

        private void OnDisable()
        {
            _sprintToggled = false;
            _crouchToggled = false;
            IsAiming = false;
        }

        private void Update()
        {
            var input = EngineInput.Default;

            if (view == null || !input.PlayerEnabled)
            {
                // No control (menu open, no camera): stand still, keep facing.
                _sprintToggled = false;
                LastCommand = new CharacterCommand { Yaw = _motor.Yaw, Crouch = _crouchToggled };
                _motor.SetCommand(LastCommand);
                return;
            }

            var actions = input.Player;
            var stick = Vector2.ClampMagnitude(actions.Move.ReadValue<Vector2>(), 1f);
            var move = view.PlanarForward * stick.y + view.PlanarRight * stick.x;

            IsAiming = settings.AimWithSecondary && actions.Secondary.IsPressed();

            var command = new CharacterCommand
            {
                Move = move,
                Yaw = FacingYaw(move),
                Sprint = Sprint(actions, stick, input.Scheme),
                Jump = actions.Jump.WasPressedThisFrame(),
                Crouch = Crouch(actions),
            };

            LastCommand = command;
            _motor.SetCommand(command);
        }

        private float FacingYaw(Vector3 move)
        {
            var faceCamera = view.View == CameraView.FirstPerson || IsAiming || FaceCamera;
            if (faceCamera)
            {
                return view.Yaw;
            }

            if (move.sqrMagnitude < 0.0001f)
            {
                return _motor.Yaw;
            }

            var target = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            return Mathf.MoveTowardsAngle(_motor.Yaw, target, settings.TurnSpeed * Time.deltaTime);
        }

        private bool Sprint(EngineInput.PlayerActions actions, Vector2 stick, InputScheme scheme)
        {
            // Forward relative to where the character will move: in third person while walking
            // freely the character faces its movement, so any direction counts as forward.
            var facesCamera = view.View == CameraView.FirstPerson || IsAiming || FaceCamera;
            var forward = !settings.SprintForwardOnly || !facesCamera || stick.y > 0.5f;
            var moving = stick.sqrMagnitude > 0.01f;

            var toggle = scheme == InputScheme.Gamepad ? settings.SprintToggleOnGamepad : settings.SprintToggle;
            if (!toggle)
            {
                _sprintToggled = false;
                return actions.Sprint.IsPressed() && moving && forward;
            }

            if (actions.Sprint.WasPressedThisFrame())
            {
                _sprintToggled = !_sprintToggled;
            }

            if (!moving)
            {
                _sprintToggled = false;
            }

            return _sprintToggled && forward;
        }

        private bool Crouch(EngineInput.PlayerActions actions)
        {
            if (!settings.CrouchToggle)
            {
                _crouchToggled = false;
                return actions.Crouch.IsPressed();
            }

            if (actions.Crouch.WasPressedThisFrame())
            {
                _crouchToggled = !_crouchToggled;
            }

            return _crouchToggled;
        }
    }
}
