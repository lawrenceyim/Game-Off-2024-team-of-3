using Godot;
using System;

public partial class PlayerAnimation : Node {
	private const string IdleAnimation = "idle";
	private const string MoveAnimation = "move";
	private const string DashAnimation = "dash";
	[Export] private AnimatedSprite2D _sprite;
	[Export] private PlayerGun _gun;

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
		_gun.FlipGunSpriteVertically(_sprite.FlipH);
	}

	public void StartDashAnimation() {
		_sprite.Play(DashAnimation);
	}
}
