using Godot;
using System;

public partial class DarkLion : CharacterBody2D {
	private const float _detectionRange = 700;
	private const float _dashSpeed = 1500f;
	private const float _wanderingSpeed = 100f;
	private const float _dashDuration = .5f;
	private const float _dashCooldown = 5f;
	private const double _minWanderTime = 3f;
	private const double _maxWanderTime = 5f;
	private const double _attackCooldown = .5f;
	private const int _attackDamage = 1;
	private const int _baseHealth = 10;
	private const string WanderingState = "wander";
	private const string PursuitState = "pursue";
	private const string DashingState = "dash";
	private const string PreparingDashState = "preparingDash";
	private const string DeathState = "death";
	private const string MoveAnimation = "move";
	private const string DashAnimation = "dash";
	private const string ResetAnimation = "RESET";
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private Sprite2D _sprite;
	[Export] private AlertLabel _alertLabel;
	private StateMachine _stateMachine;
	private PlayerCharacter _player;
	private Timer _dashCooldownTimer;
	private Timer _dashDurationTimer;
	private MeleeAttack _meleeAttack;
	private Wander _wander;
	private Health _health;
	private bool _touchingPlayer = false;
	private float _speed = 100f;

	public override void _Ready() {
		PlayerCharacter.GetInstanceWithCallback((PlayerCharacter player) => {
			_player = player;

			_dashCooldownTimer = TimerUtil.CreateTimer(this, true);
			_dashDurationTimer = TimerUtil.CreateTimer(this, true);

			_dashDurationTimer.Timeout += () => {
				_stateMachine.SwitchState(PursuitState);
			};

			_wander = new Wander(this, TimerUtil.CreateTimer(this, true), _minWanderTime, _maxWanderTime, _wanderingSpeed);

			SetStateMachine();
		});

		_health = new Health(_baseHealth);
		_health.ZeroHealthEvent += (_, _) => _stateMachine.SwitchState(DeathState);
		_meleeAttack = new MeleeAttack(TimerUtil.CreateTimer(this, true), _attackCooldown, _attackDamage);

		_animationPlayer.Play(MoveAnimation);
	}

	public void TakeDamage(int damage) {
		_health.DecreaseHealth(damage);
	}

	private void SetStateMachine() {
		AiState wanderState = new AiState.Builder(WanderingState)
			.SetStart(() => {
				_animationPlayer.Play(MoveAnimation);
				_wander.SetWanderingVelocity();
				ChangeSpriteDirection();
			})
			.SetExit(() => {
				_wander.StopWandering();
			})
			.SetUpdate((double delta) => { })
			.SetPhysicsUpdate((double delta) => {
				MoveAndSlide();

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
				ChangeSpriteDirection();
			})
			.SetPhysicsUpdate((double delta) => {
				if (_dashCooldownTimer.TimeLeft == 0) {
					_stateMachine.SwitchState(PreparingDashState);
					return;
				}

				Velocity = (_player.Position - Position).Normalized() * _wanderingSpeed;
				MoveAndSlide();

				if (_touchingPlayer) {
					_meleeAttack.AttackIfReady(_player);
				}

				if (Position.DistanceTo(_player.Position) > _detectionRange) {
					_alertLabel.DisplayQuestionMark();
					_stateMachine.SwitchState(WanderingState);
					return;
				}
			})
			.Build();

		AiState preparingDashState = new AiState.Builder(PreparingDashState)
			.SetStart(() => {
				_animationPlayer.Play(DashAnimation);
				Velocity = (_player.Position - Position).Normalized() * _dashSpeed;
				ChangeSpriteDirection();
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => { })
			.SetPhysicsUpdate((double delta) => {
				if (_touchingPlayer) {
					_meleeAttack.AttackIfReady(_player);
				}
			})
			.Build();

		AiState dashingState = new AiState.Builder(DashingState)
			.SetStart(() => {
				_dashCooldownTimer.Start(_dashCooldown);
				_dashDurationTimer.Start(_dashDuration);
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => { })
			.SetPhysicsUpdate((double delta) => {
				MoveAndSlide();

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
			.AddState(preparingDashState)
			.AddState(dashingState)
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

	// Called by the animation player to switch from preparing to dash to dashing
	private void FinishDashPreparation() {
		_stateMachine.SwitchState(DashingState);
	}
}
