using UnityEngine;
using System.Collections;

public class BossUnit : UnitEnemy 
{
	public Transform enemy;

	//Every 5 seconds generate a new enemy.
	private float spawnTimer = 0.0f;

	//Caps the number of enemies that can be spawned to 3.
	private float enemyCap = 3.0f;

	//The number of enemies to spawn. 
	private int points = 5;

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
		if (Time.time >= spawnTimer) 
		{
			//Spawn an enemy every 5 seconds. 
			spawnTimer = Time.time + 10;
//			enemyCap++;

			foreach(GenerateEnemies.enemy e in GenerateEnemies.generateEnemies(5))
			{
				//Spawn an enemy.
				Instantiate(e, control.transform.position + Vector3.one, Quaternion.identity);
			}
		}


	}
}
