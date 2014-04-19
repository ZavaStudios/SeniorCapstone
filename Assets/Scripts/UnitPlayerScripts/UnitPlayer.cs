using UnityEngine;
using System.Collections;

public class UnitPlayer : Unit {
	
    private Inventory inventory;

    public const float DefaultMoveSpeed = 8.0f;
    public const float DefaultMaxHealth = 100;

    public const float healthPerSec = 1; //health regeneration

    private enum cheatAmount
    {
        a_lot,
        a_little
    };

    WeaponModelSwitcher wepSwitcher;

	protected override void Start () 
	{
	    wepSwitcher = gameObject.GetComponentInChildren<WeaponModelSwitcher>();


		inventory = Inventory.getInstance();

        //Add the default weapons
		ItemEquipment myFirstPickaxe = new ItemWeapon(1, 1.0f, 0, 0, 0.0f, "Rusty Pickaxe", ItemWeapon.tWeaponType.WeaponPickaxe, "A slightly worn, but reliable pickaxe.",ItemBase.tOreType.Copper);

        string bladeCode = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Normal, ItemBase.tOreType.Bone, ItemWeapon.tWeaponType.WeaponSword,
		                                                       ItemComponent.tComponentPart.Blade);
		string handleCode = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Normal, ItemBase.tOreType.Bone, ItemWeapon.tWeaponType.WeaponSword,
		                                                        ItemComponent.tComponentPart.Handle);
        ItemWeapon myFirstSword = ItemFactory.createWeapon(ItemFactory.createComponent(bladeCode), ItemFactory.createComponent(handleCode));
        
        inventory.inventoryAddItem(myFirstPickaxe);
        inventory.inventoryAddItem(myFirstSword);

        cheat(cheatAmount.a_little);//add all weapons
        //cheat(cheatAmount.a_lot);//add all weapons

        base.Start();

        inventory.initialize();//calling initialize here ensures inventory Character field is set before it is used
        inventory.inventorySwitchWeapon();
    }


    //this function is used as a development tool to add all weapons to the player.
    private void cheat(cheatAmount cheatHowBadly)
    {   

        ItemBase.tOreType oreToUse = ItemBase.tOreType.Bone;
        if (cheatHowBadly == cheatAmount.a_lot)
        {
            oreToUse = ItemBase.tOreType.Dragon;
        }

        //Give the player crafting points
        for (int i = 0; i < 10; i++)
        {
            incrementScore();
            inventory.inventoryAddItem(new ItemOre(ItemBase.tOreType.Bone));
        }

                string bladeCode = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Light, oreToUse, ItemWeapon.tWeaponType.WeaponSword,
		                                                       ItemComponent.tComponentPart.Blade);
		string handleCode = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Light, oreToUse, ItemWeapon.tWeaponType.WeaponSword,
		                                                        ItemComponent.tComponentPart.Handle);
        string bladeCode2 = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Light, oreToUse, ItemWeapon.tWeaponType.WeaponStaff,
		                                                       ItemComponent.tComponentPart.Blade);
		string handleCode2 = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Light, oreToUse, ItemWeapon.tWeaponType.WeaponStaff,
		                                                        ItemComponent.tComponentPart.Handle);
        string bladeCode3 = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Light, oreToUse, ItemWeapon.tWeaponType.WeaponBow,
		                                                       ItemComponent.tComponentPart.Blade);
		string handleCode3 = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Light, oreToUse, ItemWeapon.tWeaponType.WeaponBow,
		                                                        ItemComponent.tComponentPart.Handle);
		string bladeCode4 = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Light, oreToUse, ItemWeapon.tWeaponType.WeaponToolbox,
		                                                       ItemComponent.tComponentPart.Blade);
		string handleCode4 = ItemComponent.generateComponentCode (ItemComponent.tAttributeType.Light, oreToUse, ItemWeapon.tWeaponType.WeaponToolbox,
		                                                        ItemComponent.tComponentPart.Handle);
        
        ItemBase myBlade = ItemFactory.createComponent(bladeCode);
        ItemBase myHandle = ItemFactory.createComponent(handleCode);
        ItemBase myBlade2 = ItemFactory.createComponent(bladeCode2);
        ItemBase myHandle2 = ItemFactory.createComponent(handleCode2);
        ItemBase myBlade3 = ItemFactory.createComponent(bladeCode3);
        ItemBase myHandle3 = ItemFactory.createComponent(handleCode3);
        ItemBase myBlade4 = ItemFactory.createComponent(bladeCode4);
        ItemBase myHandle4 = ItemFactory.createComponent(handleCode4);

        ItemWeapon myWeapon = ItemFactory.createWeapon((ItemComponent) myBlade, (ItemComponent) myHandle);
        ItemWeapon myWeapon2 = ItemFactory.createWeapon((ItemComponent) myBlade2, (ItemComponent) myHandle2);
        ItemWeapon myWeapon3 = ItemFactory.createWeapon((ItemComponent) myBlade3, (ItemComponent) myHandle3);
        ItemWeapon myWeapon4 = ItemFactory.createWeapon((ItemComponent) myBlade4, (ItemComponent) myHandle4);
        
        inventory.inventoryAddItem(myWeapon);
        inventory.inventoryAddItem(myWeapon2);
        inventory.inventoryAddItem(myWeapon3);
        inventory.inventoryAddItem(myWeapon4);
        inventory.inventoryAddItem(new ItemKey("The cheaters key"));
    }
    
    //this function is called by the blockDestroyerScript to 
    //give crafting points to the player
	public void incrementScore()
	{
		CraftingPoints++;
	}

    //this function gets input from the player.
	protected override void Update () 
	{
        Health += healthPerSec * Time.deltaTime;
        Health = Mathf.Min(maxHealth,Health);

		if(InputContextManager.isATTACK()) 
		{
			if (weapon != null)
				weapon.attack();
		}
        
		if(InputContextManager.isSPECIAL_ATTACK())
		{
			if (weapon != null)
			{
				weapon.attackSpecial();
			}
		}
		
		if(InputContextManager.isSWITCH_WEAPON())
		{
            inventory.inventorySwitchWeapon();
		}
		
		if(InputContextManager.isSPRINT())
		{
			temporarySetMoveSpeed(moveSpeed*1.6f);
		}
		else
		{
			resetMoveSpeed();
		}
	}
	
	
	//Gets reference to the character motor class, then sets the move speed
	public void resetMoveSpeed()
	{ 
		CharacterMotor m = gameObject.GetComponent<CharacterMotor>();
		m.movement.maxForwardSpeed = moveSpeed;
		m.movement.maxSidewaysSpeed = moveSpeed;
		m.movement.maxBackwardsSpeed = moveSpeed;	
	}
	
	public void temporarySetMoveSpeed(float newSpeed)
	{
		CharacterMotor m = gameObject.GetComponent<CharacterMotor>();
		m.movement.maxForwardSpeed = newSpeed;
		m.movement.maxSidewaysSpeed = newSpeed;
		m.movement.maxBackwardsSpeed = newSpeed;
	}

	//given an itemWeapon instance, equips the appropriate 
    //weapon model and texture
	public override void equipWeapon(ItemWeapon newWeapon)
    {
        if(newWeapon==null) {return;}

        base.equipWeapon(newWeapon);
        wepSwitcher.SwitchWeapon(newWeapon.weaponType, newWeapon.oreType);
    }   
    

    //this function should be called when the player dies
	protected override void killUnit ()
	{
		
		//Draws the gameover GUI to the screen. 
		GameOver.gameOver = true;

        inventory.removeInstance();
		
		//Wait for 5 seconds.
		StartCoroutine(wait(5));

		LevelHolder.Level = 1;
		
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
