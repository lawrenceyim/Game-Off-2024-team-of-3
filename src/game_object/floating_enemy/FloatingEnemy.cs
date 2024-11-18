using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class FloatingEnemy : CharacterBody2D, IDamageable {
	private const float _wanderingSpeed = 100f;
	private const double _minWanderTime = 3f;
	private const double _maxWanderTime = 5f;
	private const int _baseHealth = 5;
	private const double _meleeAttackCooldown = .5f;
	private const double _rangedAttackCooldown = 5f;
	private const int _attackDamage = 1;
	private const string WanderingState = "wander";
	private const string PursuitState = "pursue";
	private const string AttackState = "attack";
	private const string DeathState = "death";
	private const string MoveAnimation = "move";
	private const string AttackAnimation = "attack";
	private const string ResetAnimation = "RESET";
	[Export] private PackedScene _projectilePrefab;
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private Sprite2D _sprite;
	[Export] private AlertLabel _alertLabel;
	private StateMachine _stateMachine;
	private PlayerCharacter _player;
	private Vector2 _moveVector;
	private MeleeAttack _meleeAttack;
	private RangedAttack _rangedAttack;
	private Wander _wander;
	private Health _health;
	private float _detectionRange = 1000;
	private float _speed;
	private bool _touchingPlayer = false;

	public override void _Ready() {
		PlayerCharacter.GetInstanceWithCallback((PlayerCharacter player) => {
			_player = player;

			_wander = new Wander(this, TimerUtil.CreateTimer(this, true), _minWanderTime, _maxWanderTime, _wanderingSpeed);

			SetStateMachine();
		});

		_health = new Health(_baseHealth);
		_health.ZeroHealthEvent += (_, _) => _stateMachine.SwitchState(DeathState);

		_meleeAttack = new MeleeAttack(TimerUtil.CreateTimer(this, true), _meleeAttackCooldown, _attackDamage);
		_rangedAttack = new RangedAttack(this, TimerUtil.CreateTimer(this, true), _projectilePrefab, _rangedAttackCooldown);

		_animationPlayer.Play(MoveAnimation);
	}

	public void TakeDamage(int damage) {
		_health.DecreaseHealth(damage);
	}

	private void SetStateMachine() {
		AiState wanderState = new AiState.Builder(WanderingState)
			.SetStart(() => {
				_wander.SetWanderingVelocity();
				ChangeSpriteDirection();
				_alertLabel.DisplayQuestionMark();
			})
			.SetExit(() => {
				_wander.StopWandering();
			})
			.SetUpdate((double delta) => {
				MoveAndSlide();
			})
			.SetPhysicsUpdate((double delta) => {
				if (Position.DistanceTo(_player.Position) <= _detectionRange) {
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
				_moveVector = (_player.Position - Position).Normalized();
				Velocity = _moveVector * _wanderingSpeed;
				MoveAndSlide();
				ChangeSpriteDirection();
			})
			.SetPhysicsUpdate((double delta) => {
				if (_rangedAttack.CanAttack()) {
					_stateMachine.SwitchState(AttackState);
					return;
				}
				if (_touchingPlayer) {
					_meleeAttack.AttackIfReady(_player);
				}
				if (Position.DistanceTo(_player.Position) > _detectionRange) {
					_stateMachine.SwitchState(WanderingState);
					return;
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
				// Play death SFX
				// Play death animation
			})
			.SetExit(() => {
				QueueFree();
			})
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
		if (Velocity.X > 0) {
			_sprite.FlipH = false;
		} else {
			_sprite.FlipH = true;
		}
	}

	// Called by the animation player to switch from attacking to pursuit
	private void FinishAttackAnimation() {
		_rangedAttack.AttackIfReady(_player);
		_stateMachine.SwitchState(PursuitState);
	}
}
