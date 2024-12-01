using System;
using Godot;

public partial class PlayerCamera : Camera2D {
	private const float AimSensitivity = .1f;
	private const float AimLerpDuration = .25f;
	private const float NoiseSpeed = 5;
	private const float DecayRate = .5f;
	private const float Amplitude = 100f;
	private const float TraumaDuration = 1f;
	private readonly FastNoiseLite _noise = new FastNoiseLite();
	private Timer _traumaTimer;
	private double _trauma = 0;
	private float _traumaPower = 2;
	private float _noiseY = 0;
	private float _deadZone = 150;
	private Vector2 _aimedPosition = Vector2.Zero;

	public override void _Ready() {
		_traumaTimer = TimerUtil.CreateTimer(this, true);
		_noise.Seed = RandomNumber.RandomIntBetween(0, 3);
		_noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
	}

	public override void _PhysicsProcess(double delta) {
		Vector2 newPosition = _aimedPosition * AimSensitivity;
		Position = Position.Lerp(newPosition, AimLerpDuration);

		if (_traumaTimer.IsStopped()) {
			_trauma = 0;
			return;
		}

		if (_trauma > 0) {
			_trauma = Math.Max(_trauma - DecayRate * delta, 0);
			_noiseY += NoiseSpeed;
			ShakeScreen();
		}
	}

	public void SetAimedPosition(Vector2 aimedPosition) {
		_aimedPosition = aimedPosition;
	}

	public void AddTrauma(float amount) {
		_trauma = Math.Min(_trauma + amount, 1);
		_traumaTimer.Start(TraumaDuration);
	}

	public void ClearTrauma() {
		_trauma = 0;
		_traumaTimer.Stop();
	}

	private void ShakeScreen() {
		float amount = (float)Math.Pow(_trauma, _traumaPower);
		Vector2 newOffset = Offset;
		newOffset.X = Amplitude * amount * _noise.GetNoise2D(_noise.Seed, _noiseY);
		newOffset.Y = Amplitude * amount * _noise.GetNoise2D(_noise.Seed * 2, _noiseY);
		Offset = newOffset;
	}
}