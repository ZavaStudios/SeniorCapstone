using UnityEngine;
using System.Collections;

public class SpiderQueenAI : BossUnit 
{
	private Transform spider = GameObject.Find("spider").transform;
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

		MonoBehaviour.Instantiate(spider,
		                          this.transform.position + new Vector3(3.0f, spider.collider.bounds.center.y, 0.0f),
		                          Quaternion.identity);
	}

	//Spawns enemies of the type spider. 
	protected override void spawnEnemy()
	{

	}
}
