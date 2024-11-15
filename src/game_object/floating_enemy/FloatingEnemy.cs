using Godot;
using System;

public partial class FloatingEnemy : CharacterBody2D, IDamageable {
	private const float _wanderingSpeed = 100f;
	private const double _minWanderTime = 3f;
	private const double _maxWanderTime = 5f;
	private const int _baseHealth = 5;
	private const double _attackCooldown = .5f;
	private const int _attackDamage = 1;
	private const string WanderingState = "wander";
	private const string PursuitState = "pursue";
	private const string MoveAnimation = "move";
	[Export] AnimatedSprite2D _sprite;
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
		_health.ZeroHealthEvent += InitiateDeath;

		_meleeAttack = new MeleeAttack(TimerUtil.CreateTimer(this, true), _attackCooldown, _attackDamage);
		_rangedAttack = new RangedAttack(this, TimerUtil.CreateTimer(this, true));

		_sprite.Play(MoveAnimation);
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
			.SetStart(() => { })
			.SetExit(() => { })
			.SetUpdate((double delta) => {
				_moveVector = (_player.Position - Position).Normalized();
				Velocity = _moveVector * _wanderingSpeed;
				MoveAndSlide();
			})
			.SetPhysicsUpdate((double delta) => {
				_rangedAttack.AttackIfReady(_player);
				if (_touchingPlayer) {
					_meleeAttack.AttackIfReady(_player);
				}
				if (Position.DistanceTo(_player.Position) > _detectionRange) {
					_stateMachine.SwitchState(WanderingState);
					return;
				}
			})
			.Build();

		_stateMachine = new StateMachine.Builder(WanderingState)
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
