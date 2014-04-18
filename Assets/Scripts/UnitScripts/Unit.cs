using UnityEngine;
using System.Collections;

[RequireComponent(typeof (CharacterController))]


/// <summary>
/// Unit.
/// Base class for all units. Contains all of the functions that all units have in common, such as: what to do on death, attack, etc.
/// </summary>
public class Unit : MonoBehaviour 
{
    public bool vulnerable = true;

    //character stats for this unit instance
    protected float primaryAttackDamage;
    protected float primaryAttackDelay;
    protected float armor;
    protected float maxHealth = 100.0f; 
    protected float health = 100.0f;
    public float moveSpeed = 10.0f;

	//reference to the current weapon type the unit is using    
    public WeaponBase weapon;

    
    	
    virtual protected void Start () 
	{
        health = maxHealth;
	}
	
    
    //most units will override this function so they can have unique behavior.
	virtual protected void Update () 
	{
		
	}
	
    //the value at which there are diminishing returns on armor
    private const int armorDiminishingReturnThreshold = 100;
	
    //this function does damage to the unit, based on whatever their armor statistic is.
    virtual public void doDamage(float amount)
	{
		if(!vulnerable) return;
        
        //10% reduced damage at 10% of threshold -> 10 armor
        //20% reduced damage at 25% of threshold -> 25 armor
        //50% reduced damage at threshold    -> 100 armor
        //66% reduced damage at 2x threshold -> 200 armor
        //75% reduced damage at 3x threshold -> 300 armor
        float adjustmentScalar = 1/(1 + armor/armorDiminishingReturnThreshold);

        this.health -=  amount * adjustmentScalar;
		
		if (health <= 0)
		{
			health = 0;
			killUnit();
		}
	}
	
    //destroys the old instance of the weapon script 
    //and adds a new weapon script to the unit
    virtual public void equipWeapon(ItemWeapon newWeapon)
    {
        GameObject.Destroy(weapon);
        
        if (newWeapon != null)
        {
            weapon = (WeaponBase) gameObject.AddComponent("" + newWeapon.weaponType);//weaponType enum name will match weapon script name
        }
        else
        {
            weapon = null;
        }
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
		return gameObject.transform.position + new Vector3(0,0.5f,0); //shift up vertically because eyes are above body center.
	}


    ////////////////Accessor Functions/////////////////
    
    public float Health
    {
        get {return health; }
        set {health = value; }
    }

	public int CraftingPoints
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
        get { return primaryAttackDamage; }
        set { primaryAttackDamage = value; }
    }


    public float AttackDelay
    {
        get { return primaryAttackDelay; }
        set { primaryAttackDelay = value; }
    }

    public float Armor
    {
        get { return armor; }
        set { armor = value; }
    }

    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }
    
    //////////////////////////////////
}   
