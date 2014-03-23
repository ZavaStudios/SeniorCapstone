using UnityEngine;
using System.Collections;

public class UnitPlayer : Unit {
	
    WeaponModelSwitcher wepSwitcher;
	int wep = 1;
	
    private const float walkSpeedNormal = 10.0f;

	protected override void Start () 
	{
		setMaxSpeed(walkSpeedNormal);
        wepSwitcher = gameObject.GetComponentInChildren<WeaponModelSwitcher>();

		inventory = Inventory.getInstance();
		
		//Add the default weapons
		//TODO Instead of using the weapon types, use the names. Need some way to map between the names back to the types
        ItemEquipment myFirstPickaxe = new ItemWeapon(1, 1.0f, 0, 0, 0.0f, "Rusty Pickaxe", ItemWeapon.tWeaponType.WeaponPickaxe, "A slightly worn, but reliable pickaxe.");
        
        inventory.inventoryAddItem((ItemWeapon)myFirstPickaxe);
        
		string bladeCode = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Normal, ItemBase.tOreType.Ethereal, ItemWeapon.tWeaponType.WeaponSword,
		                                                       ItemComponent.tComponentPart.Blade);
		string handleCode = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Normal, ItemBase.tOreType.Ethereal, ItemWeapon.tWeaponType.WeaponSword,
		                                                        ItemComponent.tComponentPart.Handle);

		string bladeCode2 = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Light, ItemBase.tOreType.Ethereal, ItemWeapon.tWeaponType.WeaponSword,
		                                                       ItemComponent.tComponentPart.Blade);
		string handleCode2 = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Light, ItemBase.tOreType.Ethereal, ItemWeapon.tWeaponType.WeaponSword,
		                                                        ItemComponent.tComponentPart.Handle);

        ItemBase myBlade = ItemFactory.createComponent(bladeCode);
        ItemBase myHandle = ItemFactory.createComponent(handleCode);
                ItemBase myBlade2 = ItemFactory.createComponent(bladeCode2);
        ItemBase myHandle2 = ItemFactory.createComponent(handleCode2);

        ItemWeapon myWeapon = ItemFactory.createWeapon((ItemComponent) myBlade, (ItemComponent) myHandle);
        ItemWeapon myWeapon2 = ItemFactory.createWeapon((ItemComponent) myBlade2, (ItemComponent) myHandle2);
        inventory.inventoryAddItem(myWeapon);

		base.Start();
        primaryAttackDamage = 20.0f;
        equipWeapon(ItemWeapon.tWeaponType.WeaponPickaxe.ToString());

//		inventory.inventoryAddItem (ItemFactory.createComponent (staffHandleCode)); //Add a staff handle
//		inventory.inventoryAddItem (ItemFactory.createComponent (staffBladeCode)); //Add a staff blade
//		inventory.inventoryAddItem (ItemFactory.createComponent (bladeCode));
//		inventory.inventoryAddItem (ItemFactory.createComponent (handleCode));

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
				weapon.attack();
			//else
			//	print ("You cannot attack without a weapon!");
		}

        if(InputContextManager.isATTACK_RELEASED())
        {
            if (weapon != null)
                weapon.onAttackButtonReleased();
        }

		if(InputContextManager.isSPECIAL_ATTACK())
		{
//			print ("right clicked...");
			
			if (weapon != null)
			{
				weapon.attackSpecial();
			}
		}
		
		const int numWeapons = 6;
		if(InputContextManager.isSWITCH_WEAPON())
		{
			if (wep > (numWeapons-1))
			{
				wep = 0;
			}
			
			//print ("switch to wep: " + wep);
			
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
                case 3:
                    equipWeapon ("WeaponToolbox");
                    wep++;
                    break;
                case 4:
                    equipWeapon ("WeaponBow");
                    wep++;
                    break;
				case 5:
					equipWeapon ("WeaponKey");
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
	
	public void setSpeed(float newSpeed)
	{
		moveSpeed = newSpeed;
		CharacterMotor m = gameObject.GetComponent<CharacterMotor>();
		m.movement.maxForwardSpeed = moveSpeed;
		m.movement.maxSidewaysSpeed = moveSpeed;
		m.movement.maxBackwardsSpeed = moveSpeed;
	}
	
	public void resetSpeed()
	{
		moveSpeed = walkSpeedNormal;
		CharacterMotor m = gameObject.GetComponent<CharacterMotor>();
		m.movement.maxForwardSpeed = walkSpeedNormal;
		m.movement.maxSidewaysSpeed = walkSpeedNormal;
		m.movement.maxBackwardsSpeed = walkSpeedNormal;
	}
    
	public override void equipWeapon(string newWeapon)
    {
        base.equipWeapon(newWeapon);
        wepSwitcher.SwitchWeapon(newWeapon);
    }   
        
	protected override void killUnit ()
	{
		//print ("How did you die...???");
		
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
