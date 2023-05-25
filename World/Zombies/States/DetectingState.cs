using Godot;
using Godot.Collections;

namespace Shuut.World.Zombies.States;

public class DetectingState : BaseState<State, ZombieController>
{
    public override void OnEnter()
    {
        base.OnEnter();
        var space = Parent.GetWorld2D().DirectSpaceState;
        var exclude = new Array<Rid>() {Parent.GetRid(), ((CollisionObject2D)Parent.Target).GetRid()};

        var query = new PhysicsRayQueryParameters2D()
        {
            From = Parent.GlobalPosition,
            To = Parent.Target.GlobalPosition,
            Exclude = exclude,
        };
        var res = space.IntersectRay(query);
        if(res.Count > 0)
        {
            Parent.Target = null;
            ChangeState(State.Idle);
        }
        
        
    }

    public override void PhysicsProcess(double delta)
    {
        var dot = Vector2.Right.Rotated(Parent.GlobalRotation)
            .Dot(Parent.GlobalPosition.DirectionTo(Parent.Target.GlobalPosition));
        GD.Print(dot);
        if (dot is > 0 and < 0.5f || Parent.Target.GlobalPosition.DistanceTo(Parent.GlobalPosition) < Constants.Tile.Size)
        {
            ChangeState(State.Chasing);
        }
        else
        {
            StateManager.PreviousState.PhysicsProcess(delta);
        }
    }
    
}