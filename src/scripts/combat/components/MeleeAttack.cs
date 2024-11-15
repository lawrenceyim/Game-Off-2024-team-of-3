using Godot;

public class MeleeAttack {
	private readonly Timer _cooldownTimer;
	private readonly double _cooldown;
	private readonly int _damage;

	public MeleeAttack(Timer timer, double cooldown, int damage) {
		_cooldownTimer = timer;
		_cooldown = cooldown;
		_damage = damage;
	}

	public void AttackIfReady(IDamageable damageable) {
		if (_cooldownTimer.TimeLeft > 0) {
			return;
		}
		_cooldownTimer.Start(_cooldown);
		_cooldownTimer.Paused = false;
		damageable.TakeDamage(_damage);
	}
}