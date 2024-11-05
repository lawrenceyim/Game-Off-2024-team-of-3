using System;
using Godot;

public partial class BatEnemy : CharacterBody2D {
    private StateMachine _stateMachine;
    private Node2D _player;
    private Vector2 _moveVector;
    private Timer _timer;
    private Timer _attackCooldownTimer;
    private float _detectionRange = 10;
    private float _speed = 10f;
    private double _minWanderTime = 3f;
    private double _maxWanderTime = 5f;
    private double _attackCooldown = 3f;

    private const string Wander = "wander";
    private const string Pursue = "pursue";
    private const string Attacking = "attacking";

    public override void _Ready() {
        // TODO: GET PLAYER REFERENCE. Probably make it a singleton since there's only one player object
        // _player = Player.GetInstance();

        _attackCooldownTimer = TimerUtil.CreateTimer(this);
        _timer = TimerUtil.CreateTimer(this);
        _timer.Timeout += HandleTimeOut;

        _stateMachine = new StateMachine.Builder(Wander)
            .AddState(new AiState.Builder(Wander)
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
                    // DELETE THIS NULL CHECK LATER
                    // This is only here to avoid throwing errors until we actually set the player reference later
                    if (_player == null) {
                        return;
                    }
                    if (Position.DistanceTo(_player.Position) <= _detectionRange) {
                        _stateMachine.SwitchState(Pursue);
                        return;
                    }
                })
                .Build())
            .AddState(new AiState.Builder(Pursue)
                .SetStart(() => { })
                .SetExit(() => { })
                .SetUpdate((double delta) => {
                    _moveVector = (_player.Position - Position).Normalized();
                    MoveAndCollide(_moveVector * _speed * (float)delta);
                })
                .SetPhysicsUpdate((double delta) => {
                    // DELETE THIS NULL CHECK LATER
                    // This is only here to avoid throwing errors until we actually set the player reference later
                    if (_player == null) {
                        return;
                    }
                    if (Position.DistanceTo(_player.Position) > _detectionRange) {
                        _stateMachine.SwitchState(Wander);
                        return;
                    }
                })
                .Build())
            .AddState(new AiState.Builder(Attacking)
                .SetStart(() => {
                    // Start animation

                    _attackCooldownTimer.Start(_attackCooldown);
                })
                .SetExit(() => {
                    _stateMachine.SwitchState(Pursue);
                })
                .SetUpdate((double delta) => { })
                .SetPhysicsUpdate((double delta) => { })
                .Build())
            .Build();
        AddChild(_stateMachine);
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
