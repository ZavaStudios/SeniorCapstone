using UnityEngine;
using System.Collections;

public class ProjectileFireballEnemy : MonoBehaviour 
{
	
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
		print (other);
		//print("collision");
		if(hit == false)
		{
			Debug.Log (this.transform.parent);
			Unit otherObject = other.gameObject.GetComponent<UnitPlayer>();
			
			if(otherObject != null)
			{					
		        if(otherObject is UnitPlayer)
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
