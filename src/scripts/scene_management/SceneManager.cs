/*
	Add as AUTOLOAD
*/

using Godot;
using System.Collections.Generic;

public partial class SceneManager : Node {
	private static SceneManager _instance;
	private PackedScene _currentLevelPackedScene;
	private Node _activeScene;

	private SceneManager() {
		GD.Print("SceneManager created.");
	}

	public static SceneManager GetInstance() {
		GD.Print("SceneManager.GetInstance called.");
		if (_instance == null) {
			_instance = new SceneManager();
		}
		return _instance;
	}

	public override void _Ready() {
		_instance = this;
	}

	public void SwitchLevel(PackedScene newScene) {
		ClearAllProjectiiles();
		_activeScene.QueueFree();
		InputManager.GetInstance().ClearActiveListener();
		_currentLevelPackedScene = newScene;
		GetTree().Root.AddChild(_currentLevelPackedScene.Instantiate());
	}

	public void RestartLevel() {
		ClearAllProjectiiles();
		_activeScene.QueueFree();
		InputManager.GetInstance().ClearActiveListener();
		GetTree().Root.AddChild(_currentLevelPackedScene.Instantiate());
	}

	public void SetCurrentLevel(Node currentLevel, string filepath) {
		_activeScene = currentLevel;
		_currentLevelPackedScene = (PackedScene) ResourceLoader.Load(filepath);
	}

	private void ClearAllProjectiiles() {
		Node projectileRoot = GetTree().Root.GetNode("Projectiles");
		foreach (Node projectile in projectileRoot.GetChildren()) {
			projectile.QueueFree();
		}
	}
}
