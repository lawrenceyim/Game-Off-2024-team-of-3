using System;
using Godot;

public partial class BatEnemy : CharacterBody2D, IDamageable {
	private StateMachine _stateMachine;
	private PlayerCharacter _player;
	private Vector2 _moveVector;
	private Timer _timer;
	private Timer _attackCooldownTimer;

	private float _detectionRange = 1000;
	private float _speed = 300f;

	private double _minWanderTime = 3f;
	private double _maxWanderTime = 5f;

	private double _attackCooldown = .5f;
	private int _attackDamage = 1;


	private Health _health;
	private int _baseHealth = 5;

	private const string Wander = "wander";
	private const string Pursue = "pursue";

	public override void _Ready() {
		_player = PlayerCharacter.GetInstance();
		if (_player == null) {
			GD.Print("Player is null");
			QueueFree();
		}

		_health = new Health(_baseHealth);
		_health.ZeroHealthEvent += InitiateDeath;

		_attackCooldownTimer = TimerUtil.CreateTimer(this, true);
		_timer = TimerUtil.CreateTimer(this, true);
		_timer.Timeout += HandleTimeOut;

		// State machine creation
		AiState wanderState = new AiState.Builder(Wander)
			.SetStart(() => {
				SetRandomWander();
			})
			.SetExit(() => {
				_timer.Stop();
				_timer.Paused = true;
			})
			.SetUpdate((double delta) => {
				MoveAndCollide(_moveVector * _speed * (float)delta);
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
				_moveVector = (_player.Position - Position).Normalized();
				KinematicCollision2D collision = MoveAndCollide(_moveVector * _speed * (float)delta);
				if (collision == null) {
					return;
				}
				if (collision.GetCollider() is PlayerCharacter player) {
					AttackIfReady(player);
				}
			})
			.SetPhysicsUpdate((double delta) => {
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
	}

	private void FinishDeath() {
		QueueFree();
	}

	private void SetRandomWander() {
		_timer.Start(RandomNumber.RandomDoubleBetween(_minWanderTime, _maxWanderTime));
		_timer.Paused = false;
		_moveVector = new Vector2(RandomNumber.RandomFloatBetween(-1, 1), RandomNumber.RandomFloatBetween(-1, 1));
	}

	private void HandleTimeOut() {
		string currentState = _stateMachine.GetCurrentState();
		if (currentState.Equals(Wander)) {
			SetRandomWander();
		}
	}
}
