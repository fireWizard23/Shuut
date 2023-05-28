using Shuut.Player;

namespace Shuut.Scripts.Hurtbox;

public partial class DamageInfo : Godot.GodotObject
{
    public int Damage;
    public IDamager Source;
}