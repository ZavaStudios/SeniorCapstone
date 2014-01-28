﻿using UnityEngine;
using System.Collections;

public class ProjectileFireball : MonoBehaviour {
	
	public float damage = 0;
	private bool hit = false;
	// Use this for initialization
	void Start () 
	{
	
		SphereCollider collider = gameObject.GetComponent<SphereCollider>();
		collider.isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
	
	 void OnTriggerEnter(Collider other)
    {
		print("collision");
		if(hit == false)
		{
			
			Unit otherObject = other.gameObject.GetComponent<Unit>();
			
			if(otherObject != null)
			{
		        if(otherObject.GetType() == typeof(UnitEnemy))
		        {
	                otherObject.doDamage(damage);
	                explode();
		        }
			}
			else
			{
				explode();
			}
		}
		hit = true;
		
    }
	
	void explode()
	{
		GameObject sparks = (GameObject)Instantiate(Resources.Load("FireballSparks"), transform.position, transform.rotation);
		Destroy (gameObject);
		Destroy (sparks,0.5f);
		
	}
}
