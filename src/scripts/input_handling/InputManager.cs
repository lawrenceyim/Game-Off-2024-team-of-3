/*
	Add as AUTOLOAD
    
    This class is a Singleton that handles all input in the game. A stack is used to handle switches in input.
    For example, when the player is in the overworld, a "CharacterMovement" object may be active. When they 
    open the main menu, the "CharacterMovement" object will be added to the stack and a "Menu" object will become
    the active listener to turn off inputs for character movement and only handle input for the menu. Once the
    player returns from the menu, the "CharacterMovement" object will be popped from the stack and set as the 
    active listener again.
 */

using Godot;
using System.Collections.Generic;

public partial class InputManager : Node {
	private static InputManager _instance;
	private Stack<IInputListener> _listenerStack;
	private IInputListener _activeListener;

	private InputManager() {
		// GD.Print("InputManager created.");
	}

	public static InputManager GetInstance() {
		// GD.Print("InputManager.GetInstance called.");
		if (_instance == null) {
			_instance = new InputManager();
		}
		return _instance;
	}

	public override void _Ready() {
		_instance = this;
		_listenerStack = new Stack<IInputListener>();
	}

	public override void _Process(double delta) {
		_activeListener?.ProcessInput();
	}

	public void AddListener(IInputListener listener) {
		// GD.Print("Listener added");
		if (_activeListener == null) {
			_activeListener = listener;
			return;
		}
		_listenerStack.Push(_activeListener);
		_activeListener = listener;
	}

	public void ReturnToPreviousListener() {
		if (_listenerStack.Count > 0) {
			_activeListener = _listenerStack.Pop();
		}
	}

	public void EmptyListenerStack() {
		_listenerStack.Clear();
	}

	public void ClearActiveListener() {
		_activeListener = null;
	}

	public void RemoveAllListeners() {
		ClearActiveListener();
		EmptyListenerStack();
	}
}
