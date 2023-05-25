using Godot;
using System;
using Shuut.Scripts.Hitbox;
using Shuut.Scripts.Hurtbox;

public partial class Bullet : Node2D
{
    [Export] private float Speed = 1000;
    [Export] private Hitbox _hitbox;
    private Vector2 direction;
    public int damage;

    public override void _Ready()
    {
        base._Ready();
        _hitbox.OnHitboxHit += HitboxOnOnHitboxHit;
    }

    private void HitboxOnOnHitboxHit(Hurtbox hurtbox)
    {
        hurtbox.Hurt(new DamageInfo() {Damage =  damage, Source = this});
        QueueFree();
    }

    public void Setup(Vector2 position, Vector2 direction,uint mask, int damage)
    {
        _hitbox.CollisionMask = mask;
        GlobalPosition = position;
        this.direction = direction;
        this.damage = damage;
        GlobalRotation = this.direction.Angle();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        GlobalPosition += direction * Speed * (float)delta;
    }
}
