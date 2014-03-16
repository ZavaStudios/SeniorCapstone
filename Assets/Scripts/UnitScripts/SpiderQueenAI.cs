using UnityEngine;
using System.Collections;


public class SpiderQueenAI : BossUnit 
{
	private float immobilizedTimer = 0;
	private bool immobilizedSet = false;
	
	
	// Use this for initialization
	protected override void Start () 
	{
		equipWeapon("SpiderWeapon");
		base.Start();
		
		
		//The spider queen will spawn spiders more frequently than other bosses. 
		spawnTimer = 5;
		enemyCap = 10;
		weapon.attackRange = 10;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update();
		
		if(health <= 75 && healthAt75 != true)
		{
			playerMoveSpeed = 0;
			
			
			if(!immobilizedSet)
			{
				immobilizedTimer = Time.time + 1f;
				immobilizedSet = true;
			}
			else if(immobilizedTimer < Time.time)
			{
				playerMoveSpeed = 10;
				immobilizedSet = false;
				healthAt75 = true;
			}
		}
		else if(health <= 50 && healthAt50 != true)
		{
			playerMoveSpeed = 0;
			
			
			if(!immobilizedSet)
			{
				immobilizedTimer = Time.time + 1f;
				immobilizedSet = true;
			}
			else if(immobilizedTimer < Time.time)
			{
				playerMoveSpeed = 10;
				immobilizedSet = false;
				healthAt50 = true;
			}
		}
		else if(health <= 25 && healthAt25 != true)
		{
			playerMoveSpeed = 0;
		
			
			if(!immobilizedSet)
			{
				immobilizedTimer = Time.time + 1f;
				immobilizedSet = true;
			}
			else if(immobilizedTimer < Time.time)
			{
				playerMoveSpeed = 10;
				immobilizedSet = false;
				healthAt25 = true;
			}
		}
	}

	//Spawns enemies of the type spider. 
	protected override Transform spawnEnemy()
	{
		return bossRoom.SpawnEnemy(EnemyGenerator.EnemyType.spider);
	}
}
