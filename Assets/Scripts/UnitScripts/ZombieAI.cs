using UnityEngine;
using System.Collections;

/// <summary>
/// Zombie AI.
/// Defines how the zombie AI behaves. 
/// </summary>
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
        
		AttackDamage = 20.0f;
		AttackDelay = 4.0f;
        armor = 20;

        //Add the audio source for when this unit attacks
        attackSound = gameObject.AddComponent<AudioSource>();
        attackSound.clip = (AudioClip)Resources.Load("Sounds/Zombie");
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
		
		control.SimpleMove(dir * moveSpeed);
	}
}
