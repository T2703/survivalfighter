using Godot;
using System;
using Vector2 = Godot.Vector2; // I think my code is gas lighting me or something.

public partial class B451csmg : Node2D
{
	// Damage of the gun
	public int Damage = 10;

	// Amount of magazines in reserves.
	public int MagazinesReserves = 360;

	// Magazine Capcity
	public int MagCap = 25;

	// Track the bullets in the loaded magazine.
	public int CurrentMagazine = 0;

	// The cooldown timer for the firerate.
	private float CooolDownTimer = 0f;

	// Well the fire rate duh
	public float FireRate = 0.2f;

	// The cooldown timer for the reload.
	private float CooolDownTimerReload = 0f;

	// Speed of reload.
	public float ReloadSpeed = 0.5f;

	// Do we have ammo
	public bool HaveAmmo = false;

	// Can the player shoot
	public bool CanShoot = false;

	// The mouse's position.
	private Vector2 MousePos;

	// This gets how much shots has been used.
	private int MagCapFromLast = 0;

    public override void _Ready()
    {
    	InstantReload();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    // This is just unity update. lol
    public override void _Process(double delta)
	{
		// This makes the weapon follow
		MousePos = GetGlobalMousePosition();
		if (GetParent() is Player player) Position = new Vector2(-10, 0);
		AimAtMouse();
		GetInput();

		// Decrease it over time.
		if (CooolDownTimer > 0) CooolDownTimer -= (float)delta;
		if (CooolDownTimerReload > 0) CooolDownTimerReload -= (float)delta;

	}

	// The aiming for the gun.
	private void AimAtMouse() 
	{
		// Calculate the direction vector from weapon to mouse.
		Vector2 direction = MousePos - GlobalPosition;
		
		float angle = direction.Angle();

		Rotation = angle;
	}

	// Inputs from the player for the gun.
	private void GetInput() 
	{
		// Shoot the gun.
		if (Input.IsActionPressed("mouse1")) Shoot();
		if (Input.IsActionJustPressed("game_r") && CurrentMagazine < MagCap && CooolDownTimerReload <= 0 && CooolDownTimer <= 0) Reload();
	}

	// This controls the reload.
	private void Reload() 
	{
		// If we have any mags left reload else just no ammo.
		if (MagazinesReserves > 0 && CooolDownTimerReload <= 0 && CurrentMagazine < MagCap)
		{
			GD.Print("Reloading...");
			CooolDownTimerReload = ReloadSpeed;
			HaveAmmo = true;
			CanShoot = true;
			MagazinesReserves -= MagCapFromLast;
			CurrentMagazine = MagCap;
			MagCapFromLast = 0;
		}
		else if (MagazinesReserves <= 0)
		{
			HaveAmmo = false;
		}
	}	

	// Instant reload when player spawns (this should only be called on ready and nowhere else.)
	private void InstantReload()
	{
		// Reset ammo and magazine without triggering the reload timer
		HaveAmmo = true;
		CanShoot = true;
		MagazinesReserves -= MagCapFromLast;
		CurrentMagazine = MagCap;
		MagCapFromLast = 0;
		GD.Print("Instant reload on spawn");
	}

	// Shooting the gun.
	private void Shoot() 
	{
		if (CooolDownTimerReload > 0) 
		{
			GD.Print("Can't shoot, RELOADING");
			return;
		}

		if (CurrentMagazine > 0 && CooolDownTimer <= 0) 
		{
			GD.Print(CurrentMagazine);
			CurrentMagazine -= 1;
			// Load the bullet scene
			PackedScene bulletScene = (PackedScene)ResourceLoader.Load("res://Components/Weapons/Bullets/default_bullet.tscn");

			if (bulletScene != null)
			{
				// Create the bullet instance
				DefaultBullet bullet = bulletScene.Instantiate<DefaultBullet>();

				// Set the bullet's position to the gun's position (or adjust as needed)
				bullet.Position = GlobalPosition;

				// Calculate direction towards the mouse
				Vector2 direction = (GetGlobalMousePosition() - GlobalPosition).Normalized();

				// Fire the bullet in the direction
				bullet.Fire(direction);

				// Add the bullet to the scene tree
				GetTree().Root.AddChild(bullet);
				MagCapFromLast++;
				CooolDownTimer = FireRate;

			}
		}
		else if (CurrentMagazine < 1) Reload();
	}
}