﻿using UnityEngine;
using System.Collections;

public class UnitEnemy : Unit
{
	protected Transform Player;
	protected CharacterController control;
	protected Vector3 PlayerPosition;
	protected Vector3 dir;
	protected float distance;
	protected SphereCollider sphere;
	
	protected override void Start ()
	{

		Player = GameObject.FindGameObjectWithTag("Player").transform; 
		control = gameObject.GetComponent<CharacterController>();
		sphere = gameObject.GetComponent<SphereCollider>();
		sphere.isTrigger = true;
		sphere.radius = 10;
		moveSpeed = 5.0f;

        base.Start(); //gets reference to weapon, among other things.
	}
	
	//Is called when another collider hits the sphere collider.
	void OnTriggerStay(Collider other)
	{		
		//Player has walked into the sphere collider. 
		if(other.CompareTag("Player")&& weapon.attack != true)
		{
			//Move toward the player. 
			enemyMovement();			
		}
	}
	
	protected override void Update ()
	{	
		distance = Mathf.Abs(Player.position.z - transform.position.z);

		//Determine whether to attack or not.
		if(weapon && distance < weapon.attackRange)
		{
			weapon.attack = true;
       		animation.Play("idle");
			
		}
		//Makes sure that the zombie (construction worker mesh) is dropped in from the sky correctly. 
		control.SimpleMove(Vector3.zero);
		
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
