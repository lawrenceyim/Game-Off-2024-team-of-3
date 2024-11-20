using Godot;
using System.Collections.Generic;

public partial class StateMachine : Node {
	private readonly Dictionary<string, AiState> _stateDict = new Dictionary<string, AiState>();
	private AiState _currentState;

	public void AddState(string key, AiState aiState) {
		_stateDict.Add(key, aiState);
	}

	public string GetCurrentState() {
		return _currentState.stateName;
	}

	public override void _Process(double delta) {
		_currentState?.onUpdate(delta);
	}

	public override void _PhysicsProcess(double delta) {
		_currentState?.onPhysicsUpdate(delta);
	}

	public void SwitchState(string key) {
		AiState newState = _stateDict.GetValueOrDefault(key, null);
		_currentState?.onExit();
		newState?.onStart();
		_currentState = newState;
	}

	public class Builder {
		private readonly StateMachine _stateMachine = new StateMachine();
		private readonly string _startingStateKey;

		public Builder(string startingStateKey) {
			_startingStateKey = startingStateKey;
		}

		public Builder AddState(AiState state) {
			_stateMachine.AddState(state.stateName, state);
			return this;
		}

		public StateMachine Build() {
			_stateMachine.SwitchState(_startingStateKey);
			return _stateMachine;
		}
	}
}
