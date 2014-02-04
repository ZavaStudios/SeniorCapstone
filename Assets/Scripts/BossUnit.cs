using UnityEngine;
using System.Collections;

public class BossUnit : UnitEnemy 
{
	public Transform enemy;

	//Every 5 seconds generate a new enemy.
	private float spawnTimer = 0.0f;

	//Caps the number of enemies that can be spawned to 3.
	private float enemyCap = 0.0f;

	//ToDo: Once the enemies have been killed should we regenerate new ones? 

	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();


		//This is a boss, so lets make it a little harder ;) 
		Health = 50;
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
		if (Time.time >= spawnTimer && enemyCap < 3) 
		{
			spawnTimer += Time.time + 5;
			enemyCap++;

			//Spawn an enemy.
			Instantiate(enemy, control.transform.position + Vector3.one, Quaternion.identity);
		}


	}
}
