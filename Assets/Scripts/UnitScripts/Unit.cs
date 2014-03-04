using UnityEngine;
using System.Collections;

[RequireComponent(typeof (CharacterController))]


public class Unit : MonoBehaviour 
{
    public Inventory inventory;


    protected float attackDamage;
    protected float attackDelay;
    protected float armor;
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

    public float AttackDamage
    {
        get { return attackDamage; }
        set { attackDamage = value; }
    }


    public float AttackDelay
    {
        get { return attackDelay; }
        set { attackDelay = value; }
    }

    public float Armor
    {
        get { return armor; }
        set { armor = value; }
    }    
    
    //////////////////////////////////
	
    virtual protected void Start () 
	{

	}
	
	virtual protected void Update () 
	{
		
	}
	
	public void doDamage(float amount)
	{
		this.health -=  amount;
		//print ("health decreased.");
		
		if (health <= 0)
		{
			health = 0;
			killUnit();
		}
	}
	
    virtual public void equipWeapon(string newWeapon)
    {
        //Debug.Log("Equipping :" + newWeapon);

        GameObject.Destroy(weapon);
        weapon = (WeaponBase) gameObject.AddComponent(newWeapon);;
        
        //Debug.Log("Current Equipped: " + weapon.strWeaponType);
       
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
