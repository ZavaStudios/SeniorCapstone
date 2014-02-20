using UnityEngine;
using System.Collections;

public class UnitPlayer : Unit {
	
	//Not sure how time is measured, but 30 seems to be good. 
//	private float gameOverDelay = 0f;

    WeaponModelSwitcher wepSwitcher;
	int wep = 0;
	
    private const float walkSpeedNormal = 10.0f;

	protected override void Start () 
	{
		setMaxSpeed(walkSpeedNormal);
        wepSwitcher = gameObject.GetComponentInChildren<WeaponModelSwitcher>();

		inventory = Inventory.getInstance();
		
		//Add the default weapons
		//TODO Instead of using the weapon types, use the names. Need some way to map between the names back to the types
        ItemEquipment myFirstPickaxe = new ItemWeapon(1, 1.0f, 0, 0, "Rusty Pickaxe", ItemWeapon.tWeaponType.WeaponPickaxe, "A slightly worn, but reliable pickaxe.");
        inventory.inventoryAddItem((ItemWeapon)myFirstPickaxe);

        //test of factory. Make a sword.
        ItemOre myOre = new ItemOre(ItemBase.tOreType.Steel);

		string bladeCode = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Normal, ItemBase.tOreType.Iron, ItemWeapon.tWeaponType.WeaponSword,
		                                                       ItemComponent.tComponentPart.Blade);
		string handleCode = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Normal, ItemBase.tOreType.Iron, ItemWeapon.tWeaponType.WeaponSword,
		                                                        ItemComponent.tComponentPart.Handle);

        ItemBase myBlade = ItemFactory.createComponent(bladeCode);
        ItemBase myHandle = ItemFactory.createComponent(handleCode);

        ItemWeapon myWeapon = ItemFactory.createWeapon((ItemComponent) myBlade, (ItemComponent) myHandle);

        inventory.inventoryAddItem(myWeapon);

		//Add some components to test making items
		string staffHandleCode = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Light, ItemBase.tOreType.Copper, ItemWeapon.tWeaponType.WeaponStaff,
		                                                             ItemComponent.tComponentPart.Handle);
		string staffBladeCode = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Light, ItemBase.tOreType.Copper, ItemWeapon.tWeaponType.WeaponStaff,
		                                                             ItemComponent.tComponentPart.Blade);

		inventory.inventoryAddItem (ItemFactory.createComponent (staffHandleCode)); //Add a staff handle
		inventory.inventoryAddItem (ItemFactory.createComponent (staffBladeCode)); //Add a staff blade

		base.Start();
        attackDamage = 20.0f;
        equipWeapon(ItemWeapon.tWeaponType.WeaponPickaxe.ToString());

		inventory.inventoryAddItem (ItemFactory.createComponent (bladeCode));
		inventory.inventoryAddItem (ItemFactory.createComponent (handleCode));

	}

	public void incrementScore()
	{
		Score++;
	}

	protected override void Update () 
	{
		if(InputContextManager.isATTACK()) //or some other button on OUYA
		{
//			print ("mouse clicked....");
			if (weapon != null)
				weapon.attack = true;
			else
				print ("You cannot attack without a weapon!");
		}
		if(InputContextManager.isSPECIAL_ATTACK())
		{
//			print ("right clicked...");
			
			if (weapon != null)
			{
				weapon.attackSpecial();
			}
		}
		
		const int numWeapons = 3;
		if(InputContextManager.isSWITCH_WEAPON())
		{
			if (wep > (numWeapons-1))
			{
				wep = 0;
			}
			
			print ("switch to wep: " + wep);
			
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
				case 2:
					equipWeapon ("WeaponStaff");
					wep++;
					break;
			}
		}
		
		if(InputContextManager.isSPRINT())
		{
			setMaxSpeed (walkSpeedNormal*1.6f);
		}
		else
		{
			setMaxSpeed (walkSpeedNormal);
		}

	}
	
	
	//Gets reference to the character motor class, then sets the move speed
	void setMaxSpeed(float newSpeed)
	{
        moveSpeed = newSpeed;
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
		
		//Wait for 5 seconds.
		StartCoroutine(wait(5));
		
		Application.LoadLevel(0);
	}
    
	//A function that is a wrapper to get a yield return from WaitForSeconds. 
	private IEnumerator wait(int seconds)
	{
		yield return new WaitForSeconds(seconds);
	}
    public override void playAttackAnimation()
    {
        wepSwitcher.playAnimation();
    }
	
	public override Vector3 getLookDirection()
	{
		return Camera.main.transform.forward;
	}
	
	public override Quaternion getLookRotation()
	{
		return Camera.main.transform.rotation;
	}
	
	public override Vector3 getEyePosition()
	{
		return Camera.main.transform.position;
	}
}
