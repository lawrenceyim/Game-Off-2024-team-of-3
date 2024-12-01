/*
    A level scene is a scene that represents a game level and will be used with SceneTransitionZone and SceneManager
    for scene management.    
*/
using Godot;

public partial class LevelScene : Node {
	public override void _Ready() {
        // SceneManager.GetInstance().ReplaceCurrentScene(this);
	}

	public override void _ExitTree() {
        // InputManager.GetInstance().RemoveAllListeners();
	}
}