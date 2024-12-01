/*
	A level scene is a scene that represents a game level and will be used with SceneTransitionZone and SceneManager
	for scene management.    
*/
using Godot;

public partial class LevelScene : Node {
	public override void _Ready() {
		GD.Print("LEVELSCENE ATTACHED TO ", Name, " ", SceneFilePath);
		SceneManager.GetInstance().SetCurrentLevel(this, SceneFilePath);
	}
}
