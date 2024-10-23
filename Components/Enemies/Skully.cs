using Godot;
using System;

public partial class Skully : BaseEnemy
{
	// Called when the node enters the scene tree for the first time.
	
	private Random random; 
	public override void _Ready()
	{
		base._Ready();
		random = new Random();
		Health = random.Next(10, 20);
		Speed = 120f;
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
		Damage = random.Next(2, 4);
    }
}
