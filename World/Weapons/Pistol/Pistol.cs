using Godot;
using System;
using System.Threading.Tasks;
using Shuut.Scripts;
using Shuut.World.Weapons;

public partial class Pistol : BaseWeapon
{
	[Export] public new RangedWeaponInfo WeaponInfo;
	[Export] private PackedScene _bullet;
	[Export] private Node2D muzzle;
	private bool _canShoot = true;
	private uint mask;

	public override void _Ready()
	{
		base._Ready();
		Position = Vector2.Right * Handler.WeaponDistanceFromHandler;
	}

	public override void SetAttackMask(uint mask)
	{
		this.mask = mask;
	}

	public override async Task Sheath()
	{
	}

	public override async Task UnSheath()
	{
	}

	public override async Task Use()
	{
		if (!_canShoot)
		{
			return;
		}
		_canShoot = false;
		await CurrentAnimation.WaitAsync();
		for (int i = 0; i < WeaponInfo.ShootCount; i++)
		{
			var s = _bullet.Instantiate();
			GetTree().Root.AddChild(s);
			if (s is Bullet bullet)
			{
				bullet.CallDeferred
				(
					nameof(bullet.Setup), 
					muzzle.GlobalPosition, 
					Vector2.Right.Rotated(GlobalRotation),
					mask,
					Handler.Parent.BaseDamage + WeaponInfo.BaseDamage
				);
			}
			await Task.Delay((int)(WeaponInfo.ShootCountInterval * 1000));
		}

		CurrentAnimation.Release();
		GoCooldown();
	}

	private async void GoCooldown()
	{
		await Task.Delay((int)(WeaponInfo.ShootCooldown * 1000));
		_canShoot = true;
	}

	public override Task OnCancel() => Task.CompletedTask;

}
