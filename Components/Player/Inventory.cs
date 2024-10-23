using Godot;
using System;
using System.Collections.Generic;

public partial class Inventory : Node
{
	// Limit to 2 guns.
	private const int MaxWeapons = 2;

	// List for carrying weapons
	private List<BaseGun> Weapons = new List<BaseGun>();

	// Track the current weapoon index.
	private int CurrentWeaponIndex = 0;

	// The current weapon of the player
	private BaseGun CurrentWeapon;

    // Timer for switching weapon
    private float CoolDownTimer = 0f;

	// How many seconds does it take.
	private const float TimeToSwitch = 0.2f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BaseGun N3B2 = (BaseGun)GD.Load<PackedScene>("res://Components/Weapons/Guns/n_3b_2.tscn").Instantiate();
		BaseGun NineMM = (BaseGun)GD.Load<PackedScene>("res://Components/Weapons/Guns/9_mm.tscn").Instantiate();

		AddWeapon(N3B2);
        AddWeapon(NineMM);

		// Add the weapons to the player node tree but keep them hidden
        foreach (var weapon in Weapons)
        {
            weapon.Visible = false; // Keep all weapons invisible initially
            GetParent().AddChild(weapon); // Add to player's children
        }

		EquipWeapon(0);
	}

    public override void _Process(double delta)
    {
        if (CoolDownTimer > 0) CoolDownTimer -= (float)delta;
    }

    // Add a weapon to the inventory
    public bool AddWeapon(BaseGun weapon) 
	{
		if (Weapons.Count < MaxWeapons) 
		{
			Weapons.Add(weapon);
			return true;
		}
		else 
		{
			// Replace current weapon with a new one.
			Weapons[CurrentWeaponIndex] = weapon;
			EquipWeapon(CurrentWeaponIndex);
			return true;
		}
	}

	// Equips the weapon.
	public void EquipWeapon(int index) 
	{
		if (index >= 0 && index < Weapons.Count)
		{
			 // Remove the currently equipped weapon
			if (CurrentWeapon != null) 
			{
				CurrentWeapon.IsActive = false;
				CurrentWeapon.Visible = false;
			}
		}

		CurrentWeapon = Weapons[index];
		CurrentWeaponIndex = index;
		CurrentWeapon.Visible = true;   
		CurrentWeapon.IsActive = true;
		//CurrentWeapon.InstantReload(); 
	}

	// Switches the weapon in the inventory.
	public void SwtichWeapon()
	{
		if (CoolDownTimer > 0) 
		{
			return; 
		}
		
		int nextWeaponIndex = (CurrentWeaponIndex + 1) % Weapons.Count;
		EquipWeapon(nextWeaponIndex); 
		CoolDownTimer = TimeToSwitch;
	}

	// Gets the current weapon.
	public BaseGun GetCurrentWeapon()
    {
        return CurrentWeapon;
    }
}
