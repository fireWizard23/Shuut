using System.Threading.Tasks;
using Shuut.World.Weapons;

namespace Shuut.World.Zombies.States;

public class AttackingState : BaseState<State, ZombieController>
{
    private bool _canAttack = true;

    private Task weaponAnim;
    
    public override void OnEnter()
    {
        base.OnEnter();
        Parent.DesiredVelocity *= 0;
        Attack();
    }

    public override void OnExit()
    {
        base.OnExit();
        if (weaponAnim is { IsCompleted: true })
        {
            _canAttack = true;
        }
    }

    private async void Attack()
    {
        if (!_canAttack)
        {
            return;
        }
        _canAttack = false;
        await Parent.CreateTimer(200);
        if (StateManager.CurrentStateEnum != State.Attacking)
        {
            _canAttack = true;
            return;
        }
        weaponAnim = Parent.WeaponHandler.UseWeapon();
        await weaponAnim;
        _canAttack = true;
    }

    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);
        Parent.LookAt(Parent.Target.GlobalPosition);
        switch (_canAttack)
        {
            case false when Parent.GlobalPosition.DistanceTo(Parent.Target.GlobalPosition) > Constants.Tile.Size * 0.8f:
                ChangeState(State.Idle);
                break;
            case true:
                Attack();
                break;
        }
    }
}