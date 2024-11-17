using Godot;
using System;

public partial class DarkLion : CharacterBody2D {
	private const float _detectionRange = 1000;
	private const float _dashSpeed = 1000f;
	private const float _wanderingSpeed = 100f;
	private const float _dashDuration = 1f;
	private const float _dashCooldown = 5f;
	private const double _minWanderTime = 3f;
	private const double _maxWanderTime = 5f;
	private const double _attackCooldown = .5f;
	private const int _attackDamage = 1;
	private const int _baseHealth = 10;
	private const string WanderingState = "wander";
	private const string PursuitState = "pursue";
	private const string DashingState = "dash";
	private const String PreparingDashState = "preparingDash";
	private const string MoveAnimation = "move";
	private const string DashAnimation = "dash";
	private const string ResetAnimation = "reset";
	[Export] AnimationPlayer _animationPlayer;
	[Export] Sprite2D _sprite;
	private StateMachine _stateMachine;
	private PlayerCharacter _player;
	private Vector2 _moveVector;
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
		_health.ZeroHealthEvent += InitiateDeath;
		_meleeAttack = new MeleeAttack(TimerUtil.CreateTimer(this, true), _attackCooldown, _attackDamage);

		_animationPlayer.Play(MoveAnimation);
	}

	public void TakeDamage(int damage) {
		_health.DecreaseHealth(damage);
	}

	private void InitiateDeath(object sender, EventArgs e) {
		// Start death animation
		// Start death sfx  
		// Handle removing object in the callback from death animation to ensure that the death animation finishes
		FinishDeath(); // Remove this later and make it called by death animation function
	}

	private void FinishDeath() {
		QueueFree();
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
				_animationPlayer.Play(MoveAnimation);
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => {
				if (_dashCooldownTimer.TimeLeft == 0) {
					_stateMachine.SwitchState(PreparingDashState);
					return;
				}
				_moveVector = (_player.Position - Position).Normalized();
				Velocity = _moveVector * _wanderingSpeed;
				MoveAndSlide();
				ChangeSpriteDirection();
			})
			.SetPhysicsUpdate((double delta) => {
				if (_touchingPlayer) {
					_meleeAttack.AttackIfReady(_player);
				}
			})
			.Build();

		AiState preparingDashState = new AiState.Builder(PreparingDashState)
			.SetStart(() => {
				_animationPlayer.Play(DashAnimation);
				_moveVector = (_player.Position - Position).Normalized();
				Velocity = _moveVector * _dashSpeed;
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
			.SetUpdate((double delta) => {
				MoveAndSlide();
			})
			.SetPhysicsUpdate((double delta) => {
				if (_touchingPlayer) {
					_meleeAttack.AttackIfReady(_player);
				}
			})
			.Build();

		_stateMachine = new StateMachine.Builder(WanderingState)
			.AddState(wanderState)
			.AddState(pursueState)
			.AddState(preparingDashState)
			.AddState(dashingState)
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
