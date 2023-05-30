using Godot;

namespace Shuut.Scripts;

public interface IDamager
{
    public int BaseDamage { get; }
    public Vector2 GlobalPosition { get; set; }
}