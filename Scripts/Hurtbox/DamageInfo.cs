using Godot;

namespace Shuut.Scripts.Hurtbox;

public partial class DamageInfo : RefCounted
{
    public int Damage;
    public IDamager Source;
}