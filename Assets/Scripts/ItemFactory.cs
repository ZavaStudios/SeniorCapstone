using UnityEngine;
using System.Collections;

public static class ItemFactory 
{
    
    public static ItemComponent createComponent(ItemComponent.tComponentType componentType, ItemOre ore)
    {
        string oreString = ItemBase.getOreString(ore.oreType);
        string weaponString = getWeaponString(componentType);
        string componentString = getComponentString(componentType);

        string itemDescription =  "This is a " + oreString + " " + componentString + "for a " + weaponString + ".";
        string itemName = itemName = oreString + " " + componentString;
       
        float damage = 10 * (int)ore.oreType;
        float speed = 2.0f - (int)ore.oreType * 0.2f;
        float armor = 0;
        float health = 0;

        return new ItemComponent (damage, speed, armor, health, itemName, componentType, ore.oreType, itemDescription);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="blade"></param>
    /// <param name="handle"></param>
    /// <returns>Returns null on failure, if blade and handle type mismatch.</returns>
   public static ItemWeapon createWeapon(ItemComponent blade, ItemComponent handle)
    {
        int bladeType = (int) blade.componentType % 10;
        int handleType = (int) handle.componentType % 10;


        if (bladeType != handleType)
        {
            MonoBehaviour.print("I'm sorry children. Pig and elephant DNA just wont s(p)lice...");
            return null;
        } 

        float totalDmg = blade.damage + handle.damage;
        float totalSpeed = blade.atkspd + handle.atkspd;
        float totalArmor = blade.armor + handle.armor;
        float totalHealth = blade.health + handle.health;
        
        string handleOre = ItemBase.getOreString(handle.oreType);
        string bladeOre = ItemBase.getOreString(blade.oreType);
        string weaponString = getWeaponString(blade.componentType);

        ItemWeapon.tWeaponType wepType;
        switch (bladeType)
        {
            case 0:
            {
                wepType = ItemWeapon.tWeaponType.WeaponSword;
                break;
            }
            case 1:
            {
                wepType = ItemWeapon.tWeaponType.WeaponSword;
                break;
            }
            case 2:
            {
                wepType = ItemWeapon.tWeaponType.WeaponSword;
                break;
            }
            case 3:
            {
                wepType = ItemWeapon.tWeaponType.WeaponSword;
                break;
            }
            default:
                return null;
        }

        string weaponName = handleOre + " handled " + bladeOre + " " + weaponString;
        string weaponDescription = "A fine " + bladeOre + " " + weaponString + ", crafted with a " + handleOre + getComponentString(handle.componentType) + ".";

        return new ItemWeapon(totalDmg,totalSpeed,totalArmor,totalHealth,weaponName,wepType,weaponDescription);
    }


    static string getWeaponString(ItemComponent.tComponentType componentType)
    {

        int weaponType = (int)componentType % 10;
        string weaponString;

        switch (weaponType)
        {
            case 0:
                weaponString = "Sword";
                break;
            case 1:
                weaponString = "Staff";
                break;
            case 2:
                weaponString = "Toolbox";
                break;
            case 3:
                weaponString = "Bow and Arrows";
                break;
            default:
                weaponString = "";
                break;
        }
        return weaponString;
    }

    static string getComponentString(ItemComponent.tComponentType componentType)
    {
        int weaponType = (int)componentType % 10;
        string componentString;

        bool trueIfBlade = ((int)componentType >= 50);

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
