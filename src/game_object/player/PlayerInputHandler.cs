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
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseButton eventMouseButton) {
			if (eventMouseButton.ButtonIndex == MouseButton.Left &&
				eventMouseButton.IsPressed()
			) {
				_gun.FireGun();
			} else if (eventMouseButton.ButtonIndex == MouseButton.Right &&
				eventMouseButton.IsPressed()
			) {
				_playerMovement.Dash();
			}
		} else if (@event is InputEventMouseMotion eventMouseMotion) {
			Vector2 origin = GetViewport().GetVisibleRect().Size / 2;
			_camera.SetAimedPosition(eventMouseMotion.Position);
			Vector2 direction = (eventMouseMotion.Position - origin).Normalized();
			_gun.AimGun(direction);
			_playerMovement.SetDashDirection(direction);
		}
	}
}
