using Godot;
using Shuut.Player;
using Shuut.Scripts;
using Shuut.Scripts.Hitbox;
using Shuut.Scripts.Hurtbox;

namespace Shuut.World.Weapons.Pistol;

public partial class Bullet : Node2D
{
    [Export(PropertyHint.Layers2DPhysics)] private uint colliderMask;
    [Export] private float Speed = 1000;
    [Export] private Hitbox _hitbox;
    private Vector2 direction;
    public int damage;
    private PhysicsDirectSpaceState2D space;
    private IDamager damager;
    private Vector2 _distanceTravelled;
    private RangedWeaponInfo _weaponInfo;


    public override void _Ready()
    {
        base._Ready();
        _hitbox.OnHitboxHit += HitboxOnOnHitboxHit;
        this.space = GetWorld2D().DirectSpaceState;
    }

    private void HitboxOnOnHitboxHit(Hurtbox hurtbox)
    {
        hurtbox.Hurt(new DamageInfo() {Damage =  damage, Source = damager, PoiseDamage = _weaponInfo.PoiseDamage});
        QueueFree();
    }

    public void Setup(Vector2 position, Vector2 direction, uint mask, int damage, IDamager damager, RangedWeaponInfo weaponInfo)
    {
        _hitbox.CollisionMask = mask;
        GlobalPosition = position;
        this.direction = direction;
        this.damage = damage;
        GlobalRotation = this.direction.Angle();
        this.damager = damager;

        _weaponInfo = weaponInfo;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var positionToAdd = direction * Speed * (float)delta;
        GlobalPosition += positionToAdd;
        _distanceTravelled += positionToAdd;

        if (_distanceTravelled.Length() >= _weaponInfo.ShootRange)
        {
            QueueFree();
            return;
        }
        
        
        var query = new PhysicsShapeQueryParameters2D()
        {
            Shape = _hitbox.CollisionShape2D.Shape,
            CollisionMask = _hitbox.CollisionMask & colliderMask,
            Transform = new Transform2D()
            {
                Origin = GlobalPosition
            }
        };

        var res = space.IntersectShape(query);
        if (res.Count > 0)
        {
            QueueFree();
        }

    }
}