using UnityEngine;
using System.Collections;

public class WeaponSword : WeaponBase
{

	// Use this for initialization
	protected override void Start ()
	{
		attackRange = 5f;
		weaponDamage = 20.0f;
		attackDelay = 1.5f;
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

		if(Physics.Raycast(startPos, faceDir, out rayHit, attackRange,3<<8)) //layer mask looks at 'world' and 'enemy' layers only on raycast.
		{
			if(rayHit.collider.gameObject.CompareTag("Enemy"))
			{
				Unit enemy = rayHit.collider.GetComponent<Unit>();
				if(!enemy)
					print ("that is not a real enemy");
				else
					enemy.doDamage(weaponDamage);
			}
			if(rayHit.collider.gameObject.CompareTag("Ore"))
			{
				print ("You can't mine with a sword");
			}
		}
		attack = false;
	}
}
