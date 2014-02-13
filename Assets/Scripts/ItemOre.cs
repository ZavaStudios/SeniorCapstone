using UnityEngine;
using System.Collections;

public class ItemOre : ItemBase
{
	//NOTE tOreType enum is in ItemBase Class


    public ItemOre(tOreType type)
        : base(getOreString(type))
    {
        oreType = type;
        this.type = tItemType.Ore;
        _isStackable = true;
        _quantity = 1;
    }

    public override string toString()
    {
        return description;
    }
	
}
