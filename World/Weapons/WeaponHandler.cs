using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Shuut.Player;
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

	private BaseWeapon _knife;
	public IDamager Parent;
	

	public override void _Ready()
	{
		base._Ready();
		_knife = GetChild<BaseWeapon>(0);
		Parent = GetParent() as IDamager;;
		_knife.SetAttackMask(
			((IAttacker)GetParent()).AttackMask );
		_knife.Sheath();
		EquipWeapon();
	}

	public async Task EquipWeapon()
	{
		await _knife.OnEquip();
	}

	public async void UnequipWeapon()
	{
		await _knife.Sheath();
		await _knife.OnUnequip();
		CurrentState = State.InSheath;
	}

	public async Task UseWeapon()
	{
		switch (CurrentState)
		{
			case State.InSheath:
				await EquipWeapon();
				CurrentState = State.Ready;
				await _knife.UnSheath();
				break;
			case State.Ready:
				await _knife.Use();
				break;
		}
	}

	public void Cancel()
	{
		_knife.OnCancel();
	}
	
	

}
