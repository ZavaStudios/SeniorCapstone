using UnityEngine;
using System.Collections;


/// <summary>
/// Zombie prince AI.
/// Defines the behavior for the Zombie Prince Boss. 
/// Spawns enemies and also has special abilities. 
/// </summary>
public class ZombiePrinceAI : BossUnit
{	
	private float fearTimer;
	private bool fearTimerSet;
	private System.Random rand; 
	
	// Use this for initialization
	protected override void Start ()
	{
		equipWeapon("ZombieWeapon");
		base.Start();

		//Cap the number of enemies to be 5.
		enemyCap = 3;
		rand = new System.Random();
        AttackDamage = 50;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update();
		
		if(health <= 75 && healthAt75 != true)
		{
			//playercc.enabled = false;
			
			if(!fearTimerSet)
			{
				fearTimer = Time.time + 1f;
				fearTimerSet = true;
				player.position = Vector3.Lerp(PlayerPosition, transform.position, Time.deltaTime * 25);   
			}
			else if(fearTimer < Time.time)
			{
				healthAt75 = true;
				fearTimerSet = false;
				//playercc.enabled = true;
			}
			
		}
		else if(health <= 50 && healthAt50 != true)
		{
			//playercc.enabled = false;

			if(!fearTimerSet)
			{
				fearTimer = Time.time + 1f;
				fearTimerSet = true;
				player.position = Vector3.Lerp(PlayerPosition, transform.position, Time.deltaTime * 25); 
			}
			else if(fearTimer < Time.time)
			{
				healthAt50 = true;
				fearTimerSet = false;
				//playercc.enabled = true;		
			}
			
		}
		else if(health <= 25 && healthAt25 != true)
		{
			//playercc.enabled = false;
			
			if(!fearTimerSet)
			{
				fearTimer = Time.time + 1f;
				fearTimerSet = true;
				player.position = Vector3.Lerp(PlayerPosition, transform.position, Time.deltaTime * 25);    
			}
			else if(fearTimer < Time.time)
			{
				healthAt25 = true;
				fearTimerSet = false;
				//playercc.enabled = true;
			}
			
		}
	}

	//Only spawns zombies from the ZombiePrince. 
	protected override Transform spawnEnemy ()
	{
		return bossRoom.SpawnEnemy(EnemyGenerator.EnemyType.zombie);
	}
}
