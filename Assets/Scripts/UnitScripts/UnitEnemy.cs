using UnityEngine;
using System.Collections;

public class UnitEnemy : Unit
{
	public static Transform player;
	protected CharacterController control;
	protected Vector3 PlayerPosition;
	protected Vector3 dir;
	protected float distance;
	float turnSpeed = 120;
//	protected SphereCollider sphere;
	public BossUnit boss = null;
	
	protected override void Start ()
	{
		moveSpeed = 5.0f;
		control = GetComponent<CharacterController>();
        base.Start(); //gets reference to weapon, among other things.
	}
	
	protected override void Update ()
	{	
		distance = Vector3.Distance(transform.position, player.position);
		PlayerPosition = player.position;
		dir = PlayerPosition - transform.position;
		dir.y = transform.position.y;
		dir.Normalize();
		
		//Determine whether to attack or not.
		if(weapon && (distance <= weapon.attackRange))
		{
			weapon.attack();
			float angleToTarget = Mathf.Atan2((PlayerPosition.x - transform.position.x), (PlayerPosition.z - transform.position.z)) * Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3(0, Mathf.MoveTowardsAngle(transform.eulerAngles.y, angleToTarget, Time.deltaTime * turnSpeed), 0);
		}
		else if(distance <= 15.0f || (health != maxHealth && distance <= 25))
		{
			float angleToTarget = Mathf.Atan2((PlayerPosition.x - transform.position.x), (PlayerPosition.z - transform.position.z)) * Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3(0, Mathf.MoveTowardsAngle(transform.eulerAngles.y, angleToTarget, Time.deltaTime * turnSpeed), 0);
			//transform.Rotate(-90,0,0);
	
			control.SimpleMove(dir * moveSpeed);
			
//			enemyMovement();
		}
		else
		{
			//Makes sure that the zombie (construction worker mesh) is dropped in from the sky correctly. 
			control.SimpleMove(Vector3.zero);
		}
	}
		
	//A method for how the enemy should behave with their movement.
	//This method should be overridden by the super class.
	protected virtual void enemyMovement()
	{

	}
	
	//Kills the unit by removing the enemy from the screen and give credit to the player.
	protected override void killUnit ()
	{
		//Decrement boss's count of spawned enemies if the boss spawned you.
		if(boss != null)
		{
			boss.decreaseEnemyCount();
		}
		
		//print ("Ow you kilt meh");
		Destroy (gameObject);

		// Increment player's score
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		player.transform.SendMessage ("incrementScore", 1);
		
		
	}

	public override Vector3 getLookDirection()
	{
		return base.getLookDirection();
	}
	
	public override Vector3 getEyePosition()
	{
		return base.getEyePosition();
	}
}
