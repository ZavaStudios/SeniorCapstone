using UnityEngine;
using System.Collections;


public class ZombiePrinceAI : BossUnit
{	
	private float fearTimer;
	private bool fearTimerSet;
	
	// Use this for initialization
	protected override void Start ()
	{
		equipWeapon("ZombieWeapon");
		base.Start();

		//Cap the number of enemies to be 5.
		enemyCap = 1;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update();
		
		if(health <= 75 && healthAt75 != true)
		{
			playercc.enabled = false;
			
			if(!fearTimerSet)
			{
				fearTimer = Time.time + 1f;
				fearTimerSet = true;
			}
			else if(fearTimer < Time.time)
			{
				healthAt75 = true;
				fearTimerSet = false;
				playercc.enabled = true;
			}
			
		}
		else if(health <= 50 && healthAt50 != true)
		{
			playercc.enabled = false;
			
			if(!fearTimerSet)
			{
				fearTimer = Time.time + 1f;
				fearTimerSet = true;
			}
			else if(fearTimer < Time.time)
			{
				healthAt50 = true;
				fearTimerSet = false;
				playercc.enabled = true;				
			}
			
		}
		else if(health <= 25 && healthAt25 != true)
		{
			playercc.enabled = false;
			
			if(!fearTimerSet)
			{
				fearTimer = Time.time + 1f;
				fearTimerSet = true;
			}
			else if(fearTimer < Time.time)
			{
				healthAt25 = true;
				fearTimerSet = false;
				playercc.enabled = true;
			}
			
		}
	}

	//Only spawns zombies from the ZombiePrince. 
	protected override Transform spawnEnemy ()
	{
		return bossRoom.SpawnEnemy(EnemyGenerator.EnemyType.zombie);
	}
}
