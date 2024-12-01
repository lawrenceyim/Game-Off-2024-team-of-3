using Godot;

public partial class SceneTransitionZone : Area2D {
	[Export] private Node _currentScene;
	[Export] private PackedScene _nextScene;

	public override void _Ready() {
		BodyEntered += ChangeScene;
	}

	public void ChangeScene(Node2D body) {
		GD.Print("Player enetered zone");
		if (body is PlayerCharacter) {
			Node newScene = _nextScene.Instantiate();
			GetTree().Root.AddChild(newScene);
			_currentScene.QueueFree();
			// SceneManager.GetInstance().AddChild(_nextScene.Instantiate());
		}
	}
}
