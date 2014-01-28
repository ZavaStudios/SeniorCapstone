﻿using UnityEngine;
using System.Collections;

public class SpiderAI : UnitEnemy
{

	// Use this for initialization
	protected override void Start () 
	{
		base.Start();
		moveSpeed = 20; // Moves as fast as a player can sprint. 
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

	}
}
