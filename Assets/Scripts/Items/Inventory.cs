﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Inventory
{
		private ArrayList weapons;
		private ArrayList armors;
		private ArrayList components;
		private ArrayList items;
		private Dictionary<int,int> ores;
		private static Inventory _instance = null;


        private ItemEquipment armorHead;
        private ItemEquipment armorChest;
        private ItemEquipment armorHands;
        private ItemEquipment armorLegs;
        private ItemEquipment armorBoots;
        private ItemEquipment weapon;

		// Use this for initialization
		private Inventory ()
		{
				weapons = new ArrayList ();
				armors = new ArrayList ();
				components = new ArrayList ();
				items = new ArrayList ();
				ores = new Dictionary<int, int> ();

                armorHead = null;
                armorChest = null;
                armorHands = null;
                armorLegs = null;
                armorBoots = null;
                weapon = null;

				List<ItemBase.tOreType> exludedOres = ItemBase.getNonCraftingOres ();
				foreach (ItemBase.tOreType oreType in Enum.GetValues(typeof(ItemBase.tOreType))) {
						if (exludedOres.Contains (oreType))
								continue;
						
						//Create all the ores beforehand w/ quantity=0
						ores [(int)oreType] = 0;
				}
		}

		public static Inventory getInstance ()
		{
				if (_instance == null)
						_instance = new Inventory ();
        
				return _instance;
		}
 
		/// <summary>
		/// Add an item to the inventory.
		/// </summary>
		/// <param name="weaponBase">Weapon base.</param>
		public void inventoryAddItem (ItemWeapon newItem)
		{
				if (newItem == null)
						return;

				weapons.Add ((ItemWeapon)newItem);
		}

		public void inventoryAddItem (ItemEquipment newItem)
		{
				if (newItem == null)
						return;

				switch (newItem.type) {
				    case ItemBase.tItemType.Armor:
						armors.Add (newItem);
						break;

				    case ItemBase.tItemType.Component:
						components.Add (newItem);
						break;
				    default:
						break;
				}
		}

		public void inventoryAddItem (ItemOre newItem)
		{
				if (newItem == null)
						return;

				//Since ores are defined soly on their ore type, we can disregard any other info about the ore
				ores [(int)newItem.oreType] += 1;
		}

		public void inventoryAddItem (ItemBase newItem)
		{
				if (newItem == null)
						return;

				items.Add (newItem);
		}
    
		/// <summary>
		/// Remove an item from the inventory by string.
		/// </summary>
		/// <param name="weaponBase">Weapon base.</param>
		public void inventoryRemoveItem (ItemBase itemToRemove)
		{
				switch (itemToRemove.type) {
				case ItemBase.tItemType.Weapon:
						weapons.Remove (itemToRemove);
						break;

				case ItemBase.tItemType.Armor:
						armors.Remove (itemToRemove);
						break;

				case ItemBase.tItemType.Component:
						components.Remove (itemToRemove);
						break;

				case ItemBase.tItemType.Ore:
						ores [(int)itemToRemove.oreType] -= 1;
						break;

				default:
                //Defaults are just generic items
						items.Remove (itemToRemove);
						break;
				}
		}

		/// <summary>
		/// Remove an item from the inventory by string.
		/// </summary>
		/// <param name="weaponBase">Weapon base.</param>
		public void inventoryRemoveItem (ItemBase itemToRemove, int quantity)
		{
				switch (itemToRemove.type) {
				case ItemBase.tItemType.Weapon:
						weapons.Remove (itemToRemove);
						break;
			
				case ItemBase.tItemType.Armor:
						armors.Remove (itemToRemove);
						break;
			
				case ItemBase.tItemType.Component:
						components.Remove (itemToRemove);
						break;
			
				case ItemBase.tItemType.Ore:
						ores [(int)itemToRemove.oreType] -= quantity;
						break;
			
				default:
			//Defaults are just generic items
						items.Remove (itemToRemove);
						break;
				}
		}

		/// <summary>
		/// Gets the inventory weapon string at the specified index.
		/// </summary>
		/// <returns>The <see cref="System.String"/>.</returns>
		/// <param name="index">Index.</param>
		public ItemWeapon getInventoryWeaponAt (int index)
		{
				return (ItemWeapon)weapons [index];
		}

		/// <summary>
		/// Gets the index of the specified weapon in the inventory.
		/// </summary>
		/// <returns>The inventory weapon index.</returns>
		/// <param name="name">Name.</param>
		public int getInventoryWeaponIndex (ItemWeapon itemToFind)
		{
				return weapons.IndexOf (itemToFind);
		}

		public ArrayList getInventoryWeapons ()
		{
				return weapons;
		}

		public ArrayList getInventoryArmors ()
		{
				return armors;
		}

		public ArrayList getInventoryComponents ()
		{
				return components;
		}

		public ArrayList getInventoryItems ()
		{
				return items;
		}

		public ArrayList getInventoryOres ()
		{
				ArrayList temp = new ArrayList ();

				//Only return those ores which have are actually present
				foreach (KeyValuePair<int,int> entry in ores) {
						if (entry.Value > 0) {
								ItemOre oreToBeAdded = new ItemOre ((ItemBase.tOreType)entry.Key);
								oreToBeAdded.Quantity = entry.Value;

								temp.Add (oreToBeAdded);
						}
				}

				return temp;
		}
		
		public int getOreQuantity (ItemBase.tOreType oreType)
		{
				return ores [(int)oreType];
		}


        protected void inventoryEquipItem(ItemEquipment itemToEquip)
        {
        }

}
