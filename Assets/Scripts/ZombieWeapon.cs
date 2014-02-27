using UnityEngine;
using System.Collections;

public class ZombieWeapon : WeaponBase
{

	// Use this for initialization
	protected override void Start ()
	{
		attackRange = 2.5f;
		weaponDamage = 40.0f;
		attackDelay = 4.0f;
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update ()
	{

	}
		
	protected override void attackRoutine (Vector3 startPos, Vector3 faceDir)
	{
		print("attacking..");
		
		Transform Player = GameObject.FindGameObjectWithTag("Player").transform; 
			
		if(Physics.Raycast(transform.position - new Vector3(0, 0.5f, 0), faceDir, out rayHit, attackRange))
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
	}
}
