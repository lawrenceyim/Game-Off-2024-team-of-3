using Godot;

public partial class Wander {
	private readonly float _wanderingSpeed;
	private CharacterBody2D _body;
	private Timer _wanderingTimer;
	private double _minWanderingTime;
	private double _maxWanderingTime;

	public Wander(CharacterBody2D body, Timer timer, double minTime, double maxTime, float wanderSpeed) {
		_body = body;
		_wanderingTimer = timer;
		_minWanderingTime = minTime;
		_maxWanderingTime = maxTime;
		_wanderingSpeed = wanderSpeed;

		_wanderingTimer.Timeout += SetWanderingVelocity;
	}

	public void SetWanderingVelocity() {
		_wanderingTimer.Start(RandomNumber.RandomDoubleBetween(_minWanderingTime, _maxWanderingTime));
		_wanderingTimer.Paused = false;
		_body.Velocity = new Vector2(RandomNumber.RandomFloatBetween(-1, 1), RandomNumber.RandomFloatBetween(-1, 1)).Normalized() * _wanderingSpeed;

	}

	public void StopWandering() {
		_wanderingTimer.Paused = true;
	}
}
