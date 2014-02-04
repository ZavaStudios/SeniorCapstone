using UnityEngine;
using System.Collections;

public class UnitEnemy : Unit
{
	protected Transform Player;
	protected CharacterController control;
	protected Vector3 PlayerPosition;
	protected Vector3 dir;
	protected float distance;
	
	protected override void Start ()
	{

		Player = GameObject.FindGameObjectWithTag("Player").transform; 
		control = gameObject.GetComponent<CharacterController>();
		moveSpeed = 5.0f;

        base.Start(); //gets reference to weapon, among other things.
	}
	
	protected override void Update ()
	{	
		PlayerPosition = Player.position;
		dir = PlayerPosition - transform.position;
		distance = dir.sqrMagnitude;

		//Determine whether to attack or not.
		if(weapon && distance < weapon.attackRange)
		{
			weapon.attack = true;
            animation.Play("idle");
		}
        //If the player is within a certain distance then execute move code
		else if(distance < 700f)
		{
			enemyMovement();		
		}
		else //Doesn't move the enemy, but applies the physics affects of SimpleMove on the enemy.
		{
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
		print ("Ow you kilt meh");
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
