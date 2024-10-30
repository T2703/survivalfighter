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
	
	// Cooldowns
	private float attackCooldown = 0.4f;  
	private float cooldownTimer = 0f;
    private float CoolDownTimerHealing = 0f;
	private const float healingCooldownDuration = 25f;

	// Timer for the healing 
	private Timer timeToHeal;

	public override void _Ready() {
		base._Ready();

		inventory = new Inventory();
		AddChild(inventory);

		timeToHeal = new Timer();
		AddChild(timeToHeal);
		timeToHeal.OneShot = true; // Ensure it only triggers once per heal
    	timeToHeal.Timeout += OnHealTimeout;

		sprite = GetNode<Sprite2D>("AssaultPlayer");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Velocity movement
		Vector2 velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		Vector2 direction = GetInput();

		// This handles the movement and when the player should stop.
		if (direction.Length() > 0) velocity = velocity.Lerp(direction.Normalized() * Speed, Accelereation);
		else velocity = Vector2.Zero;

		if (cooldownTimer > 0) cooldownTimer -= (float)delta;
		if (CoolDownTimerHealing > 0) CoolDownTimerHealing -= (float)delta;

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
		if (Input.IsActionPressed("heal") && CoolDownTimerHealing <= 0 && Health < 100) HealPlayer();

		return input;
	}

	// Method to filp the sprite based on movement direction
	private void HandleSpriteFlip(Vector2 direction) {
		// This does flipping if moving right or left.
		if (direction.X < 0) sprite.Scale = new Vector2(-1, 1);
		else if (direction.X > 0) sprite.Scale = new Vector2(1, 1);
	}

	// Heals the player
	private void HealPlayer() 
	{
		// Unequip current weapon before healing.
		if (inventory.GetCurrentWeapon() != null) 
		{
			inventory.GetCurrentWeapon().Visible = false;
			inventory.GetCurrentWeapon().IsActive = false;
		}

		// Determines between how much to get healed so the player does not go over cap.
		int newHealth = Mathf.Min(Health + 20, 100); 
		Health = newHealth;
		CoolDownTimerHealing = healingCooldownDuration;
		GD.Print(CoolDownTimerHealing);
	}

	// This is called when the healing timer finishes
	private void OnHealTimeout()
	{
		// Re-equip current weapon
		if (inventory.GetCurrentWeapon() != null)
		{
			inventory.GetCurrentWeapon().Visible = true;
			inventory.GetCurrentWeapon().IsActive = true;
		}
	}

	// For when the player takes damage.
	public virtual void TakeDamage(int damage) 
	{
		Health -= damage;
		GD.Print(Health);
        if (Health <= 0) QueueFree();
	}


	// Converting code from GDScript to C# is not fun :(
}
