using UnityEngine;
using System.Collections;

public class ProjectileFireball : MonoBehaviour 
{
	
	public float damage = 0;
	private bool hit = false;
    private Transform Sparks;
    private Transform Projectile;
	// Use this for initialization
	void Start () 
	{
		SphereCollider collider = gameObject.GetComponent<SphereCollider>();
		collider.isTrigger = true;
        Sparks = transform.Find("ImpactSparks");
        Projectile = transform.Find("Projectile");
        Sparks.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
	
	 void OnTriggerEnter(Collider other)
    {
//		print (other);
		//print("collision");
		if(hit == false)
		{
			Unit otherObject = other.gameObject.GetComponent<Unit>();
			
			if(otherObject != null)
			{	
				otherObject.doDamage(damage);
	            
			}
			explode();
		    
	        rigidbody.velocity = Vector3.zero;
		}
		hit = true;
		
    }
	
	void explode()
	{
		Sparks.gameObject.SetActive(true);
        Projectile.gameObject.SetActive(false);
        Destroy (gameObject,0.5f);
		
	}
}
