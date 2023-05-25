using Godot;

namespace Shuut;

public partial class Background : TileMap
{
	public override void _Ready()
	{
        World.Pathfinding.Instance.Bake(this);
	}

	public override void _Process(double delta)
	{
	}
}
