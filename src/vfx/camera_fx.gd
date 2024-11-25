extends Camera2D

@export var target : Player 
@export var sensitivity : float = 0.1

@export var decay : float = 0.5
@export var amplitude : float = 15.0
@export var shake_sensitivity : float = 0.1

@onready var noise = FastNoiseLite.new()
@onready var timer: Timer = $Shake_Timer

var trauma = 0.0
var trauma_power = 2
var NOISE_SPEED = 5
var _noise_y = 0

var dead_zone = 150

#adding trauma is what sets off the shake effect, 
#a timer makes sure a 2nd shake cant happen until the first one is done,
#the current parameters have a nice balance for the shake.
#timer is set to 0.2s oneshot = true

func _ready() -> void: 
	randomize()
	noise.seed = randi_range(0, 3)
	noise.noise_type = FastNoiseLite.TYPE_PERLIN
	
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
