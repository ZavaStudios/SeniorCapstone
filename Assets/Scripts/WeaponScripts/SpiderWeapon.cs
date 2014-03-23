using UnityEngine;
using System.Collections;

public class SpiderWeapon : ZombieWeapon
{

	// Use this for initialization
	new void Start () 
	{
		base.Start();
		attackRange = 2;
	}
	
	// Update is called once per frame
	new void Update ()
	{
		base.Update();
	}
}
