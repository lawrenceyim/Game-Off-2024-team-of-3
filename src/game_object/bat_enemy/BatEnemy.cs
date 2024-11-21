using System;
using Godot;

public partial class BatEnemy : CharacterBody2D, IDamageable {
	private const float _wanderingSpeed = 100f;
	private const float _maxSpeed = 500f;
	private const float _accelerationTime = 5;
	private const double _minWanderTime = 3f;
	private const double _maxWanderTime = 5f;
	private const int _baseHealth = 5;
	private const double _attackCooldown = .5f;
	private const int _attackDamage = 1;
	private const float _detectionRange = 500;
	private const float _closeEnoughRange = 10f;
	private const string WanderingState = "wander";
	private const string PursuitState = "pursue";
	private const string DeathState = "death";
	private const string MoveAnimation = "move";
	[Export] private AnimatedSprite2D _sprite;
	[Export] private AlertLabel _alertLabel;
	private StateMachine _stateMachine;
	private PlayerCharacter _player;
	private Timer _accelerationTimer;
	private MeleeAttack _meleeAttack;
	private Wander _wander;
	private Health _health;
	private float _speed;
	private bool _touchingPlayer = false;

	public override void _Ready() {
		PlayerCharacter.GetInstanceWithCallback((PlayerCharacter player) => {
			_player = player;

			_accelerationTimer = TimerUtil.CreateTimer(this, true);
			_wander = new Wander(this, TimerUtil.CreateTimer(this, true), _minWanderTime, _maxWanderTime, _wanderingSpeed);

			SetStateMachine();
		});

		_health = new Health(_baseHealth);
		_health.ZeroHealthEvent += (_, _) => _stateMachine.SwitchState(DeathState);

		_meleeAttack = new MeleeAttack(TimerUtil.CreateTimer(this, true), _attackCooldown, _attackDamage);

		_sprite.Play(MoveAnimation);
	}

	public void TakeDamage(int damage) {
		_health.DecreaseHealth(damage);
	}

	private void SetStateMachine() {
		AiState wanderState = new AiState.Builder(WanderingState)
			.SetStart(() => {
				_wander.SetWanderingVelocity();
			})
			.SetExit(() => {
				_wander.StopWandering();
			})
			.SetUpdate((double delta) => {
				MoveAndSlide();
			})
			.SetPhysicsUpdate((double delta) => {
				if (Position.DistanceTo(_player.Position) <= _detectionRange) {
					_stateMachine.SwitchState(PursuitState);
					return;
				}
			})
			.Build();

		AiState pursueState = new AiState.Builder(PursuitState)
			.SetStart(() => {
				_alertLabel.DisplayExclamationMark();
				_accelerationTimer.Start(_accelerationTime);
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => { })
			.SetPhysicsUpdate((double delta) => {
				if (_accelerationTimer.TimeLeft > 0) {
					_speed = Math.Max((1 - ((float)_accelerationTimer.TimeLeft / _accelerationTime)) * _maxSpeed, _wanderingSpeed);
				}

				float distanceFromTarget = Position.DistanceTo(_player.Position);
				if (distanceFromTarget > _detectionRange) {
					_alertLabel.DisplayQuestionMark();
					_stateMachine.SwitchState(WanderingState);
					return;
				}
				if (distanceFromTarget > _closeEnoughRange) {
					Velocity = (_player.Position - Position).Normalized() * _wanderingSpeed;
					MoveAndSlide();
				}

				if (_touchingPlayer) {
					_meleeAttack.AttackIfReady(_player);
				}
			})
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
}
