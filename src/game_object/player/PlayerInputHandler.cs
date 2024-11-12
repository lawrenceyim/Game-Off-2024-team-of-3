using Godot;
using System;

public partial class PlayerInputHandler : Node, IInputListener {
    [Export] private PlayerMovement _playerMovement;

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
}
