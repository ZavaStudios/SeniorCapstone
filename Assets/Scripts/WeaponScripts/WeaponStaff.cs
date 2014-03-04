using UnityEngine;
using System.Collections;


public class WeaponStaff : WeaponBase
{
	float bulletSpeed =  1000.0f;
	float specialSpeed = 2000.0f;
	
	// Use this for initialization
	protected override void Start ()
	{
		attackRange = 10.0f;
		weaponDamage = 20.0f;
		attackDelay = 0.5f;
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update ()
	{
	}
	
	protected override void attackRoutine (Vector3 startPos, Vector3 faceDir)
	{
		 // Instantiate the projectile at the position and rotation of this transform
    	ProjectileFireball p;
    	GameObject clone = (GameObject)GameObject.Instantiate(Resources.Load("Fireball"), startPos,Character.getLookRotation());
		clone.gameObject.AddComponent("ProjectileFireball");
		Physics.IgnoreCollision(clone.collider,Character.collider);
		
		p = clone.GetComponent<ProjectileFireball>();
		p.damage = weaponDamage;
		
		
    	// Add force to the cloned object in the object's forward direction
    	clone.rigidbody.AddForce(clone.transform.forward * bulletSpeed);

	}
	
	public override void attackSpecial ()
	{
        if(Physics.Raycast(Character.getEyePosition(), Character.getLookDirection(), out rayHit, attackRange))
        {
            print(rayHit.collider.gameObject);
            print("tag: " + rayHit.collider.gameObject.tag);
            if(rayHit.collider.gameObject.CompareTag("Floor"))
            {
                GameObject.Instantiate(Resources.Load("SnowburstTimer"), rayHit.point,Character.transform.rotation);
            }
        }
	}
}
