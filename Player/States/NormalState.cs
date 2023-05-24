using System.Xml;
using Godot;
using Shuuut.World;

namespace Shuuut.Player.States;

public class NormalState : BaseState<State, Player>
{
    public override void OnEnter()
    {
        base.OnEnter();
        if (!Parent.inputBuffer.IsUsed) return;
        switch (Parent.inputBuffer.InputUsed)
        {
            case "dash":
                GD.Print("DASH BUFFER!");
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
            Parent.inputBuffer.Reset();
            ChangeState(State.Attacking);
        }

        if (Input.IsActionJustPressed("switch_weapon_up"))
        {
            Parent.inputBuffer.Reset();
            Parent._weaponHandler.UnequipWeapon();
        }

        if (Input.IsActionJustPressed("dash"))
        {
            Parent.inputBuffer.Reset();
            ChangeState(State.Dashing);
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