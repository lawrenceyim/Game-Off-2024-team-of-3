using Godot;

public class RangedAttack {
	private readonly Timer _cooldownTimer;
	[Export] private readonly PackedScene _projectilePrefab;
	[Export] private readonly double _cooldown;
	private Node2D _host;

	public RangedAttack(Node2D host, Timer timer) {
		_host = host;
		_cooldownTimer = timer;
	}

	public void AttackIfReady(IDamageable damageable) {
		if (!_cooldownTimer.IsStopped()) {
			return;
		}
		_cooldownTimer.Start(_cooldown);
		_cooldownTimer.Paused = false;

		Node2D target = damageable as Node2D;
		Vector2 movementVector = (target.Position - _host.Position).Normalized();

		Projectile projectile = (Projectile)_projectilePrefab.Instantiate();
		projectile.Initialize(movementVector);
	}
}