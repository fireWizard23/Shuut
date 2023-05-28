using System;
using System.Threading.Tasks;
using Godot;

namespace Shuut.Scripts;

public static class Extensions {
    public static Task ToTask(this SignalAwaiter signalAwaiter)
    {
        var task = Task.Run(async () => await signalAwaiter);
        return task;
    }

    public static SignalAwaiter CreateTimer(this Node node, TimeSpan time)
    {
        var timer = node.GetTree().CreateTimer(time.TotalSeconds);
        return node.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
    }

}