using UnityEngine;
using System.Collections;

public class ZombiePrinceAI : BossUnit
{
	private Transform zombie = GameObject.Find("zombie").transform;
	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
		equipWeapon("ZombieWeapon");

		//Cap the number of enemies to be 3.
		enemyCap = 3;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update();

		MonoBehaviour.Instantiate(zombie,
		                          this.transform.position + new Vector3(3.0f, zombie.collider.bounds.center.y, 0.0f),
		                          Quaternion.identity);
	}
}
