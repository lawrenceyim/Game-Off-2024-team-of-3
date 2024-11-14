/* 
	Add as Autoload

*/

using Godot;

public partial class WindowSize : Node {
	Vector2I[] _sizes = new Vector2I[] {
		new Vector2I(2560, 1440),
		new Vector2I(1920, 1080),
		new Vector2I(1280, 720),
		new Vector2I(640, 360)
	};

	public override void _Ready() {
		SetScreenSize(_sizes[2]);
	}

	public void SetScreenSize(Vector2I size) {
		GetWindow().Size = size;
	}
}
