using Godot;
using Godot.NativeInterop;

public partial class BatEnemy : Node {
    private StateMachine _stateMachine;

    public override void _Ready() {
        _stateMachine = new StateMachine.Builder("wander")
        .AddState("wander", new AiState.Builder()
            .SetStart(() => { })
            .SetExit(() => { })
            .SetUpdate((double delta) => {
                GD.Print("Delta is " + delta);
            })
            .SetPhysicsUpdate((double delta) => {
                GD.Print("Physics delta is " + delta);
            })
            .Build())
        .Build();
        AddChild(_stateMachine);
    }
}
