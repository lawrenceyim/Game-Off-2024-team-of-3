using Godot;

public partial class PlayerMovement : Node {
	private const float _dashSpeed = 1000f;
	private const float _speed = 500f;
	private const float _dashCooldown = 1f;
	private const float _dashDuration = .75f;
	[Export] private CharacterBody2D _body;
	[Export] private PlayerAnimation _animation;
	private float _dashAngle = 0;
	private Timer _dashCooldownTimer;
	private Timer _dashDurationTimer;
	private Vector2 _dashVelocity;
	private Vector2 _dashDirection;

	public override void _Ready() {
		_dashDurationTimer = TimerUtil.CreateTimer(this, true);
		_dashCooldownTimer = TimerUtil.CreateTimer(this, true);
	}

	public void Move(Vector2 move) {
		if (!_dashDurationTimer.IsStopped()) {
			_body.MoveAndCollide(_dashVelocity * (float)GetProcessDeltaTime());
			_animation.StartDashAnimation();
		} else {
			_body.MoveAndCollide(move * _speed * (float)GetProcessDeltaTime());
			_animation.UpdateMovement(move);
		}
	}

	public void SetDashDirection(Vector2 direction) {
		_dashDirection = direction;
	}

	public void Dash() {
		if (_dashCooldownTimer.IsStopped()) {
			_dashVelocity = _dashDirection * _dashSpeed;
			_dashDurationTimer.Start(_dashDuration);
			_dashCooldownTimer.Start(_dashCooldown);
		}
	}
}
