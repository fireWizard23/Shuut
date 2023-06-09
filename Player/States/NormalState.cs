﻿using Godot;
using Shuut.World;

namespace Shuut.Player.States;

public class NormalState : BaseState<State, Player>
{
    public override void OnEnter()
    {
        base.OnEnter();
        if (!Parent.InputBuffer.IsUsed) return;
        switch (Parent.InputBuffer.InputUsed)
        {
            case "attack":
                Parent.InputBuffer.Reset(); 
                ChangeState(State.Attacking);
                break;
        }

    }

    public override void Process(double delta)
    {
        if (Input.IsActionJustPressed("attack"))
        {
            ChangeState(State.Attacking);
            Parent.InputConsumed = true;
        }

        if (Input.IsActionJustPressed("switch_weapon_up"))
        {
            Parent.WeaponHandler.UnequipWeapon();
            Parent.InputConsumed = true;
        }

        Parent.InputConsumed = true;
    }

}