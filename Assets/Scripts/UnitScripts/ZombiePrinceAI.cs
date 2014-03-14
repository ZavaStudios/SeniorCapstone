using UnityEngine;
using System.Collections;
using MazeGeneration;

public class ZombiePrinceAI : BossUnit
{
    public static Transform zombie;
    public static GeneralRoom bossRoom;
	
	private float fearTimer;
	private bool fearTimerSet;
	
	// Use this for initialization
	protected override void Start ()
	{
		equipWeapon("ZombieWeapon");
		base.Start();

		//Cap the number of enemies to be 3.
		enemyCap = 5;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update();
		
		if(health <= 75 && healthAt75 != true)
		{
			player.cantMove = true;
			
			if(!fearTimerSet)
			{
				fearTimer = Time.time + 3;
			}
			else if(fearTimer < Time.time)
			{
				healthAt75 = true;
				fearTimerSet = false;
			}
			
			//Force the player to walk toward the boss.
			player.GetComponent<CharacterController>().SimpleMove(transform.position);
		}
		else if(health <= 50 && healthAt50 != true)
		{
			player.cantMove = true;
			
			if(!fearTimerSet)
			{
				fearTimer = Time.time + 3;
			}
			else if(fearTimer < Time.time)
			{
				healthAt50 = true;
				fearTimerSet = false;
			}
			
			player.GetComponent<CharacterController>().SimpleMove(transform.position);
		}
		else if(health <= 25 && healthAt25 != true)
		{
			player.cantMove = true;
			
			if(!fearTimerSet)
			{
				fearTimer = Time.time + 3;
			}
			else if(fearTimer < Time.time)
			{
				healthAt25 = true;
				fearTimerSet = false;
			}
			
			player.GetComponent<CharacterController>().SimpleMove(transform.position);
		}
	}

	//Only spawns zombies from the ZombiePrince. 
	protected override void spawnEnemy ()
	{

		MonoBehaviour.Instantiate(zombie,
		                          this.transform.position + new Vector3(3.0f, zombie.collider.bounds.center.y, 0.0f),
		                          Quaternion.identity);
	}
}
