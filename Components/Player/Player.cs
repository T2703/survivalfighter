using Godot;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

// This is the player class
// What the player does.
public partial class Player : CharacterBody2D
{
	// The speed of the player
	public float Speed = 690f;

	// The acceleration of the player
	public const float Accelereation = 1f;

	// Player health.
	public int Health = 100;

	// The player's sprite/image
	private Sprite2D sprite;

	// ref to inventory.
	private Inventory inventory;

	// Knockback variables
	private Vector2 knockbackVelocity = Vector2.Zero;
	private float knockbackDuration = 0.1f; 
	private float knockbackTimer = 0f; 

	private float attackCooldown = 0.4f;  
	private float cooldownTimer = 0f;

	public override void _Ready() {
		base._Ready();
		inventory = new Inventory();
		AddChild(inventory);
		sprite = GetNode<Sprite2D>("AssaultPlayer");
	}

	public override void _PhysicsProcess(double delta)
	{

		// Knockback logic
		if (knockbackTimer > 0)
		{
			knockbackTimer -= (float)delta;
			Velocity = knockbackVelocity; // Apply knockback velocity
			MoveAndSlide();
			return; // Skip player movement while knockback is active
		}

		// Velocity movement
		Vector2 velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		Vector2 direction = GetInput();

		// This handles the movement and when the player should stop.
		if (direction.Length() > 0) velocity = velocity.Lerp(direction.Normalized() * Speed, Accelereation);
		else velocity = Vector2.Zero;

		if (cooldownTimer > 0) cooldownTimer -= (float)delta;

		Velocity = velocity;
		MoveAndSlide();
		HandleSpriteFlip(direction);
	}

	// This method gets the input from the player
	private Vector2 GetInput() {
		// The player position.
		Vector2 input = Vector2.Zero;

		// Input actions.
		if (Input.IsActionPressed("game_right")) input.X += 1;
		if (Input.IsActionPressed("game_left")) input.X -= 1;
		if (Input.IsActionPressed("game_up")) input.Y -= 1;
		if (Input.IsActionPressed("game_down")) input.Y += 1;
		if (Input.IsActionJustPressed("switch")) inventory.SwtichWeapon();

		return input;
	}

	// Method to filp the sprite based on movement direction
	private void HandleSpriteFlip(Vector2 direction) {
		// This does flipping if moving right or left.
		if (direction.X < 0) sprite.Scale = new Vector2(-1, 1);
		else if (direction.X > 0) sprite.Scale = new Vector2(1, 1);
	}

	// For when the player takes damage.
	public virtual void TakeDamage(int damage) 
	{
		Health -= damage;
		GD.Print(Health);
        if (Health <= 0) QueueFree();
	}

	// For the knockback of the player.
	public void ApplyKnockback(Vector2 knockbackDirection, float knockbackStrength) 
	{
		knockbackVelocity = knockbackDirection * knockbackStrength;
		knockbackTimer = knockbackDuration; // Reset the knockback timer
		GD.Print("Player knocked back with velocity: ", knockbackVelocity);
	}


	// Converting code from GDScript to C# is not fun :(
}
