using UnityEngine;
using System.Collections;

public class Inventory
{
    private ArrayList weapons = new ArrayList();
    private ArrayList ores = new ArrayList();
    
    // Use this for initialization
    public Inventory()
    {
    }
 
	/// <summary>
	/// Add a weapon to the inventory by string.
	/// </summary>
	/// <param name="weaponBase">Weapon base.</param>
    public void inventoryAddWeapon(WeaponBase weaponBase)
	{
		weapons.Add (weaponBase);
	}
    
	/// <summary>
	/// Remove an item from the inventory by string.
	/// </summary>
	/// <param name="weaponBase">Weapon base.</param>
	public void inventoryRemoveWeapon(WeaponBase weaponBase)
	{
		weapons.Remove(weaponBase);
	}

	/// <summary>
	/// Gets the inventory weapon string at the specified index.
	/// </summary>
	/// <returns>The <see cref="System.String"/>.</returns>
	/// <param name="index">Index.</param>
    public WeaponBase getInventoryWeaponAt(int index)
	{
		return (WeaponBase)weapons[index];
	}

    /// <summary>
    /// Gets the index of the specified weapon in the inventory.
    /// </summary>
    /// <returns>The inventory weapon index.</returns>
    /// <param name="name">Name.</param>
	public int getInventoryWeaponIndex(string name)
	{
		return weapons.IndexOf (name);
	}

	public ArrayList getInventoryWeapons()
	{
		return weapons;
	}
}
