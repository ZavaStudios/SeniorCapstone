using UnityEngine;
using System.Collections;

public class ProjectileBouncyBomb : MonoBehaviour {
	
	public float damage = 0;
	private bool hit = false;
	private int BOOMcount = 0;
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		hit = false;
	}
	
	 void OnCollisionEnter(Collision other)
    {
		print("Y U NO");
		BOOMcount++;
		print(BOOMcount);
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
		float radius = 50.0f;
		float power = 10.0f;
		print ("BOOM!!!!!!!!");
		GameObject sparks = (GameObject)Instantiate(Resources.Load("FireballSparks"), transform.position, transform.rotation);

		Collider[] colliders = Physics.OverlapSphere (transform.position, radius);
		
		foreach ( Collider hit in colliders) {
			if (hit && hit.rigidbody && hit.rigidbody != this.rigidbody)
			{
				print("Make The Little Man FLY!!!"); power += 1000;
				print(hit.name);
				hit.rigidbody.AddExplosionForce(power, transform.position, radius, 3.0f);
			}
		}
		
		Destroy (gameObject);
		Destroy (sparks,0.25f);
	}
}
