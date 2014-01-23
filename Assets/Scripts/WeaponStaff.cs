using UnityEngine;
using System.Collections;


public class WeaponStaff : WeaponBase
{
	public GameObject Fireball;
	
	float bulletSpeed =  1000.0f;
	float specialSpeed = 2000.0f;
	
	// Use this for initialization
	protected override void Start ()
	{
		attackRange = 1000.0f;
		weaponDamage = 20.0f;
		attackDelay = 0.5f;
		base.Start();
		
		Fireball = (GameObject)Resources.Load("Fireball");
	}
	
	// Update is called once per frame
	protected override void Update ()
	{
		base.Update();
	}
	
	protected override void attackRoutine (Vector3 startPos, Vector3 faceDir)
	{
		 // Instantiate the projectile at the position and rotation of this transform
    	ProjectileFireball p;
    	GameObject clone = (GameObject)GameObject.Instantiate(Fireball, startPos,Character.getLookRotation());
		clone.gameObject.AddComponent("ProjectileFireball");
		Physics.IgnoreCollision(clone.collider,Camera.main.collider);
		Physics.IgnoreCollision(clone.collider,Character.collider);
		
		p = clone.GetComponent<ProjectileFireball>();
		p.damage = weaponDamage;
		
		
    	// Add force to the cloned object in the object's forward direction
    	clone.rigidbody.AddForce(clone.transform.forward * bulletSpeed);

		attack = false;
	}
	
	public override void attackSpecial ()
	{
		print("bouncebomb..");
		ProjectileBouncyBomb p;
    	GameObject clone = (GameObject)GameObject.Instantiate(Resources.Load("BouncingBomb"), Character.getEyePosition()+Vector3.down*0.25f,Character.getLookRotation());
		clone.gameObject.AddComponent("ProjectileBouncyBomb");
		Physics.IgnoreCollision(clone.collider,Camera.main.collider);
		Physics.IgnoreCollision(clone.collider,Character.collider);
		
		p = clone.GetComponent<ProjectileBouncyBomb>();
		p.damage = 5; //real damage later
		
    	// Add force to the cloned object in the object's forward direction
    	clone.rigidbody.AddForce(clone.transform.forward * specialSpeed);
	}
}
