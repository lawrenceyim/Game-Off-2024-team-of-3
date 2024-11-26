using Godot;

public partial class PlayerGun : Sprite2D {
	private const float AttackCooldown = .5f;
	[Export] private CharacterBody2D _player;
	[Export] private PackedScene _projectilePrefab;
	private RangedAttack _rangedAttack;

	public override void _Ready() {
		_rangedAttack = new RangedAttack(_player, TimerUtil.CreateTimer(this, true), _projectilePrefab, AttackCooldown);
	}

	public void AimGun(Vector2 aimedPosition) {
		Rotation = aimedPosition.Angle();
	}

	public void FireGun() {
		if (_rangedAttack.CanAttack()) {
			_rangedAttack.AttackIfReady(Rotation);
		}
	}
}
