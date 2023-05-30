using Godot;
using Shuut.Player.States;
using Shuut.Scripts;
using Shuut.Scripts.Poise;
using Shuut.World;
using Shuut.World.Weapons;
using Shuut.World.Zombies;
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
	public float Speed => GivenStats.MovementSpeed;
	public Poise Poise;


	public float DashLength = Constants.Tile.Size;
	public KnockbackInfo KnockbackInfo;
	public Vector2 InputDirection;

    public InputBuffer inputBuffer = new() { TimeMs = 500 };
    public bool InputConsumed = false;

    protected override void BeforeReady()
	{
		Poise.Setup(GivenStats.Poise);
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
	    InputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");

	    if (!InputConsumed)
	    {

		    if (Input.IsActionJustPressed("attack"))
		    {
			    inputBuffer.Use("attack");
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
			
			if (InputDirection != Vector2.Zero)
			{
				velocity = InputDirection.Normalized() * Speed;

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
		Rotation = (float)Mathf.LerpAngle(Rotation, targetAngle, 0.5f);
		MoveAndSlide();
	}


	private void _on_hurtbox_on_hurt(DamageInfo damageInfo)
	{
		_healthController.ReduceHealth(damageInfo.Damage);
		KnockbackInfo = new()
		{
			Direction = damageInfo.Source.GlobalPosition.DirectionTo(GlobalPosition),
			Distance = Mathf.Clamp(damageInfo.Damage, Constants.Tile.Size/2, Constants.Tile.Sizex5),
			IsStunned = Poise.Reduce(damageInfo.Damage)
		};
		if(StateManager.CurrentStateEnum == State.Attacking) 
			WeaponHandler.Cancel();
		if(inputBuffer is {InputUsed: "dash"}) 
			inputBuffer.Reset();
		
		StateManager.ChangeState(State.InKnockback);
	}

	private void _on_health_on_health_zero()
	{
		// QueueFree();
		// Hide();
		GetTree().ReloadCurrentScene();
	}

}
