using Godot;
using MonoCustomResourceRegistry;

namespace Shuut.Scripts;


[RegisteredType(nameof(WeaponAnimationsDuration))]
public partial class WeaponAnimationsDuration : Resource
{
    [Export] public float AttackAnimationLength = 1;
    [Export] public float RecoveryAnimationLength = 1;
    [Export] public float WindupAnimationLength = 1;
}