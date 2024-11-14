using Godot;
using System;

public partial class DarkLion : CharacterBody2D {
	[Export] AnimatedSprite2D _sprite;
	private StateMachine _stateMachine;
	private PlayerCharacter _player;
	private Vector2 _moveVector;
	private Timer _wanderingTimer;
	private Timer _attackCooldownTimer;
	private Timer _dashTimer;


	private float _detectionRange = 1000;
	private float _speed = 100f;
	private float _dashSpeed = 1000f;
	private float _dashDuration = .5f;
	private float _dashCooldown = 5f;


	private double _minWanderTime = 3f;
	private double _maxWanderTime = 5f;

	private double _attackCooldown = .5f;
	private int _attackDamage = 1;
	private bool _touchingPlayer = false;


	private Health _health;
	private int _baseHealth = 10;

	private const string Wander = "wander";
	private const string Pursue = "pursue";

	private const string Move = "move";

	public override void _Ready() {
		PlayerCharacter.GetInstanceWithCallback((PlayerCharacter player) => {
			_player = player;

			_attackCooldownTimer = TimerUtil.CreateTimer(this, true);
			_dashTimer = TimerUtil.CreateTimer(this, true);
			_wanderingTimer = TimerUtil.CreateTimer(this, true);
			_wanderingTimer.Timeout += HandleTimeOut;

			SetStateMachine();
		});

		_health = new Health(_baseHealth);
		_health.ZeroHealthEvent += InitiateDeath;

		_sprite.Play(Move);
	}

	public void TakeDamage(int damage) {
		_health.DecreaseHealth(damage);
	}

	private void AttackIfReady(IDamageable damageable) {
		if (_attackCooldownTimer.TimeLeft > 0) {
			return;
		}
		_attackCooldownTimer.Start(_attackCooldown);
		_attackCooldownTimer.Paused = false;
		damageable.TakeDamage(_attackDamage);
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

	private void SetRandomWander() {
		_wanderingTimer.Start(RandomNumber.RandomDoubleBetween(_minWanderTime, _maxWanderTime));
		_wanderingTimer.Paused = false;
		_moveVector = new Vector2(RandomNumber.RandomFloatBetween(-1, 1), RandomNumber.RandomFloatBetween(-1, 1));
	}

	private void HandleTimeOut() {
		string currentState = _stateMachine.GetCurrentState();
		if (currentState.Equals(Wander)) {
			SetRandomWander();
		}
	}

	private void SetStateMachine() {
		AiState wanderState = new AiState.Builder(Wander)
			.SetStart(() => {
				SetRandomWander();
			})
			.SetExit(() => {
				_wanderingTimer.Stop();
				_wanderingTimer.Paused = true;
			})
			.SetUpdate((double delta) => {
				Velocity = _moveVector * _speed;
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
				// Add feature for chase() vs dash()
				_moveVector = (_player.Position - Position).Normalized();
				Velocity = _moveVector * _speed;
				MoveAndSlide();
			})
			.SetPhysicsUpdate((double delta) => {
				if (_touchingPlayer) {
					AttackIfReady(_player);
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
			.Build();

		AddChild(_stateMachine);
	}

	private void _on_hitbox_area_2d_area_entered(Area2D body) {
		if (body.GetParent() is PlayerCharacter) {
			GD.Print("Touching player");
			_touchingPlayer = true;
		}
	}

	private void _on_hitbox_area_2d_area_exited(Area2D body) {
		if (body.GetParent() is PlayerCharacter) {
			GD.Print("No longer touching player");
			_touchingPlayer = false;
		}
	}
}
