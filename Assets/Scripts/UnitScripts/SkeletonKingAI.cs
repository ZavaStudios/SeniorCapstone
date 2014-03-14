using UnityEngine;
using System.Collections;

public class SkeletonKingAI : BossUnit
{
	public static Transform skeleton;
	private float invulTime;
	private bool invulTimeSet = false;
	
	// Use this for initialization
	protected override void Start () 
	{
		base.Start();
		equipWeapon("SkeletonWeapon");

		//Set the enemy cap to be 5.
		enemyCap = 3;
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
			}
			else if(invulTime < Time.time)
			{
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
			}
			else if(invulTime < Time.time)
			{
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
			}
			else if(invulTime < Time.time)
			{
				healthAt25 = true;
				invulTimeSet = false;
			}
			
		}
	}

	//Specifically spawns Skeletons for this boss.
	protected override void spawnEnemy()
	{
		MonoBehaviour.Instantiate(skeleton,
		                          this.transform.position + new Vector3(3.0f, skeleton.collider.bounds.center.y, 0.0f),
		                          Quaternion.identity);
	}
}
