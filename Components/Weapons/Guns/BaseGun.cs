using Godot;
using System;

public partial class BaseGun : Node2D
{
    // Common gun properties
    public int Damage { get; protected set; }
    public int MagazinesReserves { get; protected set; }
    public int MagCap { get; protected set; } 
    public int CurrentMagazine { get; protected set; } = 0;
    public float FireRate { get; protected set; } 
    public float ReloadSpeed { get; protected set; }
    public float GunScale { get; protected set; }  

    // Timers for cooldowns
    public float CoolDownTimer = 0f;
    public float CoolDownTimerReload = 0f;

	// Boolean for checking ammo and can they shoot.
    public bool HaveAmmo { get; protected set; } = false;
    public bool CanShoot { get; protected set; } = false;

    // Another boolean for active.
    public bool IsActive { get; set; } = false;

	// Mouse Position.
    private Vector2 MousePos;

	// How much bullets should we subtract
    private int MagCapFromLast = 0;

    // ref for all guns.
    protected Sprite2D gunSprite;

    public override void _Ready()
    {
        InstantReload();
    }

    public override void _Process(double delta)
    {
        if (!IsActive) return;
        
        MousePos = GetGlobalMousePosition();
        if (GetParent() is Player player) Position = new Vector2(-10, 0);

        AimAtMouse();
        GetInput();

        // Decrease cooldowns over time
        if (CoolDownTimer > 0) CoolDownTimer -= (float)delta;
        if (CoolDownTimerReload > 0) CoolDownTimerReload -= (float)delta;
    }

	// This gets the gun to aim at the mouse.
    protected void AimAtMouse()
    {
        Vector2 direction = MousePos - GlobalPosition;
        float angle = direction.Angle();
        Rotation = angle;
        HandleSpriteFlip(direction);
    }

	// Get the player input for gun controls.
    protected virtual void GetInput()
    {
        if (Input.IsActionPressed("mouse1")) Shoot();
        if (Input.IsActionJustPressed("game_r") && CurrentMagazine < MagCap && CoolDownTimerReload <= 0 && CoolDownTimer <= 0) Reload();
    }

	// Shooting.
    protected virtual void Shoot()
    {
		if (CoolDownTimerReload > 0) 
		{
			return;
		}

		if (CurrentMagazine > 0 && CoolDownTimerReload <= 0) 
		{
			GD.Print(CurrentMagazine);
			CurrentMagazine -= 1;
			CoolDownTimerReload = FireRate;
			SpawnBullet();
		}
		else if (CurrentMagazine < 1) 
		{
			Reload();
		}
    }

    private void HandleSpriteFlip(Vector2 direction)
    {
        if (gunSprite != null) 
        {
            if (direction.X < 0) gunSprite.Scale = new Vector2(GunScale, -GunScale); 
            if (direction.X > 0) gunSprite.Scale = new Vector2(GunScale, GunScale); 
        }
    }

	// This is where the bullet spawn logic happens.
	protected virtual void SpawnBullet()
    {
		PackedScene bulletScene = (PackedScene)ResourceLoader.Load("res://Components/Weapons/Bullets/default_bullet.tscn");

		if (bulletScene != null)
		{
			// Create the bullet instance
			DefaultBullet bullet = bulletScene.Instantiate<DefaultBullet>();

			// Set the bullet's position to the gun's position (or adjust as needed)
			bullet.Position = GlobalPosition;

			// Calculate direction towards the mouse
			Vector2 direction = (GetGlobalMousePosition() - GlobalPosition).Normalized();

            // Set the bullet damage from the gun used.
            bullet.Damage = this.Damage;
            
			// Fire the bullet in the direction
			bullet.Fire(direction);

			// Add the bullet to the scene tree
			GetTree().Root.AddChild(bullet);
			MagCapFromLast++;
			CoolDownTimerReload = FireRate;

		}
    }

	// Reloading
    protected virtual void Reload()
    {
        if (MagazinesReserves > 0 && CoolDownTimerReload <= 0 && CurrentMagazine < MagCap)
        {
            GD.Print("Reloading...");
            CoolDownTimerReload = ReloadSpeed;
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

        GD.Print(MagazinesReserves);
    }

	// Reload method that should be used in ready.
    protected virtual void InstantReload()
    {
        HaveAmmo = true;
        CanShoot = true;
        MagazinesReserves -= MagCapFromLast;
        CurrentMagazine = MagCap;
        MagCapFromLast = 0;
        GD.Print("Instant reload on spawn");
    }

}
