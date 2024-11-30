using Godot;
using System;

public partial class PlayerInputHandler : Node, IInputListener {
	[Export] private PlayerMovement _playerMovement;
	[Export] private PlayerGun _gun;
	[Export] private CharacterBody2D _player;
	[Export] private PlayerCamera _camera;

	public override void _Ready() {
		InputManager.GetInstance().AddListener(this);
	}

	public void ProcessInput() {
		Vector2 inputVector = Vector2.Zero;
		if (Input.IsActionPressed("MoveUp")) {
			inputVector.Y -= 1;
		}
		if (Input.IsActionPressed("MoveDown")) {
			inputVector.Y += 1;
		}
		if (Input.IsActionPressed("MoveLeft")) {
			inputVector.X -= 1;
		}
		if (Input.IsActionPressed("MoveRight")) {
			inputVector.X += 1;
		}
		_playerMovement.Move(inputVector.Normalized());

		if (Input.IsActionJustPressed("Action")) {
			// Player Action
		}
		if (Input.IsActionJustPressed("Escape")) {
			// Escape menu
		}

        _gun.AimGun(_player.GetGlobalMousePosition());

        if (Input.IsMouseButtonPressed(MouseButton.Left)) {
            _gun.FireGun();
        }
        
        if (Input.IsMouseButtonPressed(MouseButton.Right)) {
            _playerMovement.Dash();
        }
	}
}
