using UnityEngine;
using System.Collections;

public class ZombiePrinceAI : BossUnit
{
	public Transform zombie; 


	// Use this for initialization
	protected override void Start ()
	{
		equipWeapon("ZombieWeapon");
		base.Start();

		//Cap the number of enemies to be 3.
		enemyCap = 5;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update();
	}

	//Only spawns zombies from the ZombiePrince. 
	protected override void spawnEnemy ()
	{

		MonoBehaviour.Instantiate(zombie,
		                          this.transform.position + new Vector3(3.0f, zombie.collider.bounds.center.y, 0.0f),
		                          Quaternion.identity);
	}
}
