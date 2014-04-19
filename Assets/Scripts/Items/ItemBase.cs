using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


/// <summary>
/// The itembase class keeps reference to all variables that are common to inventory items:
/// name, oretype, quantity (if stackable), description, type etc.
/// </summary>
public class ItemBase
{
    protected string _name;
    public string _description;
	protected int _quantity;
    protected bool _isStackable;
    public tOreType oreType;
    public Texture2D imageIcon;
    public tItemType type;
	public int neededOreQuantity;
	public int neededPoints;

    /// <summary>
    /// This enum specifies all types of items
    /// </summary>
    public enum tItemType
    {
        Weapon,
        Armor,
        Component,
        Item,
        Ore
    }

    /// <summary>
    /// This enum specifies all types of ore
    /// </summary>
    public enum tOreType
    {
        NOT_ORE = 0,
        Bone = 1,
        Copper = 2,
        Iron = 3,
        Steel = 4,
        Mithril = 5,
        Dragon = 6,
        Ethereal = 7,
        Stone = 8  
    }

    /// <summary>
    /// Get a list of ores that cannot be used for crafting
    /// </summary>
    /// <returns></returns>
	public static List<tOreType> getNonCraftingOres()
	{
		List<tOreType> nonCrafting = new List<tOreType>();

		nonCrafting.Add(tOreType.NOT_ORE);
		nonCrafting.Add(tOreType.Stone);

		return nonCrafting;
	}
    
    /// <summary>
    /// Get the string representation of the type of ore
    /// </summary>
    /// <param name="ore"></param>
    /// <returns></returns>
    static public string getOreString(tOreType ore)
    {
		return Enum.GetName(typeof(tOreType), ore);

    }
    
    /// <summary>
    /// This constructor makes an item with the specified name.
    /// </summary>
    /// <param name="name"></param>
    public ItemBase(string name)
    {
        oreType = tOreType.NOT_ORE;
        type = tItemType.Item;
        _name = name;
        _description = "A fine " + _name + ".";
		_quantity = 1;
		
		neededOreQuantity = 1;
        neededPoints = 1;
    }

    /// <summary>
    /// This constructor makes an item with the specified name and ore type.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="oreType"></param>
	public ItemBase(string name, ItemBase.tOreType oreType)
	{
		this.oreType = oreType;
		type = tItemType.Item;
		_name = name;
		_description = "A fine " + _name + ".";
		_quantity = 1;
		
		neededOreQuantity = 1;
        neededPoints = 1;
	}
 
    /// <summary>
    /// The string representation for an item.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return _name;
    }

    /// <summary>
    /// Get the description of the item
    /// </summary>
    /// <returns></returns>
    virtual public string getDescription()
    {
        return _description;
    }
 
    /// <summary>
    /// Returns true if the item is stackable i.e. quantity can be > 1.
    /// </summary>
    public bool isStackable
	{
        set { this._isStackable = value; }
        get { return this._isStackable; }
	}

    /// <summary>
    /// Return the name of this item.
    /// </summary>
    public string name
    {
        get { return this.ToString(); }
    }	
	

}
