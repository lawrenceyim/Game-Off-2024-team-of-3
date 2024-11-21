using Godot;

public partial class Werewolf : CharacterBody2D {
	private const float _wanderingSpeed = 200f;
	private const double _minWanderTime = 3f;
	private const double _maxWanderTime = 5f;
	private const int _baseHealth = 15;
	private const double _attackCooldown = 3f;
	private const int _attackDamage = 2;
	private const float _detectionRange = 500;
	private const float _timeUntilLanding = 1f;
	private const int _landingDamage = 3;
	private const float closeEnoughRange = 30f;
	private const string WanderingState = "wander";
	private const string PursuitState = "pursue";
	private const string JumpingState = "jumping";
	private const string LandingState = "landing";
	private const string AttackingState = "attacking";
	private const string DeathState = "death";
	private const string MoveAnimation = "move";
	private const string MeleeAttackAnimation = "attack";
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private AnimatedSprite2D _sprite;
	[Export] private AlertLabel _alertLabel;
	private StateMachine _stateMachine;
	private PlayerCharacter _player;
	private MeleeAttack _meleeAttack;
	private Wander _wander;
	private Health _health;
	private bool _touchingPlayer = false;

	public override void _Ready() {
		PlayerCharacter.GetInstanceWithCallback((PlayerCharacter player) => {
			_player = player;

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

	// Called when melee attack animation finishes
	public void MeleeAttackFinished() {
		_stateMachine.SwitchState(PursuitState);
	}

	private void ChangeSpriteDirection() {
		if (Velocity.X > 0) {
			_sprite.FlipH = false;
		} else {
			_sprite.FlipH = true;
		}
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

				if (Position.DistanceTo(_player.Position) <= _detectionRange) {
					_stateMachine.SwitchState(PursuitState);
					return;
				}
			})
			.Build();

		AiState pursueState = new AiState.Builder(PursuitState)
			.SetStart(() => {
				_animationPlayer.Play(MoveAnimation);
				_alertLabel.DisplayExclamationMark();
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => {
				ChangeSpriteDirection();
			})
			.SetPhysicsUpdate((double delta) => {
				float distanceFromTarget = Position.DistanceTo(_player.Position);
				if (distanceFromTarget > _detectionRange) {
					_alertLabel.DisplayQuestionMark();
					_stateMachine.SwitchState(WanderingState);
					return;
				}
				if (distanceFromTarget > closeEnoughRange) {
					Velocity = (_player.Position - Position).Normalized() * _wanderingSpeed;
					MoveAndSlide();
				}

				if (_touchingPlayer && _meleeAttack.CanAttack()) {
					_stateMachine.SwitchState(AttackingState);
				}
			})
			.Build();

		AiState attackingState = new AiState.Builder(AttackingState)
			.SetStart(() => {
				_animationPlayer.Play(MeleeAttackAnimation);
				_meleeAttack.AttackIfReady(_player);
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => { })
			.SetPhysicsUpdate((double delta) => { })
			.Build();

		AiState jumpingState = new AiState.Builder(JumpingState)
			.SetStart(() => {
				// Play jumping animation
			})
			.SetExit(() => {
				// Turn invisible
				// Turn off collision shapes 
				_stateMachine.SwitchState(LandingState);
			})
			.SetUpdate((double delta) => { })
			.SetPhysicsUpdate((double delta) => { })
			.Build();

		AiState landingState = new AiState.Builder(LandingState)
			.SetStart(() => {
				// pick landing location
				// play sprite
				// start timer for landing

				// once timer times out, play animation for landing
				// damage to all bodies stored in a hashset of enemies within area
				// switch to pursuit state
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => { })
			.SetPhysicsUpdate((double delta) => { })
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
			.AddState(attackingState)
			.AddState(deathState)
			.AddState(jumpingState)
			.AddState(landingState)
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
