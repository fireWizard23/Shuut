using Godot;
using Shuut.Player.States;
using Shuut;
using Shuut.Scripts;
using Shuut.Scripts.Hurtbox;
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
	Dashing,
}

public partial class Player : StatefulEntity<State, Player>, IAttacker
{
	[Export]
	public float Speed = 100.0f;
	[Export] private HealthController _healthController;
	[Export] public WeaponHandler _weaponHandler;
	[Export(PropertyHint.Layers2DPhysics)] public uint AttackMask { get; set;}
	[Export] public Label Label;


	public float DashLength = Constants.Tile.Size;
	public KnockbackInfo KnockbackInfo;
	public Vector2 InputDirection;

    public InputBuffer inputBuffer = new() { TimeMs = 500 };
    public bool InputConsumed = false;

    protected override void BeforeReady()
	{
		StateManager = new(
			new()
			{
				{ State.Normal,  new NormalState() },
				{ State.Attacking,  new AttackingState() },
				{ State.InKnockback,  new InKnockbackState() },
				{ State.Dashing,  new DashingState() },
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
		    if (Input.IsActionJustPressed("dash"))
		    {
			    inputBuffer.Use("dash");
		    }

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
		if (StateManager.CurrentStateEnum is not (State.InKnockback or  State.Dashing))
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
			if (!_weaponHandler.OwnerCanMove)
			{
				Velocity *= 0;
			}
		}
		MoveAndSlide();
	}

	private void _on_hurtbox_on_hurt(DamageInfo damageInfo)
	{
		_healthController.ReduceHealth(damageInfo.Damage);
		KnockbackInfo = new()
		{
			Direction = damageInfo.Source.GlobalPosition.DirectionTo(GlobalPosition),
			Distance = Mathf.Clamp(damageInfo.Damage, Constants.Tile.Size/2, Constants.Tile.Sizex5)
		};
		StateManager.ChangeState(State.InKnockback);
		damageInfo.Dispose();
	}

	private void _on_health_on_health_zero()
	{
		QueueFree();	
	}

}
