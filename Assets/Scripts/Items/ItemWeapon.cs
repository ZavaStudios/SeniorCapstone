using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemWeapon : ItemEquipment {

    public tWeaponType weaponType;

    public enum tWeaponType
    {
        WeaponSword = 0,
        WeaponStaff = 1,
        WeaponBow = 2,
        WeaponToolbox = 3,
        WeaponPickaxe = 4,
		WeaponKey = 5,
    };

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

    public ItemWeapon(float damage, float atkspd, float armor, float health, string name, tWeaponType wepType, string description)
        : base(damage,atkspd,armor,health,name,tItemType.Weapon,description)
	{
        this.weaponType = wepType;
	}
}
