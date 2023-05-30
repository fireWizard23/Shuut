using System;
using Shuut.Scripts;

namespace Shuut.World.Zombies.States;

public class EnemyDetected : BaseState<State, ZombieController>
{
    public override async void OnEnter()
    {
        base.OnEnter();
        Parent.DesiredVelocity *= 0;
        Parent.DetectionCue.Text = "!";
        await Parent.CreateTimer(TimeSpan.FromMilliseconds(150));
        Parent.DetectionCue.Text = "!!";
        await Parent.CreateTimer(TimeSpan.FromMilliseconds(150));
        Parent.DetectionCue.Text = string.Empty;
        ChangeState(State.Chasing);
    }
}