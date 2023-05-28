using System;
using Godot;
using Shuut.Scripts;
using Shuut.World.Weapons;

namespace Shuut.World.Zombies.States;

public class KnockbackState : BaseState<State, ZombieController>
{
    private float _distanceTraveled;

    private bool _shouldExit;

    public override async void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);
        if (_shouldExit)
        {
            Parent.Velocity = Vector2.Zero;
            return;
        }
        Parent.Velocity = Parent.KnockbackInfo.Direction * 500;
        _distanceTraveled += 500 * (float)delta;
        if (!(_distanceTraveled >= Parent.KnockbackInfo.Distance)) return;
        
        _distanceTraveled = 0;
        _shouldExit = true;
        await Parent.CreateTimer(TimeSpan.FromMilliseconds(250));
        _shouldExit = false;
        ChangeState(State.Idle);
    }
}