using Godot;
using System;

public partial class NineMM : BaseGun
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Damage = 3;
		FireRate = 0.3f;
		MagazinesReserves = 69;
		MagCap = 9;
		ReloadSpeed = 0.4f;
		GunScale = 0.8f;
		gunSprite = GetNode<Sprite2D>("9Mm");
		gunSprite.Texture = (Texture2D)GD.Load("res://Graphics/Weapons/Guns/9mm.png");
		base._Ready();
	}
	
	protected override void GetInput() 
	{
		if (Input.IsActionJustPressed("mouse1")) Shoot();
        if (Input.IsActionJustPressed("game_r") && CurrentMagazine < MagCap && CoolDownTimerReload <= 0 && CoolDownTimer <= 0) Reload();
	}
	
}
