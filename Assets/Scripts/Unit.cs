using UnityEngine;
using System.Collections;

[RequireComponent(typeof (CharacterController))]


public class Unit : MonoBehaviour 
{
	protected float health = 100.0f;
    protected float maxHealth = 100.0f;
	protected float moveSpeed = 	10.0f;
	protected WeaponBase weapon;
    protected WeaponPickaxe pickaxe;

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
	virtual protected void Start () 
	{
		weapon = gameObject.GetComponent<WeaponBase>();

        //TODO player has a pickaxe and weapon equipped right now, change this so the pickaxe is a weapon
        pickaxe = gameObject.GetComponent<WeaponPickaxe>();
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
	
	virtual protected void killUnit()
	{

	}
}
