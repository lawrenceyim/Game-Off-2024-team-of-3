using Godot;
using System;

public partial class PlayerInputHandler : Node, IInputListener {
	[Export] private PlayerMovement _playerMovement;
	[Export] private PlayerGun _gun;
	[Export] private CharacterBody2D _player;

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
		// Mouse in viewport coordinates.
		if (@event is InputEventMouseButton eventMouseButton) {
			if (eventMouseButton.ButtonIndex == MouseButton.Left &&
				eventMouseButton.IsPressed()
			) {
				// Send mouse click to fire player weapon
				GD.Print("Mouse Left Button pressed at " + eventMouseButton.Position);
			}
		} else if (@event is InputEventMouseMotion eventMouseMotion) {
			Vector2 origin = GetViewport().GetVisibleRect().Size / 2;
			_gun.AimGun((eventMouseMotion.Position - origin).Normalized());
		}
	}
}
