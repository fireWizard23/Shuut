using Godot;
using MonoCustomResourceRegistry;

namespace Shuut.Scripts;


[RegisteredType(nameof(EntityStats))]
public partial class EntityStats : Resource
{
    [Export] private float _tileMovementSpeed = 1;
    [Export] public float StartingHealth { get; set; } = 100;
    
    [Export] public int BaseDamage = 1, Poise = 50;

    public float MovementSpeed => _tileMovementSpeed * Constants.Tile.Size;

}