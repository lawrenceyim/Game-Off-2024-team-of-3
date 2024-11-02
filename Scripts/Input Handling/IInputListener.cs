/*
	This interface is used to create classes that require input handling. Any input handling should be defined
	in the ProcessInput function, and the ProcessInput function will be called inside the _Process function of the 
	InputManager singleton. ProcessInput should not be called anywhere else.
 */
public interface IInputListener {
    void ProcessInput();
}
