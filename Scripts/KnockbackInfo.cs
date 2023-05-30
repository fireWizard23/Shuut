using Godot;

namespace Shuut.Scripts;


public struct KnockbackInfo
{
    public Vector2 Direction { get; set; }
    public float Distance { get; set; }
    public bool IsStunned;
}