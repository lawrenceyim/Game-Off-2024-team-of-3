using System.Collections.Generic;
using Godot;

public partial class Werewolf : CharacterBody2D, IDamageable {
	private const float WanderingSpeed = 350f;
	private const double MinWanderTime = 3f;
	private const double MaxWanderTime = 5f;
	private const int BaseHealth = 15;
	private const double AttackCooldown = 1f;
	private const int AttackDamage = 2;
	private const float TimeUntilLanding = 1f;
	private const int LandingDamage = 3;
	private const float JumpAttackCooldown = 6f;
	private const float closeEnoughRange = 60f;
	private const string PursuitState = "pursue";
	private const string JumpingState = "jumping";
	private const string LandingState = "landing";
	private const string AttackingState = "attacking";
	private const string DeathState = "death";
	private const string MoveAnimation = "move";
	private const string IdleAnimation = "idle";
	private const string MeleeAttackAnimation = "attack";
	private const string JumpingAnimation = "leap";
	private const string LandingAnimation = "land";
	private const string DeathAnimation = "die";
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private AnimatedSprite2D _sprite;
	[Export] private AlertLabel _alertLabel;
	[Export] private CollisionShape2D _hurtBoxCollisionShape;
	[Export] private CollisionShape2D _hitBoxCollisionShape;
	[Export] private CollisionShape2D _movementCollisionShape;
	[Export] private Sprite2D _landingAreaOfEffectSprite;
	[Export] private CollisionShape2D _landingAreaOfEffectCollisionShape;
	[Export] private HitFlash _hitFlash;
	private StateMachine _stateMachine;
	private PlayerCharacter _player;
	private MeleeAttack _meleeAttack;
	private Wander _wander;
	private Health _health;
	private Timer _jumpAttackTimer;
	private Timer _landingTimer;
	private bool _touchingPlayer = false;
	private Color _landingAoESpriteColor = new Color(1, 0, 0, 0);
	private HashSet<IDamageable> _withinAoE = new HashSet<IDamageable>();

	public override void _Ready() {
		PlayerCharacter.GetInstanceWithCallback((PlayerCharacter player) => {
			_player = player;

			_wander = new Wander(this, TimerUtil.CreateTimer(this, true), MinWanderTime, MaxWanderTime, WanderingSpeed);
			_landingTimer = TimerUtil.CreateTimer(this, true);

			SetStateMachine();
		});

		_health = new Health(BaseHealth);
		_health.ZeroHealthEvent += (_, _) => _stateMachine.SwitchState(DeathState);

		_meleeAttack = new MeleeAttack(TimerUtil.CreateTimer(this, true), AttackCooldown, AttackDamage);

		_jumpAttackTimer = TimerUtil.CreateTimer(this, true);

		SetLandingAoESpriteColor();
		_landingAreaOfEffectCollisionShape.Disabled = true;

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

	public void MeleeAttackFinished() {
		_stateMachine.SwitchState(PursuitState);
	}

	public void JumpAnimationFinished() {
		_stateMachine.SwitchState(LandingState);
	}

	private void LandingAnimationFinished() {
		_stateMachine.SwitchState(PursuitState);
	}

	private void ChangeSpriteDirection() {
		if (Velocity.X > 0) {
			_sprite.FlipH = false;
		} else {
			_sprite.FlipH = true;
		}
	}

	private void SetLandingAoESpriteColor() {
		_landingAreaOfEffectSprite.SelfModulate = _landingAoESpriteColor;
	}

	private void SetStateMachine() {
		AiState pursueState = new AiState.Builder(PursuitState)
			.SetStart(() => {
				_animationPlayer.Play(MoveAnimation);
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => {
				ChangeSpriteDirection();
			})
			.SetPhysicsUpdate((double delta) => {
				float distanceFromTarget = Position.DistanceTo(_player.Position);
				if (distanceFromTarget > closeEnoughRange) {
					_animationPlayer.Play(MoveAnimation);
					Velocity = (_player.Position - Position).Normalized() * WanderingSpeed;
					MoveAndSlide();
				} else {
					_animationPlayer.Play(IdleAnimation);
				}

				if (_jumpAttackTimer.IsStopped()) {
					_stateMachine.SwitchState(JumpingState);
					return;
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
				_movementCollisionShape.Disabled = true;
				_hurtBoxCollisionShape.Disabled = true;
				_hitBoxCollisionShape.Disabled = true;
				_animationPlayer.Play(JumpingAnimation);
			})
			.SetExit(() => {
				_sprite.Visible = false;
			})
			.SetUpdate((double delta) => { })
			.SetPhysicsUpdate((double delta) => { })
			.Build();

		AiState landingState = new AiState.Builder(LandingState)
			.SetUp(() => {
				_landingTimer.Timeout += () => {
					_landingAoESpriteColor.A = 0;
					SetLandingAoESpriteColor();
					_sprite.Visible = true;
					_animationPlayer.Play(LandingAnimation);
					// DAMAGE ALL targets within aoe

					foreach (IDamageable damageable in _withinAoE) {
						damageable.TakeDamage(LandingDamage);
					}
				};
			})
			.SetStart(() => {
				Position = _player.Position;
				_landingTimer.Start(TimeUntilLanding);
				_withinAoE.Clear();
				_landingAreaOfEffectCollisionShape.Disabled = false;
			})
			.SetExit(() => {
				_jumpAttackTimer.Start(JumpAttackCooldown);
				_hurtBoxCollisionShape.Disabled = false;
				_hitBoxCollisionShape.Disabled = false;
				_movementCollisionShape.Disabled = false;
				_landingAreaOfEffectCollisionShape.Disabled = true;
			})
			.SetUpdate((double delta) => {
				if (!_landingTimer.IsStopped()) {
					_landingAoESpriteColor.A = 1 - ((float)_landingTimer.TimeLeft / TimeUntilLanding);
					SetLandingAoESpriteColor();
				}
			})
			.SetPhysicsUpdate((double delta) => { })
			.Build();

		AiState deathState = new AiState.Builder(DeathState)
			.SetStart(() => {
				_animationPlayer.Play(DeathAnimation);
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => { })
			.SetPhysicsUpdate((double delta) => { })
			.Build();

		_stateMachine = new StateMachine.Builder(PursuitState)
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

	private void _on_aoe_area_2d_area_entered(Area2D body) {
		if (body.GetParent() is IDamageable damageable && damageable != this) {
			_withinAoE.Add(damageable);
		}
	}

	private void _on_aoe_area_2d_area_exited(Area2D body) {
		if (body.GetParent() is IDamageable damageable && damageable != this) {
			_withinAoE.Remove(damageable);
		}
	}
}
