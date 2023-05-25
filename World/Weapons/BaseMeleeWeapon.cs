using Godot;
using Shuut.Scripts.Hitbox;
using Hitbox = Shuut.Scripts.Hitbox.Hitbox;


namespace Shuut.World.Weapons;

public abstract partial class BaseMeleeWeapon : BaseWeapon
{
    [Export] protected Hitbox Hitbox;

    public  override void SetAttackMask(uint mask)
    {
        if(Hitbox != null)
            Hitbox.CollisionMask = mask;
    }
    
    
    
}