using UnityEngine;
using System.Collections;


public class WeaponStaff : WeaponBase
{
	float bulletSpeed =  1000.0f;
		
	// Use this for initialization
	protected override void Start ()
	{
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
		p = (ProjectileFireball) clone.gameObject.AddComponent("ProjectileFireball");
		Physics.IgnoreCollision(clone.collider,Character.collider);
		
		p.damage = Character.AttackDamage;
		
		
    	// Add force to the cloned object in the object's forward direction
    	clone.rigidbody.AddForce(clone.transform.forward * bulletSpeed);

	}
	
	protected override void specialAttackRoutine ()
	{
        if(Physics.Raycast(Character.getEyePosition(), Character.getLookDirection(), out rayHit, attackRange))
        {
            //print(rayHit.collider.gameObject);
            //print("tag: " + rayHit.collider.gameObject.tag);
            if(rayHit.collider.gameObject.CompareTag("Floor"))
            {
                SnowburstTimer t;
                GameObject snowburst = (GameObject) GameObject.Instantiate(Resources.Load("SnowburstTimer"), rayHit.point,Character.transform.rotation);
                t = snowburst.GetComponent<SnowburstTimer>();
                t.damage = Character.AttackDamage * specialAttackDamageRelative;
                return;
            }
        }

        nextSpecialAttack = 0.0f;

	}
}
