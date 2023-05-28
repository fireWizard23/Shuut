using System;
using Godot;
using Shuut.Scripts;

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

        await Parent.CreateTimer(TimeSpan.FromMilliseconds(Parent.Rng.RandiRange(1000, 2000)));
        if (StateManager.CurrentStateEnum != State.Idle) 
            return;
        ChangeState(State.Wandering);


    }
}