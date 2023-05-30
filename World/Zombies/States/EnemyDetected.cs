using System;
using Godot;
using Shuut.Scripts;

namespace Shuut.World.Zombies.States;

public class EnemyDetected : BaseState<State, ZombieController>
{
    public override async void OnEnter()
    {
        base.OnEnter();
        Parent.DesiredVelocity *= 0;
        Parent.DetectionCue.Text = "!";
        await Parent.CreateTimer(TimeSpan.FromMilliseconds(300));
        if (Parent.GlobalPosition.DistanceTo(Parent.Target.GlobalPosition) >= Constants.Tile.Sizex5)
        {
            if (StateManager.PreviousStateEnum != null) ChangeState((State)StateManager.PreviousStateEnum);
            return;
        }
        Parent.DetectionCue.Text = "!!";
        await Parent.CreateTimer(TimeSpan.FromMilliseconds(150));
        Parent.DetectionCue.Text = string.Empty;
        ChangeState(State.Chasing);
    }

    public override void Process(double delta)
    {
        base.Process(delta);
		                                                                                        
        var targetAngle = Parent.GlobalPosition.DirectionTo(Parent.Target.GlobalPosition).Angle();                                        
        Parent.GlobalRotation = (float)Mathf.LerpAngle(Parent.GlobalRotation, targetAngle, 8 * delta);    
    }
}