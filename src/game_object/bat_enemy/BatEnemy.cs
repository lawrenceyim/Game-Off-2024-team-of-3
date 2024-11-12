using System;
using Godot;

public partial class BatEnemy : CharacterBody2D, IDamageable {
    private StateMachine _stateMachine;
    private PlayerCharacter _player;
    private Vector2 _moveVector;
    private Timer _timer;
    private Timer _attackCooldownTimer;
    private float _detectionRange = 10;
    private float _speed = 10f;
    private double _minWanderTime = 3f;
    private double _maxWanderTime = 5f;
    private double _attackCooldown = 3f;
    private Health _health;
    private int _baseHealth = 5;

    private const string Wander = "wander";
    private const string Pursue = "pursue";

    public override void _Ready() {
        _player = PlayerCharacter.GetInstance();

        _health = new Health(_baseHealth);
        _health.ZeroHealthEvent += InitiateDeath;

        _attackCooldownTimer = TimerUtil.CreateTimer(this);
        _timer = TimerUtil.CreateTimer(this);
        _timer.Timeout += HandleTimeOut;

        // State machine creation
        AiState wanderState = new AiState.Builder(Wander)
            .SetStart(() => {
                SetRandomWander();
            })
            .SetExit(() => {
                _timer.Stop();
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
                MoveAndCollide(_moveVector * _speed * (float)delta);
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
        _moveVector = new Vector2(RandomNumber.RandomFloatBetween(-1, 1), RandomNumber.RandomFloatBetween(-1, 1));
    }

    private void HandleTimeOut() {
        string currentState = _stateMachine.GetCurrentState();
        if (currentState.Equals(Wander)) {
            SetRandomWander();
        }
    }
}
