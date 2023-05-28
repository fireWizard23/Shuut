using Shuut.World;

namespace Shuut.Player.States;

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
        await Parent.WeaponHandler.UseWeapon();
        ChangeState(State.Normal);
    }

}