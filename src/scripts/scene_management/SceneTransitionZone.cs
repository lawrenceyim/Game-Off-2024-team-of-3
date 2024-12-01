using Godot;

public partial class SceneTransitionZone : Area2D {
	[Export] private PackedScene _nextScene;

	public override void _Ready() {
		BodyEntered += ChangeScene;
	}

	public void ChangeScene(Node2D body) {
		GD.Print("Player enetered zone");
		if (body is PlayerCharacter) {
			CallDeferred(nameof(DeferredChangeScene));
		}
	}

	private void DeferredChangeScene() {
		SceneManager.GetInstance().SwitchLevel(_nextScene);
	}
}
