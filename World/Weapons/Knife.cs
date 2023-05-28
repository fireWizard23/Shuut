using Godot;
using System.Threading.Tasks;
using Shuut.Scripts;
using Hurtbox = Shuut.Scripts.Hurtbox.Hurtbox;

namespace Shuut.World.Weapons;

public partial class Knife : BaseMeleeWeapon
{
	private bool _isAttacking;

	private Tween _currentTween;
	public TaskCompletionSource CancelTask = new TaskCompletionSource();
	private float _origRot;

	public override async Task Use()
	{
		if (WeaponState == WeaponState.Idle)
		{
			await Attack();
		}
	}

	public override void _Ready()
	{
		base._Ready();
		var sprite = GetChildOrNull<Sprite2D>(0);
		if(sprite != null ) 
			sprite.Position = Vector2.Right.Rotated(Mathf.DegToRad(-30)) * (Handler.WeaponDistanceFromHandler + DistanceFromOwner) ;
	}



	async Task Attack()
	{
		_isAttacking = true;

		_origRot = Rotation;
		await CurrentAnimation.WaitAsync();
		var windup = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear).SetParallel();
		windup.TweenProperty(this, "rotation", Rotation-Mathf.DegToRad(90), WeaponInfo.WeaponAnimationsDuration.WindupAnimationLength);
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
		var attackSpeed = WeaponInfo.WeaponAnimationsDuration.AttackAnimationLength / 2;
		var attack1 = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut).SetParallel();
		attack1.TweenProperty(this, "rotation", _origRot, attackSpeed).SetDelay(0.15f);
		await ToSignal(attack1, Tween.SignalName.Finished);

		

		var attack2 = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut).SetParallel();

		
		attack2.TweenProperty(this, "rotation", _origRot + Mathf.DegToRad(90), attackSpeed);
		
		await ToSignal(attack2, Tween.SignalName.Finished);
		
		Hitbox.TurnOff();
		Handler.OwnerCanMove = true;
		Handler.OwnerCanRotate = true;

		// Recovery
		WeaponState = WeaponState.Recovery;
		var recovery = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut).SetParallel();
		recovery.TweenProperty(this, "rotation", _origRot, WeaponInfo.WeaponAnimationsDuration.RecoveryAnimationLength);
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
			Rotation = _origRot;
			CancelTask = new();
			CurrentAnimation.Release();
		}
	}

	
}
