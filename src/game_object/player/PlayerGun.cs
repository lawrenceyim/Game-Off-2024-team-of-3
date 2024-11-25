using Godot;

public partial class PlayerGun : Sprite2D {
	private const float _attackCooldown = .5f;
	[Export] private CharacterBody2D _player;
	[Export] private PackedScene _projectilePrefab;
	private RangedAttack _rangedAttack;
	private Vector2 _aimedPosition;

	public override void _Ready() {
		_rangedAttack = new RangedAttack(_player, TimerUtil.CreateTimer(this, true), _projectilePrefab, _attackCooldown);
	}

	public void AimGun(Vector2 aimedPosition) {
		Rotation = aimedPosition.Angle();
		_aimedPosition = aimedPosition;
	}

	public void FireGun() {
		if (_rangedAttack.CanAttack()) {
			_rangedAttack.AttackIfReady(Rotation);
		}
	}
}
