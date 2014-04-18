using UnityEngine;
using System.Collections;

public class ProjectileBouncyBomb : MonoBehaviour {
	
    public float damage = 0;
    private int BOOMcount = 0;
    
    private Transform Sparks;
    private Transform Projectile;

	// Use this for initialization
	void Start () 
	{
        Sparks = transform.Find("ImpactSparks");
        Projectile = transform.Find("Projectile");
        Sparks.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
    void OnCollisionEnter(Collision other)
    {
        BOOMcount++; //how many times the projectile has hit the ground. if boomcount == 3, projectile goes boom

        Unit otherObject = other.gameObject.GetComponent<Unit>();
		
        if(otherObject != null) //explode on impact if the other object was a unit
        {
            explode();
        }
        
        if( BOOMcount >= 3) //otherwise explode after 3 bounces
        {
            explode();
        }
    }
	
    //allbutworld mask>> all bits but bit 8->>> ...1111100000000
    int allLayersButWorldBitMask = ~(255);
    
    private void explode()
    {
        float radius = 1.0f;
        
        Collider[] colliders = Physics.OverlapSphere (transform.position, radius,allLayersButWorldBitMask);
		
        foreach ( Collider hit in colliders) 
        {
            Unit toDie = hit.gameObject.GetComponent<UnitEnemy>();
            if(toDie)
            {
                toDie.doDamage(damage);
            }				
        }
		
        Sparks.gameObject.SetActive(true);
        Projectile.gameObject.SetActive(false);
        Destroy (gameObject,0.5f);
    }
}
