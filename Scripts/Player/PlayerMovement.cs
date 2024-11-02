using Godot;

public partial class PlayerMovement : Node {
    [Export] private CharacterBody2D body;
    [Export] private float speed;

    public void Move(Vector2 move) {
        body.MoveAndCollide(move * speed * (float) this.GetProcessDeltaTime());
    }

    public void SetSpeed(float speed) {
        this.speed = speed;
    }
}
