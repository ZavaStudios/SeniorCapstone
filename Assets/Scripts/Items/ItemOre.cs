using UnityEngine;
using System.Collections;

/// <summary>
/// This class represent a piece of ore
/// </summary>
public class ItemOre : ItemBase
{
	//NOTE: tOreType enum is in ItemBase Class

    /// <summary>
    /// This constructor will create an ore of the specified type
    /// </summary>
    /// <param name="type"></param>
    public ItemOre(tOreType type)
        : base(getOreString(type), type)
    {
        oreType = type;
        this.type = tItemType.Ore;
        _isStackable = true;
        _quantity = 1;
    }

    /// <summary>
    /// This property affects the quantity of the ore
    /// </summary>
	public int Quantity
	{
		get{ return _quantity;}
		set{ _quantity = value; }
	}

    /// <summary>
    /// How this ore should look as a string.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        switch (oreType)
        {
            case tOreType.Bone:
                return "Bone";
            case tOreType.Copper:
                return "Copper Ore";
            case tOreType.Dragon:
                return "Dragon Scale";
            case tOreType.Iron:
                return "Iron Ore";
            case tOreType.Mithril:
                return "Mithril Ore";
            case tOreType.Steel:
                return "Steel Ore";
            case tOreType.Ethereal:
                return "Ethereal Ore";
            default:
                return "";

        }
    }
	
}
