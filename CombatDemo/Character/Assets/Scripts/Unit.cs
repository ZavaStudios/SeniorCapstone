using UnityEngine;
using System.Collections;

[RequireComponent(typeof (CharacterController))]


public class Unit : MonoBehaviour 
{
	protected float health = 		100.0f;
	protected float moveSpeed = 	10.0f;
	protected WeaponBase weapon;
	
	virtual protected void Start () 
	{
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
	
	virtual protected void killUnit()
	{

	}
}
