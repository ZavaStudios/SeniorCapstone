using UnityEngine;
using System.Collections;

public class SkeletonKingAI : BossUnit
{
	private Transform skeleton = GameObject.Find("skeleton").transform;
	// Use this for initialization
	protected override void Start () 
	{
		base.Start();
		equipWeapon("SkeletonWeapon");

		//Set the enemy cap to be 5.
		enemyCap = 5;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update();
		MonoBehaviour.Instantiate(skeleton,
		                          this.transform.position + new Vector3(3.0f, skeleton.collider.bounds.center.y, 0.0f),
		                          Quaternion.identity);
	}
}
