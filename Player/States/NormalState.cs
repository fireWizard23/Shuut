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
                ChangeState(State.Dashing);
                Parent.inputBuffer.Reset();
                break;
            case "attack":
                ChangeState(State.Attacking);
                Parent.inputBuffer.Reset();
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
    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);
        if (!Parent._weaponHandler.OwnerCanRotate) return;
        var targetAngle = Parent.GlobalPosition.DirectionTo(Parent.GetGlobalMousePosition()).Angle();
        Parent.Rotation = (float)Mathf.LerpAngle(Parent.Rotation, targetAngle, 0.5f);

    }
}