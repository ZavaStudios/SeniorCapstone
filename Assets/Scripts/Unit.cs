using UnityEngine;
using System.Collections;

[RequireComponent(typeof (CharacterController))]


public class Unit : MonoBehaviour 
{
    Inventory inventory;
    
	protected float health = 100.0f;
    protected float maxHealth = 100.0f;
	protected float moveSpeed = 	10.0f;
	public WeaponBase weapon;

    public float Health
    {
        get {return health; }
        set {health = value; }
    }
	
    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }
    
    //////////////////////////////////
	
    virtual protected void Start () 
	{
        inventory = new Inventory();

		weapon = gameObject.GetComponent<WeaponBase>();
	}
	
	virtual protected void Update () 
	{
		
	}
	
	public void doDamage(float amount)
	{
		this.health -=  amount;
		
		if (health <= 0)
		{
			killUnit();
		}
	}
	
    public void equipWeapon(string newWeapon)
    {
       GameObject.Destroy(weapon);
        
       gameObject.AddComponent(typeof(WeaponSword));
        
       weapon = this.GetComponent<WeaponBase>();
        
    }   
    
	virtual protected void killUnit()
	{

	}
}
