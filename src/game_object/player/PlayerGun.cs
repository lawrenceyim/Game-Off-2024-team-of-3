using Godot;

public partial class PlayerGun : Sprite2D {
	public void AimGun(Vector2 aimedPosition) {
		Rotation = aimedPosition.Angle();
	}
}
