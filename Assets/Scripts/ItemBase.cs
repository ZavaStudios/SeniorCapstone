using UnityEngine;
using System.Collections;

public class ItemBase
{
    protected string _name;
    public string description;
	protected int _quantity;
    protected bool _isStackable;
	
    public ItemBase(string name)
    {
        _name = name;
        description = "This item is called " + _name + ".";
		_quantity = 1;
    }
 
    virtual public string toString()
    {
        return description;
    }
	
	
	public bool isStackable
	{
        set { this._isStackable = value; }
        get { return this._isStackable; }
	}
	
	

}
