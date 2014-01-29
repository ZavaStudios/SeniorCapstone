using UnityEngine;
using System.Collections;

public class SpiderWeapon : ZombieWeapon
{

	// Use this for initialization
	void Start () 
	{
		base.Start();
		attackRange = 1;
		weaponDamage = 10;
		attackDelay = 0.5f;
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		base.Update();
	}
}
