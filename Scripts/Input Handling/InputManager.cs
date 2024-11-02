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
    private static InputManager instance;
    private Stack<IInputListener> listenerStack;
    private IInputListener activeListener;

    private InputManager() {
        GD.Print("InputManager created.");
    }

    public static InputManager GetInstance() {
        GD.Print("InputManager.GetInstance called.");
        if (instance == null) {
            instance = new InputManager();
        }
        return instance;
    }

    public override void _Ready() {
        instance = this;
        listenerStack = new Stack<IInputListener>();
    }

    public override void _Process(double delta) {
        activeListener?.ProcessInput();
    }

    public void AddListener(IInputListener listener) {
        GD.Print("Listener added");
        if (activeListener == null) {
            activeListener = listener;
            return;
        }
        listenerStack.Push(activeListener);
        activeListener = listener;
    }

    public void ReturnToPreviousListener() {
        if (listenerStack.Count > 0) {
            activeListener = listenerStack.Pop();
        }
    }

    public void EmptyListenerStack() {
        listenerStack.Clear();
    }

    public void ClearActiveListener() {
        activeListener = null;
    }

    public void RemoveAllListeners() {
        ClearActiveListener();
        EmptyListenerStack();
    }
}
