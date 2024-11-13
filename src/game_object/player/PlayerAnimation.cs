using Godot;
using System;

public partial class PlayerAnimation : Node {
    [Export] private AnimatedSprite2D _sprite;

    private const string Idle = "idle";
    private const string Move = "move";

    public override void _Ready() {
    }

    public void UpdateMovement(Vector2 movement) {
        if (movement.IsZeroApprox()) {
            _sprite.Play(Idle);
            return;
        }

        _sprite.Play(Move);
        if (movement.X > 0) {
            _sprite.FlipH = false;
        } else if (movement.X < 0) {
            _sprite.FlipH = true;
        }

    }
}
