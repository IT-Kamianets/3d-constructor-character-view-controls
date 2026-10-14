using System;
using UnityEngine;

namespace ITKamianets.Engine.CharacterView
{
    /// <summary>How player input drives the character in the Character View constructor.</summary>
    [Serializable]
    public class CharacterViewControlsSettings
    {
        [Header("Third person")]
        [Tooltip("How fast the character turns toward where it walks in third person, in degrees per second.")]
        public float TurnSpeed = 720f;

        [Tooltip("Hold Secondary to aim: in third person the character faces where the camera looks and strafes.")]
        public bool AimWithSecondary = true;

        [Header("Sprint")]
        [Tooltip("Sprint stays on after one press, until the player stops moving. Hold to sprint when off.")]
        public bool SprintToggle;

        [Tooltip("Toggle sprint on a gamepad, where holding a stick pressed in is tiring.")]
        public bool SprintToggleOnGamepad = true;

        [Tooltip("Sprint only while moving forward, not sideways or backwards.")]
        public bool SprintForwardOnly = true;

        [Header("Crouch")]
        [Tooltip("Crouch stays on after one press, until pressed again. Hold to crouch when off.")]
        public bool CrouchToggle;
    }
}
