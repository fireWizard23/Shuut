using Godot;
using MonoCustomResourceRegistry;

namespace Shuut.Scripts;

[RegisteredType(nameof(WeaponInfo))]
public partial  class WeaponInfo : Resource
{
    [Export] public WeaponAnimationsDuration WeaponAnimationsDuration = new();
    [Export] public int BaseDamage = 1;
    


}