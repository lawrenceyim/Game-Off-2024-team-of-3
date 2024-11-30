using System;
using Godot;

public partial class BatEnemy : CharacterBody2D, IDamageable {
	private const float WanderingSpeed = 200f;
	private const float MaxSpeed = 500f;
	private const float AccelerationTime = 5;
	private const double MinWanderTime = 3f;
	private const double MaxWanderTime = 5f;
	private const int BaseHealth = 2;
	private const double AttackCooldown = .5f;
	private const int AttackDamage = 1;
	private const float DetectionRange = 1000;
	private const float CloseEnoughRange = 40f;
	private const string WanderingState = "wander";
	private const string PursuitState = "pursue";
	private const string DeathState = "death";
	private const string MoveAnimation = "move";
	private const string DeathAnimation = "die";
	[Export] private AnimatedSprite2D _sprite;
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private AlertLabel _alertLabel;
	[Export] private HitFlash _hitFlash;
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
			_wander = new Wander(this, TimerUtil.CreateTimer(this, true), MinWanderTime, MaxWanderTime, WanderingSpeed);

			SetStateMachine();
		});

		_health = new Health(BaseHealth);
		_health.ZeroHealthEvent += (_, _) => _stateMachine.SwitchState(DeathState);

		_meleeAttack = new MeleeAttack(TimerUtil.CreateTimer(this, true), AttackCooldown, AttackDamage);

		_sprite.Play(MoveAnimation);
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
			})
			.SetExit(() => {
				_wander.StopWandering();
			})
			.SetUpdate((double delta) => {
				MoveAndSlide();
			})
			.SetPhysicsUpdate((double delta) => {
				if (Position.DistanceTo(_player.Position) <= DetectionRange) {
					_stateMachine.SwitchState(PursuitState);
					return;
				}
			})
			.Build();

		AiState pursueState = new AiState.Builder(PursuitState)
			.SetStart(() => {
				_alertLabel.DisplayExclamationMark();
				_accelerationTimer.Start(AccelerationTime);
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => { })
			.SetPhysicsUpdate((double delta) => {
				if (_accelerationTimer.TimeLeft > 0) {
					_speed = Math.Max((1 - ((float)_accelerationTimer.TimeLeft / AccelerationTime)) * MaxSpeed, WanderingSpeed);
				}

				float distanceFromTarget = Position.DistanceTo(_player.Position);
				if (distanceFromTarget > DetectionRange) {
					_alertLabel.DisplayQuestionMark();
					_stateMachine.SwitchState(WanderingState);
					return;
				}
				if (distanceFromTarget > CloseEnoughRange) {
					Velocity = (_player.Position - Position).Normalized() * WanderingSpeed;
					MoveAndSlide();
				}

				if (_touchingPlayer) {
					_meleeAttack.AttackIfReady(_player);
				}
			})
			.Build();

		AiState deathState = new AiState.Builder(DeathState)
			.SetStart(() => {
				_animationPlayer.Play(DeathAnimation);
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
