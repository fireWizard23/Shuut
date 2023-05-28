using Godot;

namespace Shuut.Scripts;

public interface IDamager
{
    public int BaseDamage { get; set; }
    public Vector2 GlobalPosition { get; set; }
}