using System;

public class AiState {
	public readonly string stateName;
	public readonly Action onStart;
	public readonly Action onExit;
	public readonly Action<double> onUpdate;
	public readonly Action<double> onPhysicsUpdate;

	private AiState(string stateName, Action start, Action exit, Action<double> update, Action<double> physicsUpdate) {
		this.stateName = stateName;
		onStart = start;
		onExit = exit;
		onUpdate = update;
		onPhysicsUpdate = physicsUpdate;
	}

	public class Builder {
		private string _stateName;
		private Action _onSetUp;
		private Action _onStart;
		private Action _onExit;
		private Action<double> _onUpdate;
		private Action<double> _onPhysicsUpdate;

		public Builder(string stateName) {
			_stateName = stateName;
		}

		public Builder SetUp(Action setUp) {
			setUp();
			return this;
		}

		public Builder SetStart(Action onStart) {
			_onStart = onStart;
			return this;
		}

		public Builder SetExit(Action onExit) {
			_onExit = onExit;
			return this;
		}

		public Builder SetUpdate(Action<double> onUpdate) {
			_onUpdate = onUpdate;
			return this;
		}

		public Builder SetPhysicsUpdate(Action<double> onPhysicsUpdate) {
			_onPhysicsUpdate = onPhysicsUpdate;
			return this;
		}

		public AiState Build() {
			return new AiState(_stateName, _onStart, _onExit, _onUpdate, _onPhysicsUpdate);
		}
	}

}