using System;
using System.Threading.Tasks;
using Godot;
using Shuut.Scripts;
using Shuut.Scripts.Hitbox;
using Hurtbox = Shuut.Scripts.Hurtbox.Hurtbox;

namespace Shuut.World.Weapons.Fist;

public partial class Fist : BaseMeleeWeapon
{
	private uint _mask;

	public override void _Ready()
	{
		base._Ready();
		Handler.CurrentState = State.Ready;
	}

	public override async Task Sheath()
	{
	}

	public override async Task UnSheath()
	{
	}

	public override void SetAttackMask(uint mask)
	{
		this._mask = mask;
	}

	public override async Task Use()
	{
		var space = GetWorld2D().DirectSpaceState;
		var query = new PhysicsRayQueryParameters2D()
		{
			CollideWithAreas = true,
			CollideWithBodies = false,
			From = GlobalPosition,
			To = GlobalPosition + Vector2.Right.Rotated(GlobalRotation) * 100,
			CollisionMask = _mask
		};
		var res = space.IntersectRay 
		(
			query
		);
		if (res.Count > 0)
		{
			if (res["collider"].As<Hurtbox>() is { } hurtbox)
			{
				Hitbox.EmitSignal(Hitbox.SignalName.OnHitboxHit, hurtbox);
			}
		}

		await Handler.CreateTimer(TimeSpan.FromMilliseconds(1000));
	}

	public override Task OnCancel() => Task.CompletedTask;

}