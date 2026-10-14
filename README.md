# IT-Kamianets 3D Engine: Character View Controls

`3d-constructor-character-view-controls` connects input to the character for the character-view constructor of the **IT-Kamianets 3D Engine**.

### Logic

`3d-unity-character` moves a character but does not decide where "forward" is. This package does: forward is where the camera looks, and looking turns the character.

It reads actions from `3d-unity-input`, drives `3d-unity-character`, and stays in sync with `3d-constructor-character-view-camera` when the player switches between first and third person.

### Status

Version 0.1: `CharacterViewControls`, placed next to the local player's `CharacterMotor`.

* Camera-relative movement: forward is `CharacterViewCamera.PlanarForward`
* First person: the character always faces the camera and strafes
* Third person: the character turns toward where it walks (`TurnSpeed`); while aiming (hold `Secondary`) or when gameplay sets `FaceCamera` (e.g. casting) it faces the camera and strafes
* Sprint: hold, or toggle (default on gamepad); forward-only while facing the camera; stops when the player stops
* Crouch: hold, or toggle
* With gameplay input off (a menu is open) the character stands still
* `LastCommand`: the command sent this frame, for the network to send to the server

Runs at execution order -40: after the camera has read look input (-50), before the motor steps (0).

Checked in Play mode with a virtual gamepad in the `temnorid-arena` Playground scene: camera-relative walking, body following the camera in first person, sprint toggle by stick click, view toggle, body turning to the walk direction in third person.

### Roadmap

* Camera-relative movement
* Look to turn
* Strafing
* Sprint and crouch toggles
* First- and third-person control differences
* Aim mode
* Control settings
* Mobile on-screen sticks
* Stable v1 controls API
