using System.Threading;
using System.Threading.Tasks;
using Godot;
using Shuut.Player;
using Shuut.Scripts;

namespace Shuut.World.Weapons;


public enum WeaponState
{
    Idle,
    Windup,
    Attacking,
    Recovery,
}

public abstract partial class BaseWeapon : Node2D
{
    [Export] protected float DistanceFromOwner = 0;
    [Export] public WeaponInfo WeaponInfo;
    protected WeaponHandler Handler;
    protected bool IsEquipped = false;

    protected readonly SemaphoreSlim CurrentAnimation = new(1);
    
    public IDamager WeaponOwner { get; set; }

    public WeaponState WeaponState = WeaponState.Idle;

    
    public override void _Ready()
    {
        Handler = GetParent() as WeaponHandler;
        Enable(IsEquipped);
    }

    public void Setup(IDamager weaponOwner)
    {
        this.WeaponOwner = weaponOwner;
    }


    public abstract void SetAttackMask(uint mask);


    public virtual async Task Sheath()
    {
		
        await CurrentAnimation.WaitAsync();
        var tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear).SetParallel();
        tween.TweenProperty(this, "modulate:a", 0, 0.25f);
        await ToSignal(tween, Tween.SignalName.Finished);
        CurrentAnimation.Release();
        Enable(false);
    }

    public virtual async Task UnSheath()
    {
        await CurrentAnimation.WaitAsync();

        var tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear).SetParallel();
        tween.TweenProperty(this, "modulate:a", 1, 0.25f);
        await ToSignal(tween, Tween.SignalName.Finished);
        CurrentAnimation.Release();
        Enable();
    }

    public abstract Task Use();
    public abstract Task OnCancel();
    
    

    protected void Enable(bool v=true)
    {
        IsEquipped = v;
        SetProcess(v);
        SetPhysicsProcess(v);
    }
    
    public async Task OnEquip()
    {
        Enable();
    }

    public async Task OnUnequip()
    {
        Enable(false);
    }

}