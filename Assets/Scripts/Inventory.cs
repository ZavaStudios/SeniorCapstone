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
 
	public void inventoryAddWeapon(string weaponBase)
	{
		weapons.Add (weaponBase);
	}
    
	public void inventoryRemoveWeapon(string weaponBase)
	{
		weapons.Remove(weaponBase);
	}

	public ArrayList getInventoryWeapons()
	{
		return weapons;
	}
}
