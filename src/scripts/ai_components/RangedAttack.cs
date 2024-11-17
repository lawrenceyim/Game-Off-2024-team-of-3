using Godot;

public class RangedAttack {
	private readonly Timer _cooldownTimer;
	private readonly PackedScene _projectilePrefab;
	private readonly double _cooldown;
	private readonly Node2D _host;

	public RangedAttack(Node2D host, Timer timer, PackedScene projectilePrefab, double cooldown) {
		_host = host;
		_cooldownTimer = timer;
		_projectilePrefab = projectilePrefab;
		_cooldown = cooldown;
	}

	public bool CanAttack() {
		return _cooldownTimer.IsStopped();
	}

	public void AttackIfReady(IDamageable damageable) {
		if (!_cooldownTimer.IsStopped()) {
			return;
		}
		_cooldownTimer.Start(_cooldown);
		_cooldownTimer.Paused = false;

		Node2D target = damageable as Node2D;
		Vector2 movementVector = (target.Position - _host.Position).Normalized();

		Node2D projectile = _projectilePrefab.Instantiate<Node2D>();
		_host.GetTree().Root.GetNode("Projectiles").AddChild(projectile);
		(projectile as Projectile).Initialize(_host.Position, movementVector);
	}
}
