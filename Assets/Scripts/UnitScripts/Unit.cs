using UnityEngine;
using System.Collections;

[RequireComponent(typeof (CharacterController))]


public class Unit : MonoBehaviour 
{
    public bool vulnerable = true;

    protected float primaryAttackDamage;
    protected float primaryAttackDelay;

    protected float armor;
    protected float maxHealth = 100.0f; 
    protected float health = 100.0f;

	
    public float moveSpeed = 10.0f;

	public WeaponBase weapon;

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
	
    virtual protected void Start () 
	{
        health = maxHealth;
	}
	
	virtual protected void Update () 
	{
		
	}
	
    private const int armorDiminishingReturnThreshold = 100;
	
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
	
    virtual public void equipWeapon(ItemWeapon newWeapon)
    {

        
        GameObject.Destroy(weapon);
        
        if (newWeapon != null)
        {
            weapon = (WeaponBase) gameObject.AddComponent("" + newWeapon.weaponType);
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
		return gameObject.transform.position + new Vector3(0,0.5f,0); //eyes are on the top of the head? idk...
	}
}   
