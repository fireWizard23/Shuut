using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Shuut.Player;
using Shuut.Scripts;
using Shuut.World.Zombies;

namespace Shuut.World.Weapons;



public enum State
{
	InSheath,
	Ready
}

public partial class WeaponHandler : Node2D
{
	private float _weaponDistanceFromHandler = 0.5f;

	public bool OwnerCanMove = true;
	public bool OwnerCanRotate = true;


	public State CurrentState = State.InSheath;

	public float WeaponDistanceFromHandler => _weaponDistanceFromHandler * Constants.Tile.Size;

	private BaseWeapon _weapon;
	public IDamager Parent;
	

	public override void _Ready()
	{
		base._Ready();
		_weapon = GetChild<BaseWeapon>(0);
		Parent = GetParent() as IDamager;;
		_weapon.Setup(Parent);
		_weapon.SetAttackMask(
			((IDamager)GetParent()).AttackMask );
		_weapon.Sheath();
		EquipWeapon();
	}

	public async Task EquipWeapon()
	{
		await _weapon.OnEquip();
	}

	public async void UnequipWeapon()
	{
		await _weapon.Sheath();
		await _weapon.OnUnequip();
		CurrentState = State.InSheath;
	}

	public async Task UseWeapon()
	{
		switch (CurrentState)
		{
			case State.InSheath:
				await EquipWeapon();
				CurrentState = State.Ready;
				await _weapon.UnSheath();
				break;
			case State.Ready:
				await _weapon.Use();
				break;
		}
	}

	public void Cancel()
	{
		_weapon.OnCancel();
	}
	
	

}
