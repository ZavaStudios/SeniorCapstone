using UnityEngine;
using System.Collections;

public class BaseItem
{
    string _name;
    string description;
    int quantity;
    
    public BaseItem(string name)
    {
        _name = name;
        description = "This item is called " + _name + ".";
        int quantity = 1;
    }
 
    virtual public string toString()
    {
        return description;
    }
}
