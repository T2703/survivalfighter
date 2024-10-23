using Godot;
using System;

public partial class BaseEnemy : CharacterBody2D 
{
	// Basic enemy properties
	public int Health { get; set; }
    public float Speed { get; set; }
	public int Damage { get; set; }
	public float KnockbackStrength = 100f;

	// Attack Cooldown
	private float attackCooldown = 0.4f;  
	private float cooldownTimer = 0f;

	// Ref to the player node.
	private Node2D player;

	// Track whether the player is in the enemy's range
	private bool playerInRange = false;

	 // Reference to Area2D
    private Area2D detectionArea;

    public override void _Ready()
    {
		player = GetParent().GetNode<Node2D>("Player");
		detectionArea = GetNode<Area2D>("DetectionArea");
		detectionArea.Connect("body_entered", new Callable(this, nameof(OnEnemyBodyEntered)));
		detectionArea.Connect("body_exited", new Callable(this, nameof(OnEnemyBodyExited)));
		GD.Print(player);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (cooldownTimer > 0) cooldownTimer -= (float)delta;
        if (player != null) MoveTowardsPlayer((float)delta);

        // Continuously check if the player is within range and cooldown is over
        if (playerInRange && cooldownTimer <= 0)
        {
            player.Call("TakeDamage", Damage);
            cooldownTimer = attackCooldown;
			ApplyKnockBack();
        }
    }

	// Moving towards the player
	private void MoveTowardsPlayer(float delta) 
	{
		Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
		Velocity = direction * Speed;
        MoveAndSlide();
	}

    // When the enemy takes damage.
    public virtual void TakeDamage(int damage) 
	{
		Health -= damage;
        if (Health <= 0) QueueFree();
	}

	// When the player enters the enemy's range
	private void OnEnemyBodyEntered(Node body)
	{
		
		if (body is Player)
		{
			playerInRange = true;  
		}
		else if (body is DefaultBullet bullet)
        {
            TakeDamage(bullet.Damage);
            bullet.QueueFree(); // Remove bullet after hit
            GD.Print("Bullet hit enemy");
        }
	}

	// When the player leaves the enemy's range
	private void OnEnemyBodyExited(Node body)
	{
		if (body is Player)
		{
			playerInRange = false;  
		}
	}

	// Knockback for the player.
	private void ApplyKnockBack() 
	{
		if (player is Player playerBody)
		{
			// Calculate knockback direction (from enemy to player)
			Vector2 knockbackDirection = (playerBody.GlobalPosition - GlobalPosition).Normalized();
			
			// Apply impulse/knockback force to the player
			playerBody.ApplyKnockback(knockbackDirection, KnockbackStrength);

			GD.Print("Player knocked back");
		}
	}
}