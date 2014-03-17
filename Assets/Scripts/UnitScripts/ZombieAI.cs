using UnityEngine;
using System.Collections;

public class ZombieAI : UnitEnemy
{
//	float turnSpeed = 90;
	
	// Use this for initialization
	protected override void Start () 
	{
		//Choose which weapon to give the Zombie. This must occur before the call to base.Start().
		equipWeapon("ZombieWeapon"); 
		base.Start(); //Call the base class to set up the basic enemy and unit class.
		maxHealth = 100; //Set the health of the zombie.
		health = 100;
		moveSpeed = 5; //Move half as fast as the player. 
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update(); //Makes the Zombie behave like a normal enemy.
	}

	//Tells the UnitEnemy class how the zombie movement should behave.
	protected override void enemyMovement()
	{
		//move
		PlayerPosition = player.position;
		dir = PlayerPosition - transform.position;
		dir.y = transform.position.y;
		dir.Normalize();
		
//		float angleToTarget = Mathf.Atan2((PlayerPosition.x - transform.position.x), (PlayerPosition.z - transform.position.z)) * Mathf.Rad2Deg;
//		transform.eulerAngles = new Vector3(0, Mathf.MoveTowardsAngle(transform.eulerAngles.y, angleToTarget, Time.deltaTime * turnSpeed), 0);
		//transform.Rotate(-90,0,0);

		control.SimpleMove(dir * moveSpeed);
	}
}
