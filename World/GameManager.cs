using Godot;
using System;

public partial class GameManager : Node2D
{
    [Export] public PackedScene mainScene;

    public void GoToMain()
    {
        GetTree().ChangeSceneToPacked(mainScene);
    }

}
