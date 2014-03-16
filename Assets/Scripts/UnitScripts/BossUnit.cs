using UnityEngine;
using System.Collections;
using MazeGeneration;

public class BossUnit : UnitEnemy 
{
	protected bool healthAt25 = false;
	protected bool healthAt50 = false;
	protected bool healthAt75 = false;
	protected float playerMoveSpeed = player.GetComponent<UnitPlayer>().moveSpeed;
	protected Transform spawnedEnemy;
	public static GeneralRoom bossRoom;
	
	//Every 5 seconds generate a new enemy.
	protected float spawnTimer = 10.0f;
	protected float delay = 2.0f;

	//Caps the number of enemies that can be spawned.
	protected float enemyCap = 0.0f;
	private int numEnemies = 0;


	//ToDo: Once the enemies have been killed should we regenerate new ones? 

	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();


		//This is a boss, so lets make it a little harder ;) 
		Health = 100;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		//Behaves the same as a normal enemy. 
		base.Update();

		//First attack: Spawn an enemy every so often.
		if (Time.time >= spawnTimer && numEnemies < enemyCap)
		{
			//Spawn an enemy every delay seconds.
			spawnedEnemy = spawnEnemy();
			spawnedEnemy.GetComponent<UnitEnemy>().boss = this; 
			spawnTimer = Time.time + delay;
			numEnemies++;
		}
	}

	//Spawns an enemy in the room. This function needs to be overriden by super classes. 
	virtual protected Transform spawnEnemy()
	{
		return null;
	}

	public void decreaseEnemyCount()
	{
		numEnemies--;
		Debug.Log(numEnemies);
		if(numEnemies == 0 && !gameObject.activeSelf)
			Debug.Log("You Win!");
		spawnTimer = Time.time + delay;
	}
	
	protected override void killUnit()
	{
		gameObject.SetActive(false);
		playerMoveSpeed = 10;
		
		if(numEnemies == 0)	
			Debug.Log("You Win!");
		//base.killUnit();
	}
}
