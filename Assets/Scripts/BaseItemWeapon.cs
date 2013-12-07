using UnityEngine;
using System.Collections;

public class BaseItemWeapon : BaseItemEquipable
{
    float attackSpeed = 0f;
    float damage = 0f;
    
    string _weaponType;
    
    public string weaponType
    {
        get { return _weaponType; }
        set { _weaponType = value; }
    }
    
    public BaseItemWeapon(string name, float attackSpeed, float damage) : base(name) 
    {
        
    }

}
