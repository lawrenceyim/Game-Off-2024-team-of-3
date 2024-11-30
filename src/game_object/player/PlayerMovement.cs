using Godot;

public partial class PlayerMovement : Node {
	private const float DashSpeed = 650f;
	private const float speed = 200f;
	private const float DashCooldown = 1f;
	private const float DashDuration = .65f;
	[Export] private CharacterBody2D _body;
	[Export] private PlayerAnimation _animation;
	[Export] private AudioStreamPlayer2D _movementAudioPlayer;
	[Export] private AudioStream _runningAudio;
	[Export] private AudioStream _dashingAudio;
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
		if (move.IsZeroApprox()) {
			_movementAudioPlayer.Stop();
			_animation.UpdateMovement(move);
		} else if (!_dashDurationTimer.IsStopped()) {
			_body.MoveAndCollide(_dashVelocity * (float)GetProcessDeltaTime());
			_animation.StartDashAnimation();
		} else {
			_body.MoveAndCollide(move * speed * (float)GetProcessDeltaTime());
			_animation.UpdateMovement(move);
			if (_movementAudioPlayer.Stream != _runningAudio) {
				_movementAudioPlayer.Stream = _runningAudio;
				_movementAudioPlayer.Stop();
			}
			if (!_movementAudioPlayer.Playing) {
				_movementAudioPlayer.Play();
			}
		}
		_dashDirection = move;
	}

	public void Dash() {
		if (_dashCooldownTimer.IsStopped() && !_dashDirection.IsZeroApprox()) {
			_dashVelocity = _dashDirection * DashSpeed;
			_dashDurationTimer.Start(DashDuration);
			_dashCooldownTimer.Start(DashCooldown);

			if (_movementAudioPlayer.Stream != _dashingAudio) {
				_movementAudioPlayer.Stream = _dashingAudio;
				_movementAudioPlayer.Play();
			}
		}
	}
}
