using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Inventory
{
    private static UnitPlayer Character; //variable is initialied to reference player by Main.cs

    private List<ItemWeapon> weapons;
    private ArrayList armors;
    private ArrayList components;
    private ArrayList items;
    private Dictionary<int, int> ores;
    private static Inventory _instance = null;

    private ItemWeapon equipedWeapon;
    private ItemArmor equipedHelmet;
    private ItemArmor equipedChest;
    private ItemArmor equipedLegs;


    // Use this for initialization
    private Inventory()
    {
        weapons = new List<ItemWeapon>();
        armors = new ArrayList();
        components = new ArrayList();
        items = new ArrayList();
        ores = new Dictionary<int, int>();
        
        equipedWeapon = null;
        equipedHelmet = null;
        equipedChest = null;
        equipedLegs = null;

        List<ItemBase.tOreType> exludedOres = ItemBase.getNonCraftingOres();
        foreach (ItemBase.tOreType oreType in Enum.GetValues(typeof(ItemBase.tOreType)))
        {
            if (exludedOres.Contains(oreType))
                continue;

            //Create all the ores beforehand w/ quantity=0
            ores[(int)oreType] = 0;
        }
    }

    public void initialize() //cannot be called from constructor, because Unity.
    {
        Character = GameObject.FindGameObjectWithTag("Player").GetComponent<UnitPlayer>();
    }

    //Clears out the previous instance of the inventory
    public void removeInstance()
    {
        _instance = null;
    }

    public static Inventory getInstance()
    {
        if (_instance == null)
        {
            _instance = new Inventory();
        }

        return _instance;
    }

    /// <summary>
    /// Add an item to the inventory.
    /// </summary>
    /// <param name="newItem">ItemWeapon</param>
    public void inventoryAddItem(ItemWeapon newItem)
    {
        if (newItem == null)
            return;

        weapons.Add((ItemWeapon)newItem);
    }
    
    
    /// <summary>
    /// Add an item to the inventory.
    /// </summary>
    /// <param name="newItem">ItemEquipment</param>
    public void inventoryAddItem(ItemEquipment newItem)
    {
        if (newItem == null)
            return;

        switch (newItem.type)
        {
            case ItemBase.tItemType.Armor:
                armors.Add(newItem);
                break;

            case ItemBase.tItemType.Component:
                components.Add(newItem);
                break;
            case ItemBase.tItemType.Weapon:
                inventoryAddItem((ItemWeapon)newItem);
                break;
            default:
                break;
        }
    }
    
    /// <summary>
    /// Add an item to the inventory.
    /// </summary>
    /// <param name="newItem">ItemOre</param>
    public void inventoryAddItem(ItemOre newItem)
    {
        if (newItem == null)
            return;

        //Since ores are defined soly on their ore type, we can disregard any other info about the ore
        ores[(int)newItem.oreType] += 1;
    }
    
    /// <summary>
    /// Add an item to the inventory.
    /// </summary>
    /// <param name="newItem">ItemBase</param>
    public void inventoryAddItem(ItemBase newItem)
    {
        if (newItem == null)
            return;

        switch (newItem.type)
        {
            case ItemBase.tItemType.Armor:
                armors.Add(newItem);
                break;

            case ItemBase.tItemType.Component:
                components.Add(newItem);
                break;

            case ItemBase.tItemType.Weapon:
                inventoryAddItem((ItemWeapon)newItem);
                break;

            default:
                items.Add(newItem);
                break;
        }

    }

    /// <summary>
    /// Remove an item from the inventory by string.
    /// </summary>
    /// <param name="itemToRemove">ItemBase</param>
    public void inventoryRemoveItem(ItemBase itemToRemove)
    {
        switch (itemToRemove.type)
        {
            case ItemBase.tItemType.Weapon:
                
                weapons.Remove((ItemWeapon)itemToRemove);
                break;

            case ItemBase.tItemType.Armor:
                armors.Remove(itemToRemove);
                break;

            case ItemBase.tItemType.Component:
                components.Remove(itemToRemove);
                break;

            case ItemBase.tItemType.Ore:
                ores[(int)itemToRemove.oreType] -= 1;
                break;

            default:
                //Defaults are just generic items
                items.Remove(itemToRemove);
                break;
        }
    }

    /// <summary>
    /// Remove an item from the inventory by string.
    /// </summary>
    /// <param name="weaponBase">Weapon base.</param>
    public void inventoryRemoveItem(ItemBase itemToRemove, int quantity)
    {
        switch (itemToRemove.type)
        {
            case ItemBase.tItemType.Weapon:
                ItemWeapon weaponToRemove = itemToRemove as ItemWeapon;
                if(weaponToRemove.weaponType == ItemWeapon.tWeaponType.WeaponPickaxe)
                {
                    return; //NO! BAD CODER MONKEY. NO REMOVING PICKAXES!! PLAYER NEED PICKAXES!!
                }
                else if(equipedWeapon == itemToRemove)
                {
                    inventorySwitchWeapon();
                }
                weapons.Remove((ItemWeapon)itemToRemove);

                break;

            case ItemBase.tItemType.Armor:
                if(equipedChest == itemToRemove)
                {
                    inventoryEquipChest(null); //unequip
                }
                else if(equipedHelmet == itemToRemove)
                {
                    inventoryEquipHelmet(null); //unequip
                }
                else if(equipedLegs == itemToRemove)
                {
                    inventoryEquipLegs(null); //unequip
                }

                armors.Remove(itemToRemove);
                break;

            case ItemBase.tItemType.Component:
                components.Remove(itemToRemove);
                break;

            case ItemBase.tItemType.Ore:
                ores[(int)itemToRemove.oreType] -= quantity;
                break;

            default:
                //Defaults are just generic items
                items.Remove(itemToRemove);
                break;
        }
    }

    /// <summary>
    /// Gets the inventory weapon string at the specified index.
    /// </summary>
    /// <returns>The <see cref="System.String"/>.</returns>
    /// <param name="index">Index.</param>
    public ItemWeapon getInventoryWeaponAt(int index)
    {
        return (ItemWeapon)weapons[index];
    }

    /// <summary>
    /// Gets the index of the specified weapon in the inventory.
    /// </summary>
    /// <returns>The inventory weapon index.</returns>
    /// <param name="name">Name.</param>
    public int getInventoryWeaponIndex(ItemWeapon itemToFind)
    {
        return weapons.IndexOf(itemToFind);
    }

    public ArrayList getInventoryWeapons()
    {
        return new ArrayList(weapons);
    }

    public ArrayList getInventoryArmors()
    {
        return armors;
    }

    public ArrayList getInventoryComponents()
    {
        return components;
    }

    public ArrayList getInventoryItems()
    {
        return items;
    }

    public ArrayList getInventoryOres()
    {
        ArrayList temp = new ArrayList();

        //Only return those ores which have are actually present
        foreach (KeyValuePair<int, int> entry in ores)
        {
            if (entry.Value > 0)
            {
                ItemOre oreToBeAdded = new ItemOre((ItemBase.tOreType)entry.Key);
                oreToBeAdded.Quantity = entry.Value;

                temp.Add(oreToBeAdded);
            }
        }

        return temp;
    }

    public ItemArmor getEquippedHelmet()
    {
        return equipedHelmet;
    }

    public ItemArmor getEquippedChest()
    {
        return equipedChest;
    }
    public ItemArmor getEquippedLegs()
    {
        return equipedLegs;
    }

    public int getOreQuantity(ItemBase.tOreType oreType)
    {
        return ores[(int)oreType];
    }

    //can be called with null to un-equip the item
    public void inventoryEquipChest(ItemArmor newChest)
    {
        if (newChest == null)
        {
            equipedChest = null;
        }
        else if (newChest.armorPart == ItemArmor.tArmorPart.Chest)
        {
            equipedChest = newChest;
        }
        else
        {
            return;
        }
        
        calculateAndApplyStats();
    }
    
    //can be called with null to un-equip the item
    public void inventoryEquipLegs(ItemArmor newLegs)
    {
        if (newLegs == null)
        {
            equipedLegs = null;
        }
        else if (newLegs.armorPart == ItemArmor.tArmorPart.Legs)
        {
            equipedLegs = newLegs;
        }
        else
        {
            return;
        }
        
        calculateAndApplyStats();
    }

    //can be called with null to un-equip the item
    public void inventoryEquipHelmet(ItemArmor newHelmet)
    {
        if (newHelmet == null)
        {
            equipedHelmet= null;
        }
        else if (newHelmet.armorPart == ItemArmor.tArmorPart.Head)
        {
            equipedHelmet = newHelmet;
        }
        else
        {
            return;
        }
        
        calculateAndApplyStats();
    }

    //this can be called to rotate through weapons in the players inventory by pushing 'SwitchWeapon' button
    public void inventorySwitchWeapon()
    {
        if(equipedWeapon == null)
        {
            if( weapons.Count != 0)
            {
                inventoryEquipWeapon((ItemWeapon)weapons[0]);
            }
            else
            {
                inventoryEquipWeapon(null);
            }
        }

        else
        {
            int weaponIndex = getInventoryWeaponIndex(equipedWeapon);

            weaponIndex++;
            
            if (weaponIndex >= weapons.Count)
            {
                weaponIndex = 0;
            }

            inventoryEquipWeapon(getInventoryWeaponAt(weaponIndex));

        }
    }

    //can be called with null to un-equip the item
    public void inventoryEquipWeapon(ItemWeapon newWeapon)
    {
        equipedWeapon = newWeapon;
        calculateAndApplyStats();
    }
    
    //generic to equip any itemEquipment. will replace it in the appropriate spot.
    public void inventoryEquipItem(ItemEquipment itemToEquip)
    {
        //equip items if applicaple.
        if(itemToEquip is ItemWeapon)
        {
            equipedWeapon = (ItemWeapon)itemToEquip;
        }
        else if(itemToEquip is ItemArmor)
        {
            ItemArmor armorToEquip = itemToEquip as ItemArmor;
            switch (armorToEquip.armorPart)
            {
                case ItemArmor.tArmorPart.Chest:
                    equipedChest = armorToEquip;
                break;
                case ItemArmor.tArmorPart.Head:
                    equipedHelmet = armorToEquip;
                break;
                case ItemArmor.tArmorPart.Legs:
                    equipedLegs = armorToEquip;
                break;
            }
        }
        else
        {
            return; //change nothing if it was not equipable equipment....
        }

        calculateAndApplyStats();

    }

    //sums all equiped armor and sets the stats in the UnitPlayer class appropriately.
    private void calculateAndApplyStats()
    {
        
        //sum up stats of all equipment
        float health = UnitPlayer.DefaultMaxHealth;
        float armor = 0;
        float damage = 0;
        float delay = 0;
        float moveSpeedMultiplier = 0;

        moveSpeedMultiplier = 1.0f; //default multiplier = 1.0f
        
        if (equipedWeapon != null)
        {
            health += equipedWeapon.health;
            armor += equipedWeapon.armor;
            damage += equipedWeapon.damage;
            delay += equipedWeapon.atkspd;
            moveSpeedMultiplier += equipedWeapon.moveSpeedModifier;
        }
        
        
        if (equipedHelmet != null)
        {
            health += equipedHelmet.health;
            armor += equipedHelmet.armor;
            damage += equipedHelmet.damage;
            delay += equipedHelmet.atkspd;
            moveSpeedMultiplier += equipedWeapon.moveSpeedModifier;
        }
        
        
        if (equipedChest != null)
        {
            health += equipedChest.health;
            armor += equipedChest.armor;
            damage += equipedChest.damage;
            delay += equipedChest.atkspd;
            moveSpeedMultiplier += equipedWeapon.moveSpeedModifier;
        }
        
        
        if (equipedLegs != null)
        {
            health += equipedLegs.health;
            armor += equipedLegs.armor;
            damage += equipedLegs.damage;
            delay += equipedLegs.atkspd;
            moveSpeedMultiplier += equipedWeapon.moveSpeedModifier;
        }

        //update character stats;
        Character.MaxHealth = health;
        Character.Armor = armor;
        Character.AttackDamage = damage;
        Character.AttackDelay = delay;
        Character.moveSpeed = UnitPlayer.DefaultMoveSpeed * moveSpeedMultiplier;

        if (Character.Health > Character.MaxHealth)
        {
            Character.Health =  Character.MaxHealth;
        }

        Character.resetMoveSpeed();
        Character.equipWeapon(equipedWeapon); //re-equips weapon every time gear switches, but OK, not very expensive
    }
}
