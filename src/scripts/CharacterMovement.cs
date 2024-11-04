using Godot;

public partial class CharacterMovement : Node, IInputListener {
    public override void _Ready() {
        InputManager.GetInstance().AddListener(this);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }

    public void ProcessInput() {
        if (Input.IsActionPressed("MoveUp")) {
            GD.Print("MoveUp pressed");
        }
    }
}
