using Godot;

public partial class PlayerMovement : Node {
	private const float DashSpeed = 1000f;
	private const float speed = 500f;
	private const float DashCooldown = 1f;
	private const float DashDuration = .75f;
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
			_body.MoveAndCollide(move * speed * (float)GetProcessDeltaTime());
			_animation.UpdateMovement(move);
		}
		_dashDirection = move;
	}

	public void Dash() {
		if (_dashCooldownTimer.IsStopped() && !_dashDirection.IsZeroApprox()) {
			_dashVelocity = _dashDirection * DashSpeed;
			_dashDurationTimer.Start(DashDuration);
			_dashCooldownTimer.Start(DashCooldown);
		}
	}
}
