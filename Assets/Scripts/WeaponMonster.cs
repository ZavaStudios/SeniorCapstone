using UnityEngine;
using System.Collections;

public class WeaponMonster : WeaponBase
{
    public override string strWeaponName {get{return "Pickaxe";}}
    public override string strWeaponType {get{return "WeaponPickaxe";}}

	// Use this for initialization
	protected override void Start ()
	{
		attackRange = 2.5f;
		weaponDamage = 10.0f;
		attackDelay = 2.0f;
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update ()
	{
		base.Update();
	}
	
	protected override void attackRoutine (Vector3 startPos, Vector3 faceDir)
	{
		print("attacking..");
		
		Transform Player = GameObject.FindGameObjectWithTag("Player").transform; 
		Vector3 PlayerPosition = Player.position;
		Vector3 dir = PlayerPosition - transform.position;
		
		if(Physics.Raycast(transform.position, dir, out rayHit, attackRange)) //layer mask looks at 'world' and 'enemy' layers only on raycast.
		{
			print ("Raycast hit");
			if(rayHit.collider.gameObject.CompareTag("Player"))
			{
				Unit enemy = rayHit.collider.GetComponent<Unit>();
				if(!enemy)
					print ("that is not a real enemy");
				else
					enemy.doDamage(weaponDamage);
			}
		}
		attack = false;
	}
}
