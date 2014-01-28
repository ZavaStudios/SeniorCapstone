using UnityEngine;
using System.Collections;

public class SkeletonAI : UnitEnemy
{

	// Use this for initialization
	protected override void Start () 
	{
		base.Start();
		moveSpeed = 10; //Moves the same speed as a player. 
		maxHealth = 50; // Half the health as a zombie and player.
		health = 50;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		
	}

	protected override void enemyMovement()
	{

	}
}
