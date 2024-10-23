using Godot;
using System;

public partial class DefaultBullet : Area2D
{
	// Speed of the bullet.
	public float Speed = 4500f;

	// Direction of the bullet.
	private Vector2 Direction;

	// This causes the bullet to disappear after 3 seconds.
	private float Lifespan = 3f;

	// The damage
	public int Damage { get; set; }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Bullet movement
		Position += Direction * Speed * (float)delta;
		Lifespan -= (float)delta;

		if (Lifespan <= 0) QueueFree();

	}

	public override void _Ready()
    {
		Connect("body_entered", new Callable(this, nameof(OnBulletBodyEntered)));
    }

	private void OnBulletBodyEntered(Node body)
	{
		if (body is BaseEnemy enemy)
		{
			enemy.TakeDamage(Damage);
			QueueFree();
		}
	}

	public void Fire(Vector2 direction) 
	{
		Direction = direction.Normalized();
	}
}
