using Godot;

public partial class Projectile : Node2D {
	private const string ProjectileAnimation = "animation";
	[Export] private AnimatedSprite2D _sprite;
	[Export] private int _damage;
	[Export] private float _projectileSpeed;
	private Vector2 _velocity = Vector2.Zero;

	public override void _Ready() {
		GetNode("/root/Projectiles").AddChild(this);
		// _sprite.Play(ProjectileAnimation);
	}

	public void Initialize(Vector2 velocity) {
		_velocity = velocity * _projectileSpeed;
	}

	public override void _Process(double delta) {
		Position += _velocity * (float)delta;
	}

	public void _on_area_2d_area_entered(Area2D area) {
		if (area.GetParent() is PlayerCharacter player) {
			player.TakeDamage(_damage);
			QueueFree();
		}
	}
}