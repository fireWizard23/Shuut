using Godot;
using Shuut.Scripts;
using Shuut.World;
using Shuut.World.Weapons;

namespace Shuut.Player.States;

public class DashingState : BaseState<State, Player>
{
    private bool _shouldExit;
    private float _distanceTraveled;
    private Vector2 _direction;

    public override void OnEnter()
    {
        base.OnEnter();
        _direction = Parent.InputDirection.LengthSquared() > 0 ? Parent.InputDirection : -Vector2.Right.Rotated(Parent.GlobalRotation);
    }

    public override async void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);
        if (_shouldExit)
        {
            Parent.Velocity = Vector2.Zero;
            return;
        }

        var speed = 1000;
        Parent.Velocity = _direction * speed;
        _distanceTraveled += speed * (float)delta;
        if (!(_distanceTraveled >= Parent.DashLength)) return;
        
        _distanceTraveled = 0;
        _shouldExit = true;
        
        await Parent.CreateTimer(150);
        _shouldExit = false;
        ChangeState(State.Normal);
    }
}