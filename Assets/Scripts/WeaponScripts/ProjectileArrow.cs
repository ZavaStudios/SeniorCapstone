using UnityEngine;
using System.Collections;

public class ProjectileArrow : MonoBehaviour {
	
	public float damage = 0;
	// Use this for initialization
	void Start () 
	{
	
		BoxCollider collider = gameObject.GetComponent<BoxCollider>();
		collider.isTrigger = true;
        
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
	
	 void OnTriggerEnter(Collider other)
    {
        //print (other);
        Unit otherUnit = other.GetComponent<Unit>();
        
        //print (other.transform);
        rigidbody.isKinematic = true; // stop physics
        
        //print (otherUnit);
       			
        if(otherUnit != null)
        {
            gameObject.transform.parent = other.transform.parent;
            otherUnit.doDamage(damage);
            Destroy (gameObject,7); // destroy after 7 seconds
        }
        else
        {
            Destroy (gameObject,7);
        }

		
    }
}
