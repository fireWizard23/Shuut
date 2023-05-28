using Godot;

namespace Shuut.Scripts.Hurtbox;

public partial class Hurtbox : Area2D
{
    [Signal]
    public delegate void OnHurtEventHandler(DamageInfo d);

    public void Hurt(DamageInfo d)
    {
        EmitSignal(Shuut.Scripts.Hurtbox.Hurtbox.SignalName.OnHurt, d);
    }

}
