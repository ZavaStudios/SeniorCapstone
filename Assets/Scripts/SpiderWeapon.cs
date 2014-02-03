using UnityEngine;
using System.Collections;

public class SpiderWeapon : ZombieWeapon
{

	// Use this for initialization
	new void Start () 
	{
		base.Start();
		attackRange = 1;
		weaponDamage = 1;
		attackDelay = 0.5f;
		
	}
	
	// Update is called once per frame
	new void Update ()
	{
		base.Update();
	}
}
