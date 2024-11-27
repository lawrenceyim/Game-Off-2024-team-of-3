using Godot;

public partial class HitFlash : Node {
	private const float EffectDuration = .25f;
	private const float LerpPercent = 1;
	private const double TweenFinalValue = 0;
	[Export] private ShaderMaterial _shaderMaterial;
	[Export] private Node2D _sprite;
	private Tween _tween;

	public override void _Ready() {
		_sprite.Material = _shaderMaterial;
	}

	public void DisplayHitFlash() {
		if (_tween != null && _tween.IsValid()) {
			_tween.Kill();
		}

		(_sprite.Material as ShaderMaterial)?.SetShaderParameter("lerp_percent", LerpPercent);
		_tween = CreateTween();
		_tween.TweenProperty(_sprite.Material, "shader_parameter/lerp_percent", TweenFinalValue, EffectDuration)
			.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

	}
}