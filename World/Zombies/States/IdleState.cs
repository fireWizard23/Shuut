using Godot;

namespace Shuut.World.Zombies.States;

public class IdleState : BaseState<State, ZombieController>
{

    public override async void OnEnter()
    {
        base.OnEnter();
        Parent.DesiredVelocity *= 0;
        if (StateManager.PreviousState is null)
        {
            ChangeState(State.Wandering);
            return;
        }

        if (Parent.Target != null)
        {
            ChangeState(State.Chasing);
            return;
        }

        if (StateManager.PreviousStateEnum is not (State.Wandering or State.Chasing))
        {
            ChangeState(State.Wandering);
            return;
        }

        await Parent.ToSignal(Parent.GetTree().CreateTimer(Parent.Rng.RandiRange(1000, 2000)), SceneTreeTimer.SignalName.Timeout);
        if (StateManager.CurrentStateEnum != State.Idle) return;
        ChangeState(State.Wandering);


    }
}