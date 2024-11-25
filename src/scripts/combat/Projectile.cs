using Godot;

public partial class Projectile : Node2D {
	private const string ProjectileAnimation = "animation";
	[Export] private int _damage;
	[Export] private float _projectileSpeed;
	private Vector2 _velocity = Vector2.Zero;
	private Timer _lifeSpanTimer;
	private Node2D _shooter;

	public void Initialize(Node2D shooter, Vector2 startPosition, Vector2 velocity) {
		_shooter = shooter;
		Position = startPosition;
		_velocity = velocity * _projectileSpeed;
		_lifeSpanTimer = TimerUtil.CreateTimer(this, true);
		_lifeSpanTimer.Timeout += QueueFree;
		_lifeSpanTimer.Start(10);
	}

	public void SetProjectileRotation(float angle) {
		Rotation = angle;
	}

	public override void _Process(double delta) {
		Position += _velocity * (float)delta;
	}

	public void _on_area_2d_area_entered(Area2D area) {
		if (area.GetParent() is IDamageable damageable) {
			if (damageable == _shooter) {
				return;
			}
			damageable.TakeDamage(_damage);
			QueueFree();
		}
	}
}