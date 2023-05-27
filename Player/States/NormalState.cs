using Godot;
using Shuut.Player;
using Shuut.World;

namespace Shuut.Player.States;

public class NormalState : BaseState<State, Player>
{
    public override void OnEnter()
    {
        base.OnEnter();
        if (!Parent.inputBuffer.IsUsed) return;
        switch (Parent.inputBuffer.InputUsed)
        {
            case "dash":
                Parent.inputBuffer.Reset();
                ChangeState(State.Dashing);
                break;
            case "attack":
                Parent.inputBuffer.Reset(); 
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
            Parent._weaponHandler.UnequipWeapon();
            Parent.InputConsumed = true;
        }

        if (Input.IsActionJustPressed("dash"))
        {
            ChangeState(State.Dashing);
            Parent.InputConsumed = true;
        }
    }

}