using UnityEngine;
using System.Collections;

public class ItemComponent : ItemEquipment {

    public tComponentType componentType;

    public enum tComponentType
    {
        SwordHandleNormal = 0,
        StaffHandleNormal = 1,
        ToolboxHandleNormal = 2,
        BowHandleNormal = 3,

        SwordHandleLight = 10,
        StaffHandleLight = 11,
        ToolboxHandleLight = 12,
        BowHandleLight = 13,

        SwordHandleHeavy = 20,
        StaffHandleHeavy = 21,
        ToolboxHandleHeavy = 22,
        BowHandleHeavy = 23,

        SwordBladeNormal = 50,
        StaffBladeNormal = 51,
        ToolboxBladeNormal = 52,
        BowBladeNormal = 53,

        SwordBladeLight = 60,
        StaffBladeLight = 61,
        ToolboxBladeLight = 62,
        BowBladeLight = 63,

        SwordBladeHeavy = 70,
        StaffBladeHeavy = 71,
        ToolboxBladeHeavy = 72,
        BowBladeHeavy = 73
    };

    public ItemComponent(string name, tItemType itemtype) 
        : base(name, itemtype)
    {
        this.type = itemtype;
        
    }

    public ItemComponent(float damage, float atkspd, float armor, float health, string name, tComponentType itemtype, tOreType itemOreType, string description)
        : base(damage,atkspd,armor,health,name,tItemType.Component,description)
	{
        this.componentType = itemtype;
        this.oreType = itemOreType;
	}
}
