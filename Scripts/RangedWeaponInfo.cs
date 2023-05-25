using Godot;
using MonoCustomResourceRegistry;

namespace Shuut.Scripts;


[RegisteredType(nameof(RangedWeaponInfo))]
public partial class RangedWeaponInfo : WeaponInfo
{
    [Export] public float ShootCooldown = 1, ShootCount = 1, ShootCountInterval=0.25f, TilesShootRange=5;

    public float ShootRange => TilesShootRange * Constants.Tile.Size;

}