using Godot;
using System;

public partial class DarkLion : CharacterBody2D, IDamageable {
	private const float DetectionRange = 1200;
	private const float DashSpeed = 1500f;
	private const float WanderingSpeed = 250f;
	private const float DashDuration = .5f;
	private const float DashCooldown = 2f;
	private const double MinWanderTime = 3f;
	private const double MaxWanderTime = 5f;
	private const double ttackCooldown = .5f;
	private const int AttackDamage = 2;
	private const int BaseHealth = 10;
	private const float CloseEnoughRange = 30f;
	private const float MinDashRange = 400f;
	private const string WanderingState = "wander";
	private const string PursuitState = "pursue";
	private const string DashingState = "dash";
	private const string PreparingDashState = "preparingDash";
	private const string DeathState = "death";
	private const string MoveAnimation = "move";
	private const string DashAnimation = "dash";
	private const string IdleAnimation = "idle";
	private const string DeathAnimation = "die";
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private Sprite2D _sprite;
	[Export] private AlertLabel _alertLabel;
	[Export] private HitFlash _hitFlash;
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

			_wander = new Wander(this, TimerUtil.CreateTimer(this, true), MinWanderTime, MaxWanderTime, WanderingSpeed);

			SetStateMachine();
		});

		_health = new Health(BaseHealth);
		_health.ZeroHealthEvent += (_, _) => _stateMachine.SwitchState(DeathState);
		_meleeAttack = new MeleeAttack(TimerUtil.CreateTimer(this, true), ttackCooldown, AttackDamage);

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

	private void SetStateMachine() {
		AiState wanderState = new AiState.Builder(WanderingState)
			.SetStart(() => {
				_animationPlayer.Play(MoveAnimation);
				_wander.SetWanderingVelocity();
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
			.SetStart(() => { })
			.SetExit(() => { })
			.SetUpdate((double delta) => {
				ChangeSpriteDirection();
			})
			.SetPhysicsUpdate((double delta) => {
				float distanceFromTarget = Position.DistanceTo(_player.Position);
				if (distanceFromTarget > DetectionRange) {
					_alertLabel.DisplayQuestionMark();
					_stateMachine.SwitchState(WanderingState);
					return;
				}
				if (distanceFromTarget > CloseEnoughRange) {
					if (_dashCooldownTimer.TimeLeft == 0 &&
							distanceFromTarget >= MinDashRange) {
						_stateMachine.SwitchState(PreparingDashState);
						return;
					}

					_animationPlayer.Play(MoveAnimation);
					Velocity = (_player.Position - Position).Normalized() * WanderingSpeed;
					MoveAndSlide();
				} else {
					_animationPlayer.Play(IdleAnimation);
				}

				if (_touchingPlayer) {
					_meleeAttack.AttackIfReady(_player);
				}
			})
			.Build();

		AiState preparingDashState = new AiState.Builder(PreparingDashState)
			.SetStart(() => {
				_animationPlayer.Play(DashAnimation);
				Velocity = (_player.Position - Position).Normalized() * DashSpeed;
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
				_dashCooldownTimer.Start(DashCooldown);
				_dashDurationTimer.Start(DashDuration);
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
				_animationPlayer.Play(DeathAnimation);
			})
			.SetExit(() => { })
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
}
