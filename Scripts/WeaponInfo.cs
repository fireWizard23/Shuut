using Godot;
using MonoCustomResourceRegistry;

namespace Shuut.Scripts;


[RegisteredType(nameof(WeaponInfo))]
public partial  class WeaponInfo : Resource
{
    [Export] public float AttackAnimationLength = 1;
    [Export] public float RecoveryAnimationLength = 1;
    [Export] public float WindupAnimationLength = 1;
}