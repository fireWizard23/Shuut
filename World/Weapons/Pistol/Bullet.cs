using Godot;
using System;
using System.Data.Common;
using Shuut.Player;
using Shuut.Scripts.Hitbox;
using Shuut.Scripts.Hurtbox;

public partial class Bullet : Node2D
{
    [Export(PropertyHint.Layers2DPhysics)] private uint colliderMask;
    [Export] private float Speed = 1000;
    [Export] private Hitbox _hitbox;
    private Vector2 direction;
    public int damage;
    private PhysicsDirectSpaceState2D space;
    private IDamager damager;

    public override void _Ready()
    {
        base._Ready();
        _hitbox.OnHitboxHit += HitboxOnOnHitboxHit;
        this.space = GetWorld2D().DirectSpaceState;
    }

    private void HitboxOnOnHitboxHit(Hurtbox hurtbox)
    {
        hurtbox.Hurt(new DamageInfo() {Damage =  damage, Source = damager});
        QueueFree();
    }

    public void Setup(Vector2 position, Vector2 direction,uint mask, int damage, IDamager damager)
    {
        _hitbox.CollisionMask = mask;
        GlobalPosition = position;
        this.direction = direction;
        this.damage = damage;
        GlobalRotation = this.direction.Angle();
        this.damager = damager;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        GlobalPosition += direction * Speed * (float)delta;

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
