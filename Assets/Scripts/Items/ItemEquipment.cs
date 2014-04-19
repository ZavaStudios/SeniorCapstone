using UnityEngine;
using System.Collections;

/// <summary>
/// The equipment class is a subclass of the base item.
/// Equipments consist stat affecting items.
/// </summary>
public class ItemEquipment : ItemBase {
 
    //Default values
	protected float _damage = 0.0f;
	protected float _atkspd = 0.0f;
	protected float _armor = 0.0f;
	protected float _health = 0.0f;
    protected float _moveSpeedModifier = 0.0f;
    
    /// <summary>
    /// The constructor which creates an equipment of the specified type with the specified name
    /// </summary>
    /// <param name="name"></param>
    /// <param name="itemtype"></param>
    public ItemEquipment(string name, tItemType itemtype) 
        : base(name)
    {
        this.type = itemtype;
    }

    /// <summary>
    /// The constructor which makes an equipment with all the given stats and with the description.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="atkspd"></param>
    /// <param name="armor"></param>
    /// <param name="health"></param>
    /// <param name="moveSpeedModifier"></param>
    /// <param name="name"></param>
    /// <param name="itemtype"></param>
    /// <param name="description"></param>
    public ItemEquipment(float damage, float atkspd, float armor, float health, float moveSpeedModifier, string name, tItemType itemtype, string description)
        : base(name)
	{
		_damage = damage;
		_atkspd = atkspd;
		_armor = armor;
		_health = health;
        _moveSpeedModifier = moveSpeedModifier;
        this.type = itemtype;

	}
	
    /// <summary>
    /// Public property to access how much the equipment affects the damage
    /// </summary>
	public float damage
    {
        set { this._damage = value; }
        get { return this._damage; }
    }
	
    /// <summary>
    /// Public property to access how much the equipment affects the attack speed
    /// </summary>
	public float atkspd
    {
        set { this._atkspd = value; }
        get { return this._atkspd; }
    }
	
    /// <summary>
    /// Public property to access how much the equipment affects the damage taken
    /// </summary>
	public float armor
    {
        set { this._armor = value; }
        get { return this._armor; }
    }
	
    /// <summary>
    /// Public property to  access how much the equipment affects health points
    /// </summary>
	public float health
    {
        set { this._health = value; }
        get { return this._health; }
    }

    /// <summary>
    /// Public property for how much the equipment modifies the movement speed
    /// </summary>
    public float moveSpeedModifier
    {
        set { this._moveSpeedModifier = value; }
        get { return this._moveSpeedModifier; }
    }

    /// <summary>
    /// The description for the equipment
    /// </summary>
	public string description
	{
		get{ return this._description; }
	}
}
