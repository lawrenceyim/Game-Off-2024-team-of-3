using Godot;
using System;

public partial class PlayerAnimation : Node {
	private const string IdleAnimation = "idle";
	private const string MoveAnimation = "move";
	[Export] private AnimatedSprite2D _sprite;

	public override void _Ready() {
	}

	public void UpdateMovement(Vector2 movement) {
		if (movement.IsZeroApprox()) {
			_sprite.Play(IdleAnimation);
			return;
		}

		_sprite.Play(MoveAnimation);
		if (movement.X > 0) {
			_sprite.FlipH = false;
		} else if (movement.X < 0) {
			_sprite.FlipH = true;
		}

	}
}
