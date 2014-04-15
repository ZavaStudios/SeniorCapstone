using UnityEngine;
using System.Collections;

public class ItemOre : ItemBase
{
	//NOTE tOreType enum is in ItemBase Class


    public ItemOre(tOreType type)
        : base(getOreString(type), type)
    {
        oreType = type;
        this.type = tItemType.Ore;
        _isStackable = true;
        _quantity = 1;
    }

	public int Quantity
	{
		get{ return _quantity;}
		set{ _quantity = value; }
	}

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
