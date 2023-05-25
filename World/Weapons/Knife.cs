using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Godot;
using System.Threading.Tasks;
using Shuut.Scripts;
using Shuut.Scripts.Hurtbox;
using Hurtbox = Shuut.Scripts.Hurtbox.Hurtbox;

namespace Shuut.World.Weapons;

public static class Extensions {
	public static Task ToTask(this Godot.SignalAwaiter signalAwaiter)
	{
		var task = Task.Run(async () => await signalAwaiter);
		return task;
	}
}

public partial class Knife : BaseMeleeWeapon
{
	[Export] public WeaponInfo WeaponInfo;
	private bool _isAttacking;

	private Tween _currentTween;
	public TaskCompletionSource CancelTask = new TaskCompletionSource();
	private float origRot;

	public override async Task Use()
	{
		if (WeaponState == WeaponState.Idle)
		{
			await Attack();
		}
	}

	public override async Task Sheath()
	{
		
		await CurrentAnimation.WaitAsync();
		var tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear).SetParallel();
		tween.TweenProperty(this, "modulate:a", 0, 0.25f);
		await ToSignal(tween, Tween.SignalName.Finished);
		CurrentAnimation.Release();
		Enable(false);
	}

	public override async Task UnSheath()
	{
		await CurrentAnimation.WaitAsync();

		var tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear).SetParallel();
		tween.TweenProperty(this, "modulate:a", 1, 0.25f);
		await ToSignal(tween, Tween.SignalName.Finished);
		CurrentAnimation.Release();
		Enable();
	}

	async Task Attack()
	{
		_isAttacking = true;

		origRot = Rotation;
		await CurrentAnimation.WaitAsync();
		var windup = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear).SetParallel();
		windup.TweenProperty(this, "rotation", Rotation-Mathf.DegToRad(90), WeaponInfo.WindupAnimationLength);
		_currentTween = windup;
		WeaponState = WeaponState.Windup;
		
		var s = ToSignal(windup, Tween.SignalName.Finished);
		await Task.WhenAny( s.ToTask(), CancelTask.Task);
		if (WeaponState == WeaponState.Idle)
		{
			return;
		}	
		
		// Attack animation
		WeaponState = WeaponState.Attacking;
		Handler.OwnerCanMove = false;
		Handler.OwnerCanRotate = false;
		Hitbox.TurnOn();
		var attackSpeed = WeaponInfo.AttackAnimationLength / 2;
		var attack1 = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut).SetParallel();
		attack1.TweenProperty(this, "rotation", origRot, attackSpeed).SetDelay(0.15f);
		await ToSignal(attack1, Tween.SignalName.Finished);

		

		var attack2 = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut).SetParallel();

		
		attack2.TweenProperty(this, "rotation", origRot + Mathf.DegToRad(90), attackSpeed);
		
		await ToSignal(attack2, Tween.SignalName.Finished);
		
		Hitbox.TurnOff();
		Handler.OwnerCanMove = true;
		Handler.OwnerCanRotate = true;

		// Recovery
		WeaponState = WeaponState.Recovery;
		var recovery = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut).SetParallel();
		recovery.TweenProperty(this, "rotation", origRot, WeaponInfo.RecoveryAnimationLength);
		_currentTween = recovery;
		s = ToSignal(recovery, Tween.SignalName.Finished);
		await Task.WhenAny( s.ToTask(), CancelTask.Task );
		
		if (WeaponState == WeaponState.Idle)
		{
			return;
		}
		WeaponState = WeaponState.Idle;
		CurrentAnimation.Release();
		_isAttacking = false;
		Rotation = 0;
	}

	public override async Task OnCancel()
	{
		if (WeaponState != WeaponState.Attacking)
		{
			WeaponState = WeaponState.Idle;
			CancelTask.TrySetResult();
			_currentTween.Stop();
			_currentTween.Kill();
			Rotation = origRot;
			CancelTask = new();
			CurrentAnimation.Release();
		}
	}

	private void _on_hitbox_on_hitbox_hit(Hurtbox hurtbox)
	{
		hurtbox.Hurt(new()
		{
			Damage =  10,
			Source =  this
		});
	}
	
}
