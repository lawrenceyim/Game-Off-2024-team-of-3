using System;

public class AiState {
    public readonly Action onStart;
    public readonly Action onExit;
    public readonly Action<double> onUpdate;
    public readonly Action<double> onPhysicsUpdate;

    private AiState(Action start, Action exit, Action<double> update, Action<double> physicsUpdate) {
        onStart = start;
        onExit = exit;
        onUpdate = update;
        onPhysicsUpdate = physicsUpdate;
    }

    public class Builder {
        private Action _onStart;
        private Action _onExit;
        private Action<double> _onUpdate;
        private Action<double> _onPhysicsUpdate;

        public Builder() {

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
            if (_onStart == null || _onExit == null || _onUpdate == null || _onPhysicsUpdate == null) {
                throw new Exception("State is not fully initialized");
            }
            return new AiState(_onStart, _onExit, _onUpdate, _onPhysicsUpdate);
        }
    }

}