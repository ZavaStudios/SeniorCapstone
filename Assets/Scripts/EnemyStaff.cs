using UnityEngine;
using System.Collections;


public class EnemyStaff : WeaponBase
{
	public GameObject Fireball;
	
	float bulletSpeed =  1000.0f;
	private Collider enemyCollider;
	private SphereCollider enemySphere;
//	private int myLayer = 30;
	private Vector3 heightDifference = new Vector3(0, 0.3f, 0);
	
	// Use this for initialization
	protected override void Start ()
	{
//		gameObject.layer = myLayer;
//		Physics.IgnoreLayerCollision(30, 10, true);
		attackRange = 10.0f;
		weaponDamage = 20.0f;
		attackDelay = 0.5f;
		base.Start();
		
		enemyCollider = this.collider;
		enemySphere = this.GetComponent<SphereCollider>();
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
    	GameObject clone = (GameObject)GameObject.Instantiate(Fireball, startPos - heightDifference,Character.getLookRotation());
		clone.gameObject.AddComponent("ProjectileFireball");
		Physics.IgnoreCollision(clone.collider, enemyCollider);
		Physics.IgnoreCollision(clone.collider, enemySphere);
		
		p = clone.GetComponent<ProjectileFireball>();
		p.damage = weaponDamage;
		
		
    	// Add force to the cloned object in the object's forward direction
    	clone.rigidbody.AddForce(clone.transform.forward * bulletSpeed);

		attack = false;
	}
}
