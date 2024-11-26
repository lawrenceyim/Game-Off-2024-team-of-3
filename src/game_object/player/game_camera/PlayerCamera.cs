using System;
using Godot;

public partial class PlayerCamera : Camera2D {
	private const float AimSensitivity = .1f;
	private const float AimLerpDuration = .25f;
	private const float NoiseSpeed = 5;
	private const float DecayRate = .5f;
	private const float Amplitude = 15f;
	private const float TraumaDuration = 1;
	private readonly FastNoiseLite _noise = new FastNoiseLite();
	[Export] private PlayerCharacter _playerCharacter;
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

	private void ShakeScreen() {
		float amount = (float)Math.Pow(_trauma, _traumaPower);
		Vector2 newOffset = Offset;
		newOffset.X = Amplitude * amount * _noise.GetNoise2D(_noise.Seed, _noiseY);
		newOffset.Y = Amplitude * amount * _noise.GetNoise2D(_noise.Seed * 2, _noiseY);
		Offset = newOffset;
	}
}

/*


#adding trauma is what sets off the shake effect, 
#a timer makes sure a 2nd shake cant happen until the first one is done,
#the current parameters have a nice balance for the shake.
#timer is set to 0.2s oneshot = true

func _physics_process(delta: float) -> void:
	if !Global.player.weapon_sprite.inventory_mode:
		var target_position = target.aim_position * sensitivity
		position = position.lerp(target_position, 0.25)

	if trauma:
		trauma = max(trauma - decay * delta, 0)
		_noise_y += NOISE_SPEED
		_shake()
		
	if timer.is_stopped():
		trauma = 0
	
func add_trauma(amount: float):
	trauma = min(trauma + amount, 1.0)
	timer.start()
	
func _shake():
	var amount = pow(trauma, trauma_power)
	offset.x = amplitude * amount * noise.get_noise_2d(noise.seed, _noise_y)
	offset.y = amplitude * amount * noise.get_noise_2d(noise.seed * 2, _noise_y)

*/