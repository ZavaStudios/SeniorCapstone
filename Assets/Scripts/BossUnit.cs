using UnityEngine;
using System.Collections;

public class BossUnit : UnitEnemy 
{
	public Transform enemy;

	//Every 5 seconds generate a new enemy.
	protected float spawnTimer = 5.0f;
	protected float delay = 0.0f;

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
		weapon.weaponDamage = 20;
		weapon.attackDelay = 4;
		weapon.attackRange = 5.0f;
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
			spawnTimer = Time.time + delay;
			numEnemies++;
		}
	}

	//Spawns an enemy in the room. This function needs to be overriden by super classes. 
	virtual protected void spawnEnemy()
	{

	}


}
