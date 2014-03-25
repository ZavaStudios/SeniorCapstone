﻿using UnityEngine;
using System.Collections;
using System;

public class ItemArmor : ItemEquipment {
 
    public string strArmorCode;

    //	NOTE New enum for componentType. Length 3 strings, abcd. a={Light=0, Normal=1, Heavy=2}, b=tOreType, c=tArmorType
	//	Example: 030 = Light iron helmet
	//	Example: 272 = Heavy ethereal greaves
    
    public enum tAttributeType
	{
		Light = 0,
		Normal = 1,
		Heavy = 2
	};

    public enum tArmorPart
	{
		Head = 0,
		Chest = 1,
        Legs = 2
	};

    public static string generateArmorCode(tAttributeType att, tOreType ore, ItemArmor.tArmorPart part)
	{
		return "" + (int)att + (int)ore + (int)part;
	}

	public static tAttributeType getArmorAttribute(string strCode)
	{
		return (tAttributeType)(Char.GetNumericValue (strCode [0]));
	}

	public static tOreType getArmorOre(string strCode)
	{
		return (tOreType)(Char.GetNumericValue (strCode [1]));
	}

	public static tArmorPart getArmorPart(string strCode)
	{
		return (tArmorPart)(Char.GetNumericValue (strCode [2]));
	}

	public static string getComponentCategoryCode(ItemWeapon.tWeaponType wep, tArmorPart part)
	{
		return "" + (int)wep + (int)part;
	}

	/// <summary>
	/// This method takes a full armor code (length 3) and returns the name
	/// </summary>
	public static string getArmorName(string strCode)
	{
		string strName = "";

		if (strCode.Length == 3) 
		{
			strName += ((tAttributeType)(Char.GetNumericValue(strCode[0]))).ToString () + " ";
			strName += ((tOreType)(Char.GetNumericValue(strCode[1]))).ToString () + " ";
			strName += ((tArmorPart)(Char.GetNumericValue(strCode[2]))).ToString () + " ";
			
		} 
		else
		{
			//Debug.Log("Invalid component code. Code must be in the format [0-2][0-7][0-5][0-1]");
			strName = "N/A";
		}

		return strName;
	}

    public ItemArmor(float damage, float atkspd, float armor, float health, float moveSpeedModifier, string name, string armorCode, string description)
        : base(damage, atkspd, armor, health, moveSpeedModifier, name, tItemType.Armor, description)
	{
        this.oreType = ItemComponent.getComponentOre(armorCode);
		this.strArmorCode = armorCode;
	}

}