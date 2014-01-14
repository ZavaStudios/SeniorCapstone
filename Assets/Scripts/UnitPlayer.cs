using UnityEngine;
using System.Collections;

public class UnitPlayer : Unit {
	
	//Not sure how time is measured, but 30 seems to be good. 
	private float delay = 0f;
	WeaponModelSwitcher wepSwitcher;
	int wep = 0;
	
	protected override void Start () 
	{
		setMaxSpeed();
        wepSwitcher = gameObject.GetComponentInChildren<WeaponModelSwitcher>();

		inventory = new Inventory();
		
		//Add the default weapons
		//TODO Instead of using the weapon types, use the names. Need some way to map between the names back to the types
		inventory.inventoryAddWeapon (WeaponPickaxe.strWeaponType);
		inventory.inventoryAddWeapon (WeaponSword.strWeaponType);

		base.Start();
                 
        equipWeapon("WeaponPickaxe");
	}

	public void incrementScore()
	{
		Score++;
	}

	protected override void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Mouse0)) //or some other button on OUYA
		{
			print ("mouse clicked....");
			if (weapon != null)
				weapon.attack = true;
			else
				print ("You cannot attack without a weapon!");
		}
		
		if(Input.GetKeyDown (KeyCode.Q))
		{
			switch (wep)
			{
				case 0:
					equipWeapon("WeaponPickaxe");
					wep++;
					break;
				case 1:
					equipWeapon("WeaponSword");
					wep++;
					break;
			}
			if (wep > 1)
			{
				wep = 0;
			}
			
		}
		
		if(Input.GetKeyDown (KeyCode.LeftShift) || Input.GetKeyDown (KeyCode.RightShift))
		{
			moveSpeed = 20.0f;
			setMaxSpeed ();
		}
		else if(Input.GetKeyUp (KeyCode.LeftShift) || Input.GetKeyUp (KeyCode.RightShift))
		{
			moveSpeed = 10.0f;
			setMaxSpeed ();
		}

	}
	
	
	//Gets reference to the character motor class, then sets the move speed
	void setMaxSpeed()
	{
		CharacterMotor m = gameObject.GetComponent<CharacterMotor>();
		m.movement.maxForwardSpeed = moveSpeed;
		m.movement.maxSidewaysSpeed = moveSpeed;
		m.movement.maxBackwardsSpeed = moveSpeed;
	}
    
	public override void equipWeapon(string newWeapon)
    {
        base.equipWeapon(newWeapon);
        wepSwitcher.SwitchWeapon(newWeapon);
    }   
        
	protected override void killUnit ()
	{
		print ("How did you die...???");
		
		//Draws the gameover GUI to the screen. 
		GameOver.gameOver = true;
		
		//Set the length of time to display the game over tag.
		if(delay == 0)
		{
			delay = Time.time + 5;
		}
		
		//Restart the application when the game is over.
		if(Time.time >= delay)
		{
			Application.LoadLevel(0);
		}
	}
    
    public override void playAttackAnimation()
    {
        wepSwitcher.playAnimation();
    }
	
	public override Vector3 getLookDirection()
	{
		return Camera.main.transform.forward;
	}
	
	public override Vector3 getEyePosition()
	{
		return Camera.main.transform.position;
	}
}
