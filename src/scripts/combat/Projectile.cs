using Godot;

public partial class Projectile : Node2D {
	private const string ProjectileAnimation = "animation";
	[Export] private AnimatedSprite2D _sprite;
	[Export] private int _damage;
	[Export] private float _projectileSpeed;
	private Vector2 _velocity = Vector2.Zero;
	private Timer _lifeSpanTimer;

	public void Initialize(Vector2 startPosition, Vector2 velocity) {
		Position = startPosition;
		_velocity = velocity * _projectileSpeed;
		_lifeSpanTimer = TimerUtil.CreateTimer(this, true);
		_lifeSpanTimer.Timeout += QueueFree;
		_lifeSpanTimer.Start(10);
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