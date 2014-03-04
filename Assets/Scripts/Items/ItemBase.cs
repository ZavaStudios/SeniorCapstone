using UnityEngine;
using System.Collections;
using System;

public class ItemBase
{
    protected string _name;
    public string _description;
	protected int _quantity;
    protected bool _isStackable;
    public tOreType oreType;
    public Texture2D imageIcon;
    public tItemType type;

    public enum tItemType
    {
        Weapon,
        Armor,
        Component,
        Item,
        Ore
    }

    public enum tOreType
    {
        NOT_ORE = 0,
        Bone = 1,
        Copper = 2,
        Iron = 3,
        Steel = 4,
        Mithril = 5,
        Dragon = 6,
        Ethereal = 7
    }

    static public string getOreString(tOreType ore)
    {
//        switch (ore)
//        {
//            case tOreType.Bone:
//                return "Bone";
//            case tOreType.Copper:
//                return "Copper";
//            case tOreType.Iron:
//                return "Iron";
//            case tOreType.Steel:
//                return "Steel";
//            case tOreType.Mithril:
//                return "Mithril";
//            case tOreType.Dragon:
//                return "Dragon";
//            case tOreType.Ethereal:
//                return "Ethereal";
//            default:
//                return "Not Ore";
//        }
		return Enum.GetName(typeof(tOreType), ore);


    }
    
    public ItemBase(string name)
    {
        oreType = tOreType.NOT_ORE;
        type = tItemType.Item;
        _name = name;
        _description = "This item is called " + _name + ".";
		_quantity = 1;
    }
 
    virtual public string ToString()
    {
        return _name;
    }

    virtual public string getDescription()
    {
        return _description;
    }
 
    public bool isStackable
	{
        set { this._isStackable = value; }
        get { return this._isStackable; }
	}

    public string name
    {
        get { return this.ToString(); }
    }	
	

}