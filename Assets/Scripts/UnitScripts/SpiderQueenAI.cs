using UnityEngine;
using System.Collections;

public class SpiderQueenAI : BossUnit 
{
	public Transform spider;
	
	// Use this for initialization
	protected override void Start () 
	{
		base.Start();
		equipWeapon("SpiderWeapon");

		//The spider queen will spawn spiders more frequently than other bosses. 
		spawnTimer = 5;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update();
		
		if(health <= 75 && healthAt75 != true)
		{
			player.cantMove = true;
			healthAt75 = true;
		}
		else if(health <= 50 && healthAt50 != true)
		{
			player.cantMove = true;
			healthAt50 = true;
		}
		else if(health <= 25 && healthAt25 != true)
		{
			player.cantMove = true;
			healthAt25 = true;
		}
	}

	//Spawns enemies of the type spider. 
	protected override void spawnEnemy()
	{

		MonoBehaviour.Instantiate(spider,
		                          this.transform.position + new Vector3(3.0f, spider.collider.bounds.center.y, 0.0f),
		                          Quaternion.identity);
	}
}
