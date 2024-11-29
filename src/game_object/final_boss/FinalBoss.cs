using Godot;
using System;

public partial class FinalBoss : CharacterBody2D, IDamageable {
	private const float Phase1Speed = 200f;
	private const float Phase2Speed = 350f;
	private const int BaseHealth = 5;
	private const double MeleeAttack1Cooldown = 1f;
	private const double MeleeAttack2Cooldown = 1f;
	private const int Attack1Damage = 1;
	private const int Attack2Damage = 2;
	private const float CloseEnoughRange = 50f;
	private const string PursuitState = "pursue";
	private const string AttackState1 = "attack1";
	private const string AttackState2 = "attack2";
	private const string DeathState = "death";
	private const string MoveAnimation = "move";
	private const string IdleAnimation = "idle";
	private const string AttackAnimation1 = "attack1";
	private const string AttackAnimation2 = "attack2";
	private const string ResetAnimation = "RESET";
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private AnimatedSprite2D _sprite;
	[Export] private HitFlash _hitFlash;
	private StateMachine _stateMachine;
	private PlayerCharacter _player;
	private MeleeAttack _meleeAttack;
	private Health _health;
	private bool _firstPhase = true;
	private bool _touchingPlayer = false;

	public override void _Ready() {
		PlayerCharacter.GetInstanceWithCallback((PlayerCharacter player) => {
			_player = player;

			SetStateMachine();
		});

		_health = new Health(BaseHealth);
		_health.ZeroHealthEvent += (_, _) => _stateMachine.SwitchState(DeathState);

		_meleeAttack = new MeleeAttack(TimerUtil.CreateTimer(this, true), MeleeAttack1Cooldown, Attack1Damage);

		_animationPlayer.Play(MoveAnimation);
	}

	public void TakeDamage(int damage) {
		_hitFlash.DisplayHitFlash();
		_health.DecreaseHealth(damage);
	}

	private void OnAttackAnimationFinished() {
		_stateMachine.SwitchState(PursuitState);
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
				if (distanceFromTarget > CloseEnoughRange) {
					_animationPlayer.SpeedScale = _firstPhase ? 1 : 2;
					Velocity = (_player.Position - Position).Normalized() * (_firstPhase ? Phase1Speed : Phase2Speed);
					MoveAndSlide();
					_animationPlayer.Play(MoveAnimation);
				} else {
					_animationPlayer.Play(IdleAnimation);
				}

				if (_touchingPlayer && _meleeAttack.CanAttack()) {
					_stateMachine.SwitchState(_firstPhase ? AttackState1 : AttackState2);
				}
			})
			.Build();

		AiState attackState1 = new AiState.Builder(AttackState1)
			.SetStart(() => {
				_animationPlayer.SpeedScale = 1;
				_animationPlayer.Play(AttackAnimation1);
				_meleeAttack.AttackIfReady(_player);
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => {
			})
			.SetPhysicsUpdate((double delta) => { })
			.Build();

		AiState attackState2 = new AiState.Builder(AttackState2)
			.SetStart(() => {
				_animationPlayer.SpeedScale = 1;
				_animationPlayer.Play(AttackAnimation2);
				_meleeAttack.AttackIfReady(_player);
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => { })
			.SetPhysicsUpdate((double delta) => { })
			.Build();

		AiState deathState = new AiState.Builder(DeathState)
			.SetStart(() => {
				if (_firstPhase) {
					_firstPhase = false;
					_health.IncreaseHealth(BaseHealth);
					_meleeAttack = new MeleeAttack(TimerUtil.CreateTimer(this, true), MeleeAttack2Cooldown, Attack2Damage);
					_stateMachine.SwitchState(PursuitState);
					return;
				}
				// Play death SFX
				// Play death animation
				QueueFree();
			})
			.SetExit(() => { })
			.SetUpdate((double delta) => { })
			.SetPhysicsUpdate((double delta) => { })
			.Build();

		_stateMachine = new StateMachine.Builder(PursuitState)
			.AddState(pursueState)
			.AddState(attackState1)
			.AddState(attackState2)
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

	private void ChangeSpriteDirection() {
		if (Velocity.X > 0) {
			_sprite.FlipH = false;
		} else {
			_sprite.FlipH = true;
		}
	}
}
