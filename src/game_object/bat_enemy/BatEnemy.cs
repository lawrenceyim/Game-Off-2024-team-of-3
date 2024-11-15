using System;
using Godot;

public partial class BatEnemy : CharacterBody2D, IDamageable {
	[Export] AnimatedSprite2D _sprite;
	private StateMachine _stateMachine;
	private PlayerCharacter _player;
	private Vector2 _moveVector;
	private Timer _wanderingTimer;
	private Timer _accelerationTimer;
	private MeleeAttack _meleeAttack;
	private Health _health;
	private float _detectionRange = 1000;
	private float _speed;
	private bool _touchingPlayer = false;
	private const float _initialSpeed = 100f;
	private const float _maxSpeed = 500f;
	private const float _accelerationTime = 5;
	private const double _minWanderTime = 3f;
	private const double _maxWanderTime = 5f;
	private const int _baseHealth = 5;
	private const double _attackCooldown = .5f;
	private const int _attackDamage = 1;
	private const string Wander = "wander";
	private const string Pursue = "pursue";
	private const string Move = "move";

	public override void _Ready() {
		PlayerCharacter.GetInstanceWithCallback((PlayerCharacter player) => {
			_player = player;

			_accelerationTimer = TimerUtil.CreateTimer(this, true);
			_wanderingTimer = TimerUtil.CreateTimer(this, true);
			_wanderingTimer.Timeout += HandleTimeOut;

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
				_speed = _initialSpeed;
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
			.SetStart(() => {
				_accelerationTimer.Start(_accelerationTime);
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => {
				_moveVector = (_player.Position - Position).Normalized();
				Velocity = _moveVector * _speed;
				MoveAndSlide();
			})
			.SetPhysicsUpdate((double delta) => {
				if (_accelerationTimer.TimeLeft > 0) {
					_speed = Math.Max((1 - ((float)_accelerationTimer.TimeLeft / _accelerationTime)) * _maxSpeed, _initialSpeed);
				}
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
