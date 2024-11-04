using Godot;

public partial class PlayerMovement : Node {
	[Export] private CharacterBody2D _body;
	[Export] private float _speed;

	public void Move(Vector2 move) {
		_body.MoveAndCollide(move * _speed * (float) this.GetProcessDeltaTime());
	}

	public void SetSpeed(float speed) {
		_speed = speed;
	}
}
