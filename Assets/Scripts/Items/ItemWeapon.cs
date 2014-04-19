using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class represents a weapon.
/// </summary>
public class ItemWeapon : ItemEquipment {

    public tWeaponType weaponType;

    /// <summary>
    /// Types of weapons
    /// </summary>
    public enum tWeaponType
    {
        WeaponSword = 0,
        WeaponStaff = 1,
        WeaponBow = 2,
        WeaponToolbox = 3,
        WeaponPickaxe = 4,
		WeaponKey = 5,
    };

    /// <summary>
    /// Get weapons that can't be crafted
    /// </summary>
    /// <returns></returns>
	public static List<tWeaponType> getNonCraftingWeapons()
	{
		List<tWeaponType> nonCraftingWeapons = new List<tWeaponType>();

		nonCraftingWeapons.Add(tWeaponType.WeaponPickaxe);
		nonCraftingWeapons.Add(tWeaponType.WeaponKey);

		return nonCraftingWeapons;
	}

    public ItemWeapon(string name, tWeaponType wepType)
        : base(name, tItemType.Weapon)
    {
        this.weaponType = wepType;        
    }

    public ItemWeapon(float damage, float atkspd, float armor, float health, float moveSpeedModifier, string name, tWeaponType wepType, string description, ItemBase.tOreType bladeOreType)
        : base(damage,atkspd,armor,health,moveSpeedModifier,name,tItemType.Weapon,description)
	{
        this.weaponType = wepType;
        this.oreType = bladeOreType;
	}
}
