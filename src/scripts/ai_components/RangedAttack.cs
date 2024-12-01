using System;
using Godot;

public class RangedAttack {
	private readonly Timer _cooldownTimer;
	private readonly PackedScene _projectilePrefab;
	private readonly double _cooldown;
	private readonly Node2D _host;
	private readonly Node2D _spawnPosition;
	private Node _projectileRoot;

	public RangedAttack(Node2D host, Node2D spawnPosition, Timer timer, PackedScene projectilePrefab, double cooldown) {
		_host = host;
		_spawnPosition = spawnPosition;
		_cooldownTimer = timer;
		_projectilePrefab = projectilePrefab;
		_cooldown = cooldown;
		_projectileRoot = _host.GetTree().Root.GetNode("Projectiles");
	}

	public bool CanAttack() {
		return _cooldownTimer.IsStopped();
	}

	public void AttackIfReady(Vector2 targetPosition) {
		if (!_cooldownTimer.IsStopped()) {
			return;
		}
		_cooldownTimer.Start(_cooldown);
		_cooldownTimer.Paused = false;

		Vector2 movementVector = (targetPosition - _host.Position).Normalized();

		Node2D projectile = _projectilePrefab.Instantiate<Node2D>();
		_projectileRoot.AddChild(projectile);
		(projectile as Projectile).Initialize(_host, _spawnPosition.GlobalPosition, movementVector);
	}

	public void AttackIfReady(float rotation) {
		if (!_cooldownTimer.IsStopped()) {
			return;
		}
		_cooldownTimer.Start(_cooldown);
		_cooldownTimer.Paused = false;

		Vector2 movementVector = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation)).Normalized();
		Node2D projectile = _projectilePrefab.Instantiate<Node2D>();
		_projectileRoot.AddChild(projectile);
		(projectile as Projectile).Initialize(_host, _spawnPosition.GlobalPosition, movementVector);
		(projectile as Projectile).SetProjectileRotation(rotation);
	}
}
