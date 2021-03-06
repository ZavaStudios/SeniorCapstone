﻿using UnityEngine;
using System.Collections;

public class UnitEnemy : Unit
{
	Transform Player;
	CharacterController control;
	
	float turnSpeed = 90;
	
	protected override void Start ()
	{
		base.Start();
		Player = GameObject.FindGameObjectWithTag("Player").transform; 
		control = gameObject.GetComponent<CharacterController>();
		moveSpeed = 5.0f;
		weapon.attackRange = 2.5f;
		weapon.weaponDamage = 10.0f;

	}
	
	protected override  void Update ()
	{
		//move
		Vector3 PlayerPosition = Player.position;
		Vector3 dir = PlayerPosition - transform.position;
		float distance = dir.sqrMagnitude;
		dir.y = transform.position.y;
		dir.Normalize();
		
		float angleToTarget = Mathf.Atan2((PlayerPosition.x - transform.position.x), (PlayerPosition.z - transform.position.z)) * Mathf.Rad2Deg;
		transform.eulerAngles = new Vector3(0, Mathf.MoveTowardsAngle(transform.eulerAngles.y, angleToTarget, Time.deltaTime * turnSpeed), 0);
		transform.Rotate(-90,0,0);
	
		control.SimpleMove (dir * moveSpeed);		
	
		if(weapon && distance < weapon.attackRange)
		{
			weapon.attack = true;
		}
		
	}
		
	protected override void killUnit ()
	{
		print ("Ow you kilt meh");
		Destroy (gameObject);
	}
}
