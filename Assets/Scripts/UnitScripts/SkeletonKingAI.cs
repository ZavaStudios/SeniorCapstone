using UnityEngine;
using System.Collections;


public class SkeletonKingAI : BossUnit
{
	private float invulTime;
	private bool invulTimeSet = false;
	
	// Use this for initialization
	protected override void Start () 
	{
		equipWeapon("EnemyStaff");
		base.Start();
		
	    AttackDelay = 0.33f;
        AttackDamage = 15;
		//Set the enemy cap to be 3.
		enemyCap = 1;
		weapon.attackRange = 20;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update();
		
		if(health <= 75 && healthAt75 != true)
		{
			//Make the boss invulnerable (unable to take damage), and invisible.
			vulnerable = false;
			transform.renderer.enabled = false;
			
			if(!invulTimeSet)
			{
				invulTime = Time.time + 3;
				invulTimeSet = true;
			}
			else if(invulTime < Time.time)
			{
				vulnerable = true;
				transform.renderer.enabled = true;
				healthAt75 = true;
				invulTimeSet = false;
			}
			
		}
		else if(health <= 50 && healthAt50 != true)
		{
			vulnerable = false;
			transform.renderer.enabled = false;
			
			if(!invulTimeSet)
			{
				invulTime = Time.time + 3;
				invulTimeSet = true;
			}
			else if(invulTime < Time.time)
			{
				vulnerable = true;
				transform.renderer.enabled = true;
				healthAt50 = true;
				invulTimeSet = false;
			}
			
		}
		else if(health <= 25 && healthAt25 != true)
		{
			vulnerable = false;
			transform.renderer.enabled = false;
			
			if(!invulTimeSet)
			{
				invulTime = Time.time + 3;
				invulTimeSet = true;
			}
			else if(invulTime < Time.time)
			{
				vulnerable = true;
				transform.renderer.enabled = true;
				healthAt25 = true;
				invulTimeSet = false;
			}
			
		}
	}

	//Specifically spawns Skeletons for this boss.
	protected override Transform spawnEnemy()
	{
		return bossRoom.SpawnEnemy(EnemyGenerator.EnemyType.skeleton);
	}
}
