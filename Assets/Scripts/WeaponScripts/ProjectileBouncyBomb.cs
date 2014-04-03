using UnityEngine;
using System.Collections;

public class ProjectileBouncyBomb : MonoBehaviour {
	
	public float damage = 0;
	private int BOOMcount = 0;
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	 void OnCollisionEnter(Collision other)
    {
		BOOMcount++;
		Unit otherObject = other.gameObject.GetComponent<Unit>();
		
		if(otherObject != null && otherObject.GetType() == typeof(UnitEnemy))
		{
            otherObject.doDamage(damage);
			explode();

		}
		if( BOOMcount >= 3)
		{
			explode();
		}
    }
	
	void explode()
	{
		float radius = 1.0f;
		float power = 100.0f;
		//print ("BOOM!!!!!!!!");
		GameObject sparks = (GameObject)Instantiate(Resources.Load("FireballSparks"), transform.position, transform.rotation);

		Collider[] colliders = Physics.OverlapSphere (transform.position, radius);
		
		foreach ( Collider hit in colliders) {
				Unit toDie = hit.gameObject.GetComponent<Unit>();
                //print(toDie);

				if(toDie)
                	toDie.doDamage(damage);
                //print("Make The Little Man FLY!!!"); power += 1000;
				//print(hit.name);
				//hit.rigidbody.AddExplosionForce(power, transform.position, radius, 3.0f);
		}
		
		Destroy (gameObject);
		Destroy (sparks,0.25f);
	}
}
