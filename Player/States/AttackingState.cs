using System.Threading.Tasks;
using Godot;
using Shuuut.Scripts;
using Shuuut.World;

namespace Shuuut.Player.States;

public class AttackingState : BaseState<State, Player>
{
    public override  void OnEnter()
    {
        base.OnEnter();
        Attack();
    }

    private async void Attack()
    {
        Parent.Rotation = Parent.GlobalPosition.DirectionTo(Parent.GetGlobalMousePosition()).Angle();
        await Parent._weaponHandler.UseWeapon();
        ChangeState(State.Normal);
    }

}