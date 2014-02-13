using UnityEngine;
using System.Collections;
using System;

public class ItemComponent : ItemEquipment {

    public tComponentType componentType;

    public enum tComponentType
    {
        SwordHandleNormal = 0,
        StaffHandleNormal = 1,
        ToolboxHandleNormal = 2,
        BowHandleNormal = 3,

        SwordHandleLight = 10,
        StaffHandleLight = 11,
        ToolboxHandleLight = 12,
        BowHandleLight = 13,

        SwordHandleHeavy = 20,
        StaffHandleHeavy = 21,
        ToolboxHandleHeavy = 22,
        BowHandleHeavy = 23,

        SwordBladeNormal = 50,
        StaffBladeNormal = 51,
        ToolboxBladeNormal = 52,
        BowBladeNormal = 53,

        SwordBladeLight = 60,
        StaffBladeLight = 61,
        ToolboxBladeLight = 62,
        BowBladeLight = 63,

        SwordBladeHeavy = 70,
        StaffBladeHeavy = 71,
        ToolboxBladeHeavy = 72,
        BowBladeHeavy = 73
    };

	//TODO Check with Ari before switching to new system (Sorry I'm already using this system, but it was a lot easier to use)
	//		Pending approval, change constructor etc. to use this code instead
	//	NOTE New enum for componentType. Length 4 strings, abcd. a={Light=0, Normal=1, Heavy=2}, b=tOreType, c=tWeaponType d={handle=0, blade=1}.
	//	Example: 0300 = Light copper sword handle
	//	Example: 2731 = Heavy ethereal toolbox blade
	// To check whether 2 items can be combined, simply make sure that two items have the same tWeaponType and tOreType and that XOR of d = true
	//		To expand the number of possible combinations, simply change a,b,c, or d to use letters instead
	//		This also means we don't have to enumerate through all possible items, and we can infer things from the code.
	public enum tAttributeType
	{
		Light = 0,
		Normal = 1,
		Heavy = 2
	};

	public enum tComponentPart
	{
		Handle = 0,
		Blade = 1
	};

	public static string getComponentCode(tAttributeType att, tOreType ore, ItemWeapon.tWeaponType wep, tComponentPart part)
	{
		return "" + (int)att + (int)ore + (int)wep + (int)part;
	}

	public static string getComponentCategoryCode(ItemWeapon.tWeaponType wep, tComponentPart part)
	{
		return "" + (int)wep + (int)part;
	}

	/// <summary>
	/// This method takes a full component code (length 4) and returns the name, or
	/// a partial code (length 2) of the category e.g. sword handle or sword blade
	/// </summary>
	/// <returns>The component name.</returns>
	/// <param name="strCode">String code.</param>
	public static string getComponentName(string strCode)
	{
		string strName = "";

		if (strCode.Length == 4) 
		{
			strName += ((tAttributeType)(Char.GetNumericValue(strCode[0]))).ToString () + " ";
			strName += ((tOreType)(Char.GetNumericValue(strCode[1]))).ToString () + " ";
			strName += ((ItemWeapon.tWeaponType)(Char.GetNumericValue(strCode[2]))).ToString () + " ";
			strName += ((tComponentPart)(Char.GetNumericValue(strCode[3]))).ToString () + " ";
			
		} 
		else if (strCode.Length == 2) 
		{
			//TODO If we want to avoid having something like staff blade, then make some cases of numbers or
			//		change tComponentType names to more generic things like offensive/defensive part
			strName += ((ItemWeapon.tWeaponType)(Char.GetNumericValue(strCode[0]))).ToString () + " ";
			strName += ((tComponentPart)(Char.GetNumericValue(strCode[1]))).ToString () + " ";
		}
		else
		{
			Debug.Log("Invalid component code. Code must be in the format [0-2][0-7][0-5][0-1]");
			strName = "N/A";
		}

		return strName;
	}


    public ItemComponent(string name, tItemType itemtype) 
        : base(name, itemtype)
    {
        this.type = itemtype;
        
    }

    public ItemComponent(float damage, float atkspd, float armor, float health, string name, tComponentType itemtype, tOreType itemOreType, string description)
        : base(damage,atkspd,armor,health,name,tItemType.Component,description)
	{
        this.componentType = itemtype;
        this.oreType = itemOreType;
	}
}
