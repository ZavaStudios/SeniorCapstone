using UnityEngine;
using System.Collections;


public class EnemyStaff : WeaponBase
{
	public GameObject Fireball;
	public static Transform player;
	
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
		attackRange = 5.0f;

		base.Start();
		
		enemyCollider = this.collider;
		Fireball = (GameObject)Resources.Load("FireballEnemy");
	}
	
	// Update is called once per frame
	protected override void Update ()
	{
		
	}
	
	protected override void attackRoutine (Vector3 startPos, Vector3 faceDir)
	{
		 // Instantiate the projectile at the position and rotation of this transform
    	ProjectileFireball p;
    	GameObject clone = (GameObject)GameObject.Instantiate(Fireball, 
			new Vector3(startPos.x, player.position.y, startPos.z), Character.getLookRotation());
		p = (ProjectileFireball)clone.gameObject.AddComponent("ProjectileFireball");
		Physics.IgnoreCollision(clone.collider, enemyCollider);
		
		p.damage = Character.AttackDamage;
		
		
    	// Add force to the cloned object in the object's forward direction
    	clone.rigidbody.AddForce(clone.transform.forward * bulletSpeed);

	}
}
