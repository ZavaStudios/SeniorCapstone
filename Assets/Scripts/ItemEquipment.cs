using UnityEngine;
using System.Collections;

public class ItemEquipment : ItemBase {
 
	private bool _isCompleteEquipment;
	
	protected float _damage = 0.0f;
	protected float _atkspd = 0.0f;
	protected float _armor = 0.0f;
	protected float _health = 0.0f;
	
    public ItemEquipment(string name):base(name)
    {
        
    }
	
	public ItemEquipment(float damage, float atkspd, float armor, float health, string name, string description):base(name)
	{
		_damage = damage;
		_atkspd = atkspd;
		_armor = armor;
		_health = health;
	}
	
	public float damage
    {
        set { this._damage = value; }
        get { return this._damage; }
    }
	
	public float atkspd
    {
        set { this._atkspd = value; }
        get { return this._atkspd; }
    }
	
	public float armor
    {
        set { this._armor = value; }
        get { return this._armor; }
    }
	
	public float health
    {
        set { this._health = value; }
        get { return this._health; }
    }
	
	public bool isEquipable
    {
        set { this._isCompleteEquipment = value; }
        get { return this._isCompleteEquipment; }
    }
	
	
	
	
}
