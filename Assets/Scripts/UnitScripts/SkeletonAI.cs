﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Skeleton AI.
/// Defines how the skeleton AI behaves. 
/// </summary>
public class SkeletonAI : UnitEnemy
{
//	private float turnSpeed = 90;
	
	// Use this for initialization
	protected override void Start () 
	{
//		gameObject.layer = 30;
		equipWeapon("EnemyStaff");
		base.Start();
		moveSpeed = 7; //Moves the same speed as a player. 
		maxHealth = 50; // Half the health as a zombie and player.
		health = 50;
        AttackDamage = 20.0f;
		AttackDelay = 1.0f;

        //Add the audio source for when this unit attacks
        attackSound = gameObject.AddComponent<AudioSource>();
        attackSound.clip = (AudioClip)Resources.Load("Sounds/Fireball");
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
		
		
		//transform.Rotate(-90,0,0);

		control.SimpleMove(dir * moveSpeed);
	}
}
