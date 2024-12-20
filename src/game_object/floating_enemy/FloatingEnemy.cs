using System.Collections;
using Godot;

public partial class FloatingEnemy : CharacterBody2D, IDamageable {
	private const float WanderingSpeed = 150f;
	private const double MinWanderTime = 3f;
	private const double MaxWanderTime = 5f;
	private const int BaseHealth = 4;
	private const double MeleeAttackCooldown = .5f;
	private const double RangedAttackCooldown = 1f;
	private const int AttackDamage = 1;
	private const float DetectionRange = 1200;
	private const float CloseEnoughRange = 500f;
	private const string WanderingState = "wander";
	private const string PursuitState = "pursue";
	private const string AttackState = "attack";
	private const string DeathState = "death";
	private const string MoveAnimation = "move";
	private const string AttackAnimation = "attack";
	private const string DeathAnimation = "die";
	[Export] private PackedScene _projectilePrefab;
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private Sprite2D _sprite;
	[Export] private AlertLabel _alertLabel;
	[Export] private HitFlash _hitFlash;
	private StateMachine _stateMachine;
	private PlayerCharacter _player;
	private MeleeAttack _meleeAttack;
	private RangedAttack _rangedAttack;
	private Wander _wander;
	private Health _health;
	private float _speed;
	private bool _touchingPlayer = false;

	public override void _Ready() {
		PlayerCharacter.GetInstanceWithCallback((PlayerCharacter player) => {
			_player = player;

			_wander = new Wander(this, TimerUtil.CreateTimer(this, true), MinWanderTime, MaxWanderTime, WanderingSpeed);

			SetStateMachine();
		});

		_health = new Health(BaseHealth);
		_health.ZeroHealthEvent += (_, _) => _stateMachine.SwitchState(DeathState);

		_meleeAttack = new MeleeAttack(TimerUtil.CreateTimer(this, true), MeleeAttackCooldown, AttackDamage);
		_rangedAttack = new RangedAttack(this, this, TimerUtil.CreateTimer(this, true), _projectilePrefab, RangedAttackCooldown);

		_animationPlayer.Play(MoveAnimation);
	}

	public void TakeDamage(int damage) {
		_health.DecreaseHealth(damage);
		if (!_health.IsHealthZero()) {
			_hitFlash.DisplayHitFlash();
		}
	}

	public void DeathAnimationFinished() {
		QueueFree();
	}

	private void SetStateMachine() {
		AiState wanderState = new AiState.Builder(WanderingState)
			.SetStart(() => {
				_wander.SetWanderingVelocity();
				_alertLabel.DisplayQuestionMark();
			})
			.SetExit(() => {
				_wander.StopWandering();
			})
			.SetUpdate((double delta) => {
				ChangeSpriteDirection();
			})
			.SetPhysicsUpdate((double delta) => {
				MoveAndSlide();

				if (Position.DistanceTo(_player.Position) <= DetectionRange) {
					_alertLabel.DisplayExclamationMark();
					_stateMachine.SwitchState(PursuitState);
					return;
				}
			})
			.Build();

		AiState pursueState = new AiState.Builder(PursuitState)
			.SetStart(() => {
				_animationPlayer.Play(MoveAnimation);
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => {
				ChangeSpriteDirection();
			})
			.SetPhysicsUpdate((double delta) => {
				float distanceFromTarget = Position.DistanceTo(_player.Position);
				if (distanceFromTarget > DetectionRange) {
					_stateMachine.SwitchState(WanderingState);
					return;
				}
				if (distanceFromTarget > CloseEnoughRange) {
					Velocity = (_player.Position - Position).Normalized() * WanderingSpeed;
					MoveAndSlide();
				}

				if (_rangedAttack.CanAttack()) {
					ChangeSpriteDirection();
					_stateMachine.SwitchState(AttackState);
					return;
				}

				if (_touchingPlayer) {
					_meleeAttack.AttackIfReady(_player);
				}
			})
			.Build();

		AiState attackState = new AiState.Builder(AttackState)
			.SetStart(() => {
				_animationPlayer.Play(AttackAnimation);
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => { })
			.SetPhysicsUpdate((double delta) => { })
			.Build();

		AiState deathState = new AiState.Builder(DeathState)
			.SetStart(() => {
				_animationPlayer.Play(DeathAnimation);
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => { })
			.SetPhysicsUpdate((double delta) => { })
			.Build();

		_stateMachine = new StateMachine.Builder(WanderingState)
			.AddState(wanderState)
			.AddState(pursueState)
			.AddState(attackState)
			.AddState(deathState)
			.Build();

		AddChild(_stateMachine);
	}

	private void _on_hitbox_area_2d_area_entered(Area2D body) {
		if (body.GetParent() is PlayerCharacter) {
			_touchingPlayer = true;
		}
	}

	private void _on_hitbox_area_2d_area_exited(Area2D body) {
		if (body.GetParent() is PlayerCharacter) {
			_touchingPlayer = false;
		}
	}

	private void ChangeSpriteDirection() {
		if (_stateMachine.GetCurrentStateName().Equals(PursuitState)) {
			if (_player.GlobalPosition.X > GlobalPosition.X) {
				_sprite.FlipH = false;
			} else {
				_sprite.FlipH = true;
			}
			return;
		}
		if (Velocity.X > 0) {
			_sprite.FlipH = false;
		} else {
			_sprite.FlipH = true;
		}
	}

	// Called by the animation player to switch from attacking to pursuit
	private void FinishAttackAnimation() {
		_rangedAttack.AttackIfReady(_player.Position);
		_stateMachine.SwitchState(PursuitState);
	}
}
