using UnityEngine;
using System.Collections;


public class SkeletonKingAI : BossUnit
{
	private float disappTime;
	private bool disappTimeSet = false;
	
	// Use this for initialization
	protected override void Start () 
	{
		equipWeapon("EnemyStaff");
		base.Start();
		
	    AttackDelay = 0.33f;
        AttackDamage = 15;
		//Set the enemy cap to be 3.
		enemyCap = 5;
		weapon.attackRange = 20;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update();
		
		if(health <= 75 && healthAt75 != true)
		{
			healthBar.gameObject.SetActive(false);
			foreach(Transform child in transform.GetChild(0))
			{
				if(child.renderer)
					child.renderer.enabled = false;
			}
			
			if(!disappTimeSet)
			{
				disappTime = Time.time + 3;
				disappTimeSet = true;
			}
			else if(disappTime < Time.time)
			{
				foreach(Transform child in transform.GetChild(0))
				{
					if(child.renderer)
						child.renderer.enabled = true;
				}

				healthBar.gameObject.SetActive(true);
				healthAt75 = true;
				disappTimeSet = false;
			}
			
		}
		else if(health <= 50 && healthAt50 != true)
		{
			healthBar.gameObject.SetActive(false);
			foreach(Transform child in transform.GetChild(0))
			{
				if(child.renderer)
					child.renderer.enabled = false;
			}
			
			if(!disappTimeSet)
			{
				disappTime = Time.time + 3;
				disappTimeSet = true;
			}
			else if(disappTime < Time.time)
			{
				foreach(Transform child in transform.GetChild(0))
				{
					if(child.renderer)
						child.renderer.enabled = true;
				}

				healthBar.gameObject.SetActive(true);
				healthAt50 = true;
				disappTimeSet = false;
			}
			
		}
		else if(health <= 25 && healthAt25 != true)
		{
			healthBar.gameObject.SetActive(false);
			foreach(Transform child in transform.GetChild(0))
			{
				if(child.renderer)
					child.renderer.enabled = false;
			}
			
			if(!disappTimeSet)
			{
				disappTime = Time.time + 3;
				disappTimeSet = true;
			}
			else if(disappTime < Time.time)
			{
				foreach(Transform child in transform.GetChild(0))
				{
					if(child.renderer)
						child.renderer.enabled = true;
				}

				healthBar.gameObject.SetActive(true);
				healthAt25 = true;
				disappTimeSet = false;
			}
			
		}
	}

	//Specifically spawns Skeletons for this boss.
	protected override Transform spawnEnemy()
	{
		return bossRoom.SpawnEnemy(EnemyGenerator.EnemyType.skeleton);
	}
}
