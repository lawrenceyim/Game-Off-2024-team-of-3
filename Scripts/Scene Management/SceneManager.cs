/*
	Add as AUTOLOAD
*/

using Godot;
using System.Collections.Generic;

public partial class SceneManager : Node
{
	private static SceneManager instance;
	private Stack<Node> sceneStack;
	private Node activeScene;

	private SceneManager() {
		GD.Print("SceneManager created.");
	}

	public static SceneManager GetInstance() {
        GD.Print("SceneManager.GetInstance called.");
		if (instance == null) {
			instance = new SceneManager();
		}
		return instance;
	}

	public override void _Ready() {
        instance = this;
        sceneStack = new Stack<Node>();
    }

	public void PauseAndAddScene(Node scene) {
		GD.Print("Scene paused and added.");
		if (activeScene == null) {
			activeScene = scene;
			return;
		}
		PauseScene(activeScene);
		sceneStack.Push(activeScene);
		activeScene = scene;
		GD.Print("Stack count: " + sceneStack.Count);
	}

	public void AddScene(Node scene) {
		GD.Print("Scene added.");
		if (activeScene == null) {
			activeScene = scene;
			return;
		}
		sceneStack.Push(activeScene);
		activeScene = scene;
		GD.Print("Stack count: " + sceneStack.Count);
	}

	public void ReplaceCurrentScene(Node scene) {
		GD.Print("Current scene replaced.");
		activeScene = scene;
	}

	public void ReturnToPreviousScene() {
		GD.Print("Return to previous scene called.");
		if (sceneStack.Count > 0) {
			activeScene?.QueueFree();
			activeScene = sceneStack.Pop();
			UnpauseScene(activeScene);
		}
	}

	public void EmptySceneStack() {
		sceneStack.Clear();
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
