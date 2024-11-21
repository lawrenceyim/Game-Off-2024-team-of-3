using Godot;

public partial class AlertLabel : Node2D {
	private const string IndicatorAnimation = "indicator";
	[Export] private Label _alertLabel;
	[Export] private AnimationPlayer _animationPlayer;
	public Timer _alertTimer;
	public const float _alertDuration = .5f;

	public override void _Ready() {
		_alertLabel.Visible = false;
		_alertTimer = TimerUtil.CreateTimer(this, true);
		_alertTimer.Timeout += () => _alertLabel.Visible = false;
		_animationPlayer.AnimationFinished += (StringName name) => _alertLabel.Visible = false;
	}

	public void DisplayExclamationMark() {
		_animationPlayer.Stop();
		_animationPlayer.Play(IndicatorAnimation);
		_alertLabel.Visible = true;
		_alertLabel.Text = "!";
		_alertTimer.Start(_alertDuration);
	}

	public void DisplayQuestionMark() {
		_animationPlayer.Stop();
		_animationPlayer.Play(IndicatorAnimation);
		_alertLabel.Visible = true;
		_alertLabel.Text = "?";
		_alertTimer.Start(_alertDuration);
	}
}
