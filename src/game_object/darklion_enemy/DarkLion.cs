using Godot;
using System;

public partial class DarkLion : CharacterBody2D {
	[Export] AnimatedSprite2D _sprite;
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
	private const string Wander = "wander";
	private const string Pursue = "pursue";
	private const string Dash = "dash";
	private const string Move = "move";

	public override void _Ready() {
		PlayerCharacter.GetInstanceWithCallback((PlayerCharacter player) => {
			_player = player;

			_dashCooldownTimer = TimerUtil.CreateTimer(this, true);
			_dashDurationTimer = TimerUtil.CreateTimer(this, true);

			_dashDurationTimer.Timeout += () => {
				_stateMachine.SwitchState(Pursue);
			};

			_wander = new Wander(this, TimerUtil.CreateTimer(this, true), _minWanderTime, _maxWanderTime, _wanderingSpeed);

			SetStateMachine();
		});

		_health = new Health(_baseHealth);
		_health.ZeroHealthEvent += InitiateDeath;
		_meleeAttack = new MeleeAttack(TimerUtil.CreateTimer(this, true), _attackCooldown, _attackDamage);

		_sprite.Play(Move);
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
		AiState wanderState = new AiState.Builder(Wander)
			.SetStart(() => {
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
					_stateMachine.SwitchState(Pursue);
					return;
				}
			})
			.Build();

		AiState pursueState = new AiState.Builder(Pursue)
			.SetStart(() => { })
			.SetExit(() => { })
			.SetUpdate((double delta) => {
				if (_dashCooldownTimer.TimeLeft == 0) {
					_stateMachine.SwitchState(Dash);
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

		AiState dashingState = new AiState.Builder(Dash)
			.SetStart(() => {
				_moveVector = (_player.Position - Position).Normalized();
				Velocity = _moveVector * _dashSpeed;
				_dashCooldownTimer.Start(_dashCooldown);
				_dashDurationTimer.Start(_dashDuration);
				ChangeSpriteDirection();
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => {
				MoveAndSlide();
			})
			.SetPhysicsUpdate((double delta) => {
				if (_touchingPlayer) {
					_meleeAttack.AttackIfReady(_player);
				}
				if (Position.DistanceTo(_player.Position) > _detectionRange) {
					_stateMachine.SwitchState(Wander);
					return;
				}
			})
			.Build();

		_stateMachine = new StateMachine.Builder(Wander)
			.AddState(wanderState)
			.AddState(pursueState)
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
}
