using System.Threading.Tasks;
using Godot;

namespace Shuut.Scripts;

public static class Extensions {
    public static Task ToTask(this SignalAwaiter signalAwaiter)
    {
        var task = Task.Run(async () => await signalAwaiter);
        return task;
    }

    public static SignalAwaiter CreateTimer(this Node node, float seconds)
    {
        var timer = node.GetTree().CreateTimer(seconds);
        return node.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
    }

    public static SignalAwaiter CreateTimer(this Node node, int ms) => node.CreateTimer((float)ms / 1000);
}