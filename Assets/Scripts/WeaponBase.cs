using UnityEngine;
using System.Collections;

public class WeaponBase : MonoBehaviour
{
	//The name of the weapon base
	public virtual string strWeaponName {get{return "Default";}}
	public virtual string strWeaponType {get{return "Default";}}

	public Unit Character;
	
	protected RaycastHit rayHit;
		
	public float attackRange = 0f;
	public float weaponDamage = 0f;
	public float attackDelay = 2.0f; //default 2 second attack delay.
	
	public bool attack = false;
	
	private float nextDamageEvent = 0.0f;

	// Use this for initialization
	virtual protected void Start ()
	{
		Character = GetComponent<Unit>();
	}
	
	// Update is called once per frame
	virtual protected void Update ()
	{
		if (attack)
		{
			if (Time.time >= nextDamageEvent)
	        {
	            nextDamageEvent = Time.time + attackDelay;
       			attackRoutine(Character.getEyePosition(),Character.getLookDirection());
                Character.playAttackAnimation();
	        }
		}
		
	}
	
	virtual protected void attackRoutine(Vector3 startPos, Vector3 faceDir)
	{

	}
	
	virtual public void attackSpecial ()
	{
	}
}
