using UnityEngine;
using System.Collections;


public class Inventory
{
    private ArrayList weapons;
    private ArrayList armors;
    private ArrayList components;
    private ArrayList items;
    private ArrayList ores;

    private static Inventory _instance = null;

    // Use this for initialization
    private Inventory()
    {
        weapons = new ArrayList();
        armors = new ArrayList();
        components = new ArrayList();
        items = new ArrayList();
        ores = new ArrayList();
    }

    public static Inventory getInstance()
    {
        if(_instance == null)
            _instance = new Inventory();
        
        return _instance;
    }
 
	/// <summary>
	/// Add an item to the inventory.
	/// </summary>
	/// <param name="weaponBase">Weapon base.</param>
    public void inventoryAddItem(ItemWeapon newItem)
    {
                weapons.Add((ItemWeapon)newItem);
    }

    public void inventoryAddItem(ItemEquipment newItem)
    {
        switch (newItem.type)
        {
            case ItemBase.tItemType.Armor:
                armors.Add(newItem);
                break;

            case ItemBase.tItemType.Component:
                components.Add(newItem);
                break;
            default:
                break;
        }
    }

    public void inventoryAddItem(ItemOre newItem)
    {
        ores.Add(newItem);
    }


    public void inventoryAddItem(ItemBase newItem)
    {
        items.Add(newItem);
    }
    
	/// <summary>
	/// Remove an item from the inventory by string.
	/// </summary>
	/// <param name="weaponBase">Weapon base.</param>
    public void inventoryRemoveItem(ItemBase itemToRemove)
	{
        switch (itemToRemove.type)
        {
            case ItemBase.tItemType.Weapon:
                weapons.Remove(itemToRemove);
                break;

            case ItemBase.tItemType.Armor:
                armors.Remove(itemToRemove);
                break;

            case ItemBase.tItemType.Component:
                components.Remove(itemToRemove);
                break;

            case ItemBase.tItemType.Ore:
                components.Remove(itemToRemove);
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
		return weapons;
	}

    public ArrayList getInventoryComponents()
    {
        return components;
    }
}
