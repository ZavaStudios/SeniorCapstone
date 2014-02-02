using UnityEngine;
using System.Collections;

public class ItemOre : ItemBase
{
    public ItemOre(tOreType type)
        : base(getOreString(type))
    {
        oreType = type;
        _isStackable = true;
        _quantity = 1;
    }

    public override string toString()
    {
        return description;
    }
	
}
