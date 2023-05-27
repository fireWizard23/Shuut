using Godot;
using Shuut.Scripts.Hitbox;
using Shuut.Scripts.Hurtbox;
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

	public override void _Ready()
	
	{
		base._Ready();
		if(Hitbox != null) 
			Hitbox.OnHitboxHit += _on_hitbox_on_hitbox_hit;
	}

	protected virtual void _on_hitbox_on_hitbox_hit(Hurtbox hurtbox)
	{
		hurtbox.Hurt(new()
		{
			Damage =  WeaponInfo.BaseDamage + Handler.Parent.BaseDamage,
			Source =  WeaponOwner
		});
	}
	
}
