using UnityEngine;
using System.Collections;

public class ZombieWeapon : WeaponBase
{
	public static Transform Player;
	
	// Use this for initialization
	protected override void Start ()
	{
        base.Start();
        attackRange = 2.5f;

        //Add the audio source for when this unit attacks
        attackSound = gameObject.AddComponent<AudioSource>();
        attackSound.clip = (AudioClip)Resources.Load("Sounds/Zombie");
	}
	
	// Update is called once per frame
	protected override void Update ()
	{

	}
		
	protected override void attackRoutine (Vector3 startPos, Vector3 faceDir)
	{
			
		if(Physics.Raycast(new Vector3(transform.position.x, Player.position.y, transform.position.z), faceDir, out rayHit, attackRange))
		{
			if(rayHit.collider.gameObject.CompareTag("Player"))
			{
				Unit enemy = rayHit.collider.GetComponent<Unit>();
				if(!enemy)
					print ("that is not a real enemy");
				else
					enemy.doDamage(Character.AttackDamage);
			}
		}
	}
}
