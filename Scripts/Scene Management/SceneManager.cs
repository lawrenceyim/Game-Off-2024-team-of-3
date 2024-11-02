/*
	Add as AUTOLOAD
*/

using Godot;
using System.Collections.Generic;

public partial class SceneManager : Node
{
	private static SceneManager _instance;
	private Stack<Node> _sceneStack;
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
        _sceneStack = new Stack<Node>();
    }

	public void PauseAndAddScene(Node scene) {
		GD.Print("Scene paused and added.");
		if (_activeScene == null) {
			_activeScene = scene;
			return;
		}
		PauseScene(_activeScene);
		_sceneStack.Push(_activeScene);
		_activeScene = scene;
		GD.Print("Stack count: " + _sceneStack.Count);
	}

	public void AddScene(Node scene) {
		GD.Print("Scene added.");
		if (_activeScene == null) {
			_activeScene = scene;
			return;
		}
		_sceneStack.Push(_activeScene);
		_activeScene = scene;
		GD.Print("Stack count: " + _sceneStack.Count);
	}

	public void ReplaceCurrentScene(Node scene) {
		GD.Print("Current scene replaced.");
		_activeScene = scene;
	}

	public void ReturnToPreviousScene() {
		GD.Print("Return to previous scene called.");
		if (_sceneStack.Count > 0) {
			_activeScene?.QueueFree();
			_activeScene = _sceneStack.Pop();
			UnpauseScene(_activeScene);
		}
	}

	public void EmptySceneStack() {
		_sceneStack.Clear();
	}

	public void PauseScene(Node scene) {
		GD.Print("Scene Paused.");
		scene.ProcessMode = ProcessModeEnum.Disabled;
		foreach(Node node in scene.GetChildren()) {
			PauseScene(node);
		}
	}

	public void UnpauseScene(Node scene) {
		GD.Print("Scene unpaused.");
		scene.ProcessMode = ProcessModeEnum.Inherit;
		foreach(Node node in scene.GetChildren()) {
			UnpauseScene(node);
		}
	}
}
