using Godot;
using System;

public partial class N3B2 : BaseGun
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Damage = 6;
		FireRate = 0.2f;
		MagazinesReserves = 260;
		MagCap = 31;
		ReloadSpeed = 0.6f;
		GunScale = 1.7f;
		gunSprite = GetNode<Sprite2D>("N3b2");
		gunSprite.Texture = (Texture2D)GD.Load("res://Graphics/Weapons/Guns/N3B2.png");
		base._Ready();
	}

}
