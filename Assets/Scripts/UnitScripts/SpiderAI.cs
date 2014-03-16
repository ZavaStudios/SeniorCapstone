using UnityEngine;
using System.Collections;

public class SpiderAI : UnitEnemy
{
	
//	float turnSpeed = 90;

	// Use this for initialization
	protected override void Start () 
	{
		equipWeapon("SpiderWeapon");
		base.Start();
		moveSpeed = 10; // Moves as fast as a player can sprint. 
		maxHealth = 10; //Very low life for a spider (one hit)
		health = 10;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update();
	}

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
