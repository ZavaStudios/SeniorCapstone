using UnityEngine;
using System.Collections;

/// <summary>
/// This static class is used to construct items given some parameters
/// </summary>
public static class ItemFactory 
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="blade"></param>
	/// <param name="handle"></param>
	/// <returns>Returns null on failure, if blade and handle type mismatch.</returns>
	public static ItemWeapon createWeapon(ItemComponent blade, ItemComponent handle)
	{
		//Since we only have 2 weapon parts now, just make sure they're not the same
		bool haveBladeAndHandle = ItemComponent.getComponentPart(blade.strComponentCode) != ItemComponent.getComponentPart(handle.strComponentCode);

		//If we don't have all the required parts, return null
		if(!haveBladeAndHandle)
			return null;

		int bladeType = (int)ItemComponent.getComponentAttribute(blade.strComponentCode);
		int handleType = (int)ItemComponent.getComponentAttribute(handle.strComponentCode);

		int bladeWeaponType = (int)ItemComponent.getComponentWeaponType (blade.strComponentCode);
		int handleWeaponType = (int)ItemComponent.getComponentWeaponType (handle.strComponentCode);
		
		if (bladeType != handleType || bladeWeaponType != handleWeaponType) //Can't mix attributes(e.g. light and heavies) or weapon types(e.g. sword, bow)
		{
			return null;
		} 
		
        float totalDmg = handle.damage + blade.damage;
		float totalSpeed = handle.atkspd + blade.atkspd;
        float totalArmor = blade.armor + handle.armor;
		float totalHealth = blade.health + handle.health;
        float totalMoveSpeedModifier = handle.moveSpeedModifier + blade.moveSpeedModifier; //get moveSpeed from handle

		string handleOre = ItemBase.getOreString(handle.oreType);
		string bladeOre = ItemBase.getOreString(blade.oreType);
		string weaponString = ItemComponent.getComponentWeaponType(blade.strComponentCode).ToString();
		
		ItemWeapon.tWeaponType wepType = ItemComponent.getComponentWeaponType(blade.strComponentCode);
		
		string weaponName = handleOre + " handled " + bladeOre + " " + weaponString;
		string weaponDescription = "A fine " + bladeOre + " " + weaponString + ", crafted with a " + handleOre + getComponentString(handle.strComponentCode) + ".";
		
		return new ItemWeapon(totalDmg,totalSpeed,totalArmor,totalHealth,totalMoveSpeedModifier,weaponName,wepType,weaponDescription,blade.oreType);
	}

    //creates an ItemArmor based on the ArmorCode passed to it.
    public static ItemArmor createArmor (string strArmorCode)
    {
        int oreType =(int) ItemArmor.getArmorOre (strArmorCode); 
        string itemName = ItemArmor.getArmorName (strArmorCode);
		string itemDescription =  "This is a " + ItemArmor.getArmorName(strArmorCode) + ".";

        float damage = 0.0f;
		float speed = 0.0f;
        
        float armor = 0.0f;
		float health = 0.0f;
        float moveSpeedModifier = 0.0f;
        
        
        //build damage and delay based on armor part...
        switch(ItemArmor.getArmorPart(strArmorCode))
        {
            case ItemArmor.tArmorPart.Chest:
                armor = 2.5f * oreType;
                health = 5 * oreType;
                health += (oreType > 4) ? 5 : 0;
                health += (oreType == 7) ? 5 : 0;
            break;
            case ItemArmor.tArmorPart.Head:
                armor = 1.5f * oreType;
                health = 2 * oreType;
                damage += 5;
                damage += (oreType > 4) ? 5 : 0;
                damage += (oreType == 7) ? 5 : 0;
            break;
            case ItemArmor.tArmorPart.Legs:
                armor = 2f * oreType;
                health = 3 * oreType;
                moveSpeedModifier += 0.15f;
                speed -= (oreType > 4) ? 0.01f : 0;
                speed -= (oreType == 7) ? 0.02f : 0;
            break;
            default:
                health = -99;
                moveSpeedModifier = -1;
            break;
        }
        
        //modify damage delay health etc based on heavy/light status
        switch(ItemArmor.getArmorAttribute(strArmorCode))
        {
            case ItemComponent.tAttributeType.Heavy:
                armor *= (1+ 0.1f*oreType);
                health *= (1+ 0.1f*oreType);
                moveSpeedModifier += (-0.08f + 0.005f * oreType); 
                damage *= 0.7f;
                speed *= -0.7f;
            break;
            case ItemComponent.tAttributeType.Normal:
                damage += 1 * oreType;
                speed *= -0.7f;
            break;
            case ItemComponent.tAttributeType.Light:
                damage *= 0.7f;
                armor *= (0.2f + 0.1f *oreType);
                health *= (0.2f + 0.1f *oreType);
                moveSpeedModifier += (0.0075f * oreType);
            break;
        }

        return new ItemArmor (damage, speed, armor, health, moveSpeedModifier, itemName, strArmorCode, itemDescription);
    }

	public static ItemComponent createComponent (string strCompCode)
	{
		ItemBase.tOreType oreType = ItemComponent.getComponentOre (strCompCode);

		string itemName = ItemComponent.getComponentName (strCompCode);
		string itemDescription =  "This is a " + ItemComponent.getComponentName(strCompCode) + ".";

        float damage = 0.0f;
		float speed = 0.0f;
        
        float armor = 0.0f;
		float health = 0.0f;
        float moveSpeedModifier = 0.0f;
    
        //build damage and delay based on weapon type...
        switch(ItemComponent.getComponentWeaponType(strCompCode))
        {
            case ItemWeapon.tWeaponType.WeaponBow:
                damage += 5 * (int)oreType; // 5, 10, 15, 20, 25, 30, 35 (all can be x3 via 3 arrow hits>>> 105 possible damage level 8)
                speed += 1.6f - (int)oreType * 0.1f; //from 1.5 second delay to 0.7 second delay. ...> 150 possible dps, no special attack dps
            break;
            case ItemWeapon.tWeaponType.WeaponStaff:
                damage += 10 + 5 * (int) oreType; // 20, 25, 30, 35, 40, 45, 50, 55
                speed += 2.05f - (int)oreType * 0.15f; // from 2.1 second delay to 0.85 second delay > 64 possible dps, also special attack
            break;
            case ItemWeapon.tWeaponType.WeaponSword:
                damage += 25 + 2 * (int) oreType + (((int)oreType*(int)oreType)/49)*50; // 28-89 damage
                speed += 2.00f - (int)oreType * 0.1f; // from 2.1 second delay to 1.2 second delay > 74 possible dps, also special attack
            break;
            case ItemWeapon.tWeaponType.WeaponToolbox:
                damage += 25 + 5 * (int) oreType; // 30, 35, 40, 45, 50, 55, 60, 65
                speed += 1.9f - (int)oreType * 0.1f; // from 1.8 second delay to 1.0 second delay > 65 possible dps, also special attack
            break;
            default:
                damage += 10.0f;
                speed += 1.0f;
            break;
        }
        
        //modify damage and delay based on heavy/light
	    switch(ItemComponent.getComponentAttribute(strCompCode))
        {
            case ItemComponent.tAttributeType.Heavy:
                damage *= 1.3f;
                speed *= 1.1f;
                moveSpeedModifier = -0.25f + 0.02f * (int)oreType;
            break;
            case ItemComponent.tAttributeType.Normal:
                health += 1 + 2 * (int)oreType; //5 to 17 bonus health
                armor +=  2 * (int)oreType; //5 to 17 bonus armor
            break;
            case ItemComponent.tAttributeType.Light:
                damage *= 0.9f;
                speed *= 0.7f;
                moveSpeedModifier = 0 + 0.02f * (int)oreType;
            break;
        }

        //70% of damage comes from blade, 70% of delay comes from handle...
        if ( ItemComponent.getComponentPart(strCompCode) == ItemComponent.tComponentPart.Blade)
        {
            damage *= 0.7f;
            speed *= 0.3f;

            moveSpeedModifier *= 0.3f;
        }
        else
        {
            damage *= 0.3f;
            speed *= 0.7f;

            moveSpeedModifier *= 0.7f;
        }

        return new ItemComponent (damage, speed, armor, health, moveSpeedModifier, itemName, strCompCode, itemDescription);
    }
        

	//helper function to create more realistic english sounding names in game.
	static string getComponentString(string componentCode)
	{
		int weaponType = (int)ItemComponent.getComponentWeaponType (componentCode);
		string componentString;

		bool trueIfBlade = ItemComponent.getComponentPart (componentCode) == ItemComponent.tComponentPart.Blade;

		switch (weaponType)
		{
		case 0:
			componentString = (trueIfBlade) ? "Blade" : "Handle";
			break;
		case 1:
			componentString = (trueIfBlade) ? "Powerstone" : "Shaft";
			break;
		case 2:
			componentString = (trueIfBlade) ? "Box" : "Handle";
			break;
		case 3:
			componentString = (trueIfBlade) ? "Stack of Arrows" : "Bow";
			break;
		default:
			componentString = "";
			break;
		}

		return componentString;
	}



}

