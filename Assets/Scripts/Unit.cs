using UnityEngine;
using System.Collections;

[RequireComponent(typeof (CharacterController))]


public class Unit : MonoBehaviour 
{
    Inventory inventory;
    
	protected float health = 100.0f;
    protected float maxHealth = 100.0f;
	protected float moveSpeed = 10.0f;
	public WeaponBase weapon;

    public float Health
    {
        get {return health; }
        set {health = value; }
    }

	public int Score
	{
				get;
				set;
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
	
    virtual public void equipWeapon(string newWeapon)
    {
       GameObject.Destroy(weapon);
        
       gameObject.AddComponent(newWeapon);
        
       weapon = (WeaponBase) this.GetComponent(newWeapon);
       
    }   
    
	virtual protected void killUnit()
	{

	}
    
    virtual public void playAttackAnimation()
    {

    }
	
	virtual public Vector3 getLookDirection()
	{
		return gameObject.transform.forward;	
	}
	
	virtual public Quaternion getLookRotation()
	{
		return gameObject.transform.rotation;
	}
		
	virtual public Vector3 getEyePosition()
	{
		return gameObject.transform.position + new Vector3(0,0.5f,0); //eyes are on the top of the head? idk...
	}
}   
