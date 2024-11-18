using Godot;

public partial class AlertLabel : Node2D {
	[Export] private Label _alertLabel;
	public Timer _alertTimer;
	public const float _alertDuration = .5f;

	public override void _Ready() {
		_alertLabel.Visible = false;

		_alertTimer = TimerUtil.CreateTimer(this, true);
		_alertTimer.Timeout += () => _alertLabel.Visible = false;
	}

	public void DisplayExclamationMark() {
		_alertLabel.Visible = true;
		_alertLabel.Text = "!";
		_alertTimer.Start(_alertDuration);
	}

	public void DisplayQuestionMark() {
		_alertLabel.Visible = true;
		_alertLabel.Text = "?";
		_alertTimer.Start(_alertDuration);
	}
}
