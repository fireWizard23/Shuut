using Godot;

namespace Shuut.Scripts;

public interface IDamager
{
    public uint AttackMask { get; set; }	
    public int BaseDamage { get; }
    public Vector2 GlobalPosition { get; set; }
}