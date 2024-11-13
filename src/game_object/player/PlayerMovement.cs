using Godot;

public partial class PlayerMovement : Node {
	[Export] private CharacterBody2D _body;
	private float _speed = 500f;

	public void Move(Vector2 move) {
		_body.MoveAndCollide(move * _speed * (float)GetProcessDeltaTime());
	}

	public void SetSpeed(float speed) {
		_speed = speed;
	}
}
