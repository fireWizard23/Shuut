using System;
using System.Linq;
using Godot;
using Godot.Collections;
using Shuut.Player;
using Shuut.Scripts;
using Shuut.World.Weapons;
using Shuut.World.Zombies.States;
using DamageInfo = Shuut.Scripts.Hurtbox.DamageInfo;

namespace Shuut.World.Zombies;


using StateManager = StateManager<State, ZombieController>;

public enum State
{
	Idle,
	Chasing,
	Wandering,
	Attacking,
	InKnockback,
	EnemyDetected,
}


public partial class ZombieController : StatefulEntity<State, ZombieController>, IAttacker, IDamager
{

	[Export] public float MovementSpeed { get; private set; } = 100;
	
	[Export] public Line2D PathLine2D;
	[Export] public Area2D Detector { get; private set; }
	[Export] public Label StateLabel;
	[Export] public HealthController HealthController;
	[Export] public WeaponHandler WeaponHandler;
	[Export] public Label DetectionCue;
	[Export] public int BaseDamage { get; set; } = 1;
	
	[Export(PropertyHint.Layers2DPhysics)]  public uint AttackMask { get; set; }

	
	[Export(PropertyHint.Layers2DPhysics)] private uint _entitySteerAwayLayer;
	
	
	public Vector2 SpawnPosition { get; private set; }
	public Node2D Target { get; set; }
	public RandomNumberGenerator Rng = new();
	public KnockbackInfo KnockbackInfo { get; set; }
	
	public Vector2 DesiredVelocity;

	private Array<Rid> _exclude;
	private Node2D _potentialTarget;


	protected override void BeforeReady()
	{
		_exclude = new(){ GetRid()};
		SpawnPosition = GlobalPosition;
		Rng.Randomize();
		
		StateManager = new(
			new()
			{
				{ State.Idle, new IdleState() },
				{ State.Wandering , new WanderingState()},
				{ State.Attacking , new AttackingState()},
				{ State.Chasing , new ChasingState()},
				{ State.InKnockback, new KnockbackState()},
				{ State.EnemyDetected, new EnemyDetected()},
				
			},
			this
		);

	}


	public override void _Process(double delta)
	{
		base._Process(delta);
		QueueRedraw();
	}


	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		var ac = StateManager.CurrentStateEnum is State.Attacking or State.InKnockback
			? Vector2.Zero
			: ContextSteer(DesiredVelocity.Normalized(), _entitySteerAwayLayer);

		if (StateManager.CurrentStateEnum is not State.InKnockback)
		{
			var desiredDirection = DesiredVelocity.Normalized();
			Velocity = (desiredDirection + ac).Normalized() * MovementSpeed;
		}
		
		MoveAndSlide();
		StateLabel.Text = StateManager.CurrentStateEnum.ToString();
		StateLabel.Rotation = -Rotation;
		StateLabel.Position = Vector2.Zero;
														
		DetectionCue.Rotation = -Rotation;
		StateLabel.Position = Vector2.Up * 10;


		void DeterminePotentialTarget()
		{
			var distance = GlobalPosition.DistanceTo(_potentialTarget.GlobalPosition);
			switch (distance)
			{
				case >= Constants.Tile.Sizex5:
					DetectionCue.Text = string.Empty;
					_potentialTarget = null;
					return;
				case < Constants.Tile.Size * 1.5f:
					Target = _potentialTarget;
					_potentialTarget = null;
					ChangeState(State.EnemyDetected);
					return;
			}

			DetectionCue.Text = "?";
			var dir = GlobalPosition.DirectionTo(_potentialTarget.GlobalPosition);
			var dot = Vector2.Right.Rotated(GlobalRotation).Dot(dir);
			
			if (!(dot > 0.4f)) return;
			
			Target = _potentialTarget;
			_potentialTarget = null;
			ChangeState(State.EnemyDetected);

		}

		if (_potentialTarget != null)
		{
			DeterminePotentialTarget();
		}
		
		
		//Rotation
		if (StateManager.CurrentStateEnum is State.InKnockback) return;
		
		var targetAngle = Velocity.Normalized().Angle();
		if (Velocity.LengthSquared() > 0)
		{
			GlobalRotation = (float)Mathf.LerpAngle(GlobalRotation, targetAngle, 8 * delta);
		}

		
	}

	Vector2 ContextSteer(Vector2 desiredDirection, uint collisionLayer,int rayCount=8,int rayLength=100)
	{
		var directions = new Vector2[rayCount].Select((v, i) => Vector2.Right.Rotated(2 * i * Mathf.Pi / rayCount)).ToArray();
		var dangers = new float[rayCount];
		var interestWeights = new float[rayCount];
		var space = GetWorld2D().DirectSpaceState;

		for (int i = 0; i < rayCount; i++)
		{
			var direction = directions[i];
			interestWeights[i] = Mathf.Max(direction.Dot(desiredDirection),0);
		}
		
		
		for (var i = 0; i < rayCount; i++)
		{
			var direction = directions[i];
			var query = new PhysicsRayQueryParameters2D()
			{
				Exclude = _exclude,
				From = GlobalPosition,
				To = GlobalPosition + direction * rayLength,
				CollisionMask = collisionLayer,

			};
			var hit = space.IntersectRay(query);
			if (hit.Count <= 0) continue;
			
			var dire = GlobalPosition.DirectionTo(hit["position"].AsVector2());
			dangers[i] = 1 - (dire.Length() / rayLength);

			if (hit["collider"].As<Node2D>() is ZombieController z)
			{
				dangers[i] = 0.8f + (GlobalPosition.DistanceTo(hit["position"].AsVector2()) / rayLength) * 0.5f;
			}

		}

		var go = Vector2.Zero;
		for (var i = 0; i < rayCount; i++)
		{
			var direction = directions[i];
			go +=( interestWeights[i] - dangers[i]) * direction;
		}
		

		return go;

	}

	public void Destroy()
	{
		StateManager.Destroy();
		QueueFree();
	}


	private void _on_health_on_health_zero()
	{
		Destroy();
	}

	private void _on_detector_body_entered(Node2D body)
	{
		if (Target != null || _potentialTarget != null)
		{
			return;
		}
		
		_potentialTarget = body;
	}


	private void _on_hurtbox_on_hurt(DamageInfo damageInfo)
	{
		HealthController.ReduceHealth(damageInfo.Damage);
		this.KnockbackInfo = new KnockbackInfo()
		{
			Direction = damageInfo.Source.GlobalPosition.DirectionTo(GlobalPosition),
			Distance = Mathf.Clamp(damageInfo.Damage, Constants.Tile.Size/2, Constants.Tile.Sizex5)
		}; 
		ChangeState(State.InKnockback);
		
		Target ??= (Node2D)damageInfo.Source;
		damageInfo.Dispose();
	}

}
