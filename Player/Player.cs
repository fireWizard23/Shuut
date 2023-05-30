using Godot;
using Shuut.Player.States;
using Shuut.Scripts;
using Shuut.Scripts.Poise;
using Shuut.World.Weapons;
using DamageInfo = Shuut.Scripts.Hurtbox.DamageInfo;

namespace Shuut.Player;

public enum State
{
	Normal,
	Attacking,
	InKnockback,
}

public partial class Player : StatefulEntity<State, Player>, IDamager
{
	[Export] public EntityStats GivenStats;
	[Export] private HealthController _healthController;
	[Export] public WeaponHandler WeaponHandler;
	[Export(PropertyHint.Layers2DPhysics)] public uint AttackMask { get; set;}
	[Export] public Label Label;

	public int BaseDamage => GivenStats.BaseDamage;
	private float Speed => GivenStats.MovementSpeed;
	private Poise _poise;


	public KnockbackInfo KnockbackInfo;
	private Vector2 _inputDirection;

    public readonly InputBuffer InputBuffer = new() { TimeMs = 500 };
    public bool InputConsumed;

    protected override void BeforeReady()
	{
		_poise.Setup(GivenStats.Poise);
		StateManager = new(
			new()
			{
				{ State.Normal,  new NormalState() },
				{ State.Attacking,  new AttackingState() },
				{ State.InKnockback,  new InKnockbackState() },
			},
			this
		);
	}

    public override void _Process(double delta)
    {
	    base._Process(delta);
	    _inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");

	    if (!InputConsumed)
	    {

		    if (Input.IsActionJustPressed("attack"))
		    {
			    InputBuffer.Use("attack");
		    }
	    }

	    InputConsumed = false;
	    Label.Text = StateManager.CurrentStateEnum.ToString();
    }

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		var velocity = Velocity;
		if (StateManager.CurrentStateEnum is not (State.InKnockback))
		{
			
			if (_inputDirection != Vector2.Zero)
			{
				velocity = _inputDirection.Normalized() * Speed;

			}
			else
			{
				velocity = velocity.MoveToward(Vector2.Zero, Speed);
			}

			Velocity = velocity;
			if (!WeaponHandler.OwnerCanMove)
			{
				Velocity *= 0;
			}
		}
		if (!WeaponHandler.OwnerCanRotate) return;
		var targetAngle = GlobalPosition.DirectionTo(GetGlobalMousePosition()).Angle();
		Rotation = Mathf.LerpAngle(Rotation, targetAngle, 0.5f);
		MoveAndSlide();
	}


	private void _on_hurtbox_on_hurt(DamageInfo damageInfo)
	{
		_healthController.ReduceHealth(damageInfo.Damage);
		var isStunned = _poise.Reduce(damageInfo.PoiseDamage);
		this.KnockbackInfo = new KnockbackInfo()
		{
			Direction = damageInfo.Source.GlobalPosition.DirectionTo(GlobalPosition),
			Distance = Mathf.Clamp(damageInfo.Damage, Constants.Tile.Size/ (isStunned ? 2 : 4), Constants.Tile.Sizex5),
			IsStunned = isStunned
		};
		
		if(StateManager.CurrentStateEnum == State.Attacking) 
			WeaponHandler.Cancel();
		
		StateManager.ChangeState(State.InKnockback);
	}

	private void _on_health_on_health_zero()
	{
		// QueueFree();
		// Hide();
		GetTree().ReloadCurrentScene();
	}

}
