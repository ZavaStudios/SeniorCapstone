using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// This class represents components which weapons are made out of
/// </summary>
public class ItemComponent : ItemEquipment {

    public string strComponentCode;

	//	Enum for componentType. Length 4 strings, abcd. a={Light=0, Normal=1, Heavy=2}, b=tOreType, c=tWeaponType d={handle=0, blade=1}.
	//	Example: 0300 = Light iron sword handle
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

    /// <summary>
    /// Types of components
    /// </summary>
	public enum tComponentPart
	{
		Handle = 0,
		Blade = 1
	};

	public static string generateComponentCode(tAttributeType att, tOreType ore, ItemWeapon.tWeaponType wep, tComponentPart part)
	{
		return "" + (int)att + (int)ore + (int)wep + (int)part;
	}

	public static tAttributeType getComponentAttribute(string strCode)
	{
		return (tAttributeType)(Char.GetNumericValue (strCode [0]));
	}

	public static tOreType getComponentOre(string strCode)
	{
		return (tOreType)(Char.GetNumericValue (strCode [1]));
	}

	public static ItemWeapon.tWeaponType getComponentWeaponType(string strCode)
	{
		return (ItemWeapon.tWeaponType)(Char.GetNumericValue (strCode [2]));
	}

	public static tComponentPart getComponentPart(string strCode)
	{
		return (tComponentPart)(Char.GetNumericValue (strCode [3]));
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
			strName += ((ItemWeapon.tWeaponType)(Char.GetNumericValue(strCode[0]))).ToString () + " ";
			strName += ((tComponentPart)(Char.GetNumericValue(strCode[1]))).ToString () + " ";
		}
		else
		{
			strName = "N/A";
		}

		return strName;
	}

	public override string ToString()
	{
		return ItemComponent.getComponentName (this.strComponentCode);
	}
    
    public ItemComponent(float damage, float atkspd, float armor, float health, float moveSpeedModifier, string name, string componentCode, string description)
		: base(damage,atkspd,armor,health,moveSpeedModifier,ItemComponent.getComponentName(componentCode),tItemType.Component,description)
	{
        this.oreType = ItemComponent.getComponentOre(componentCode);
		this.strComponentCode = componentCode;
	}
}
