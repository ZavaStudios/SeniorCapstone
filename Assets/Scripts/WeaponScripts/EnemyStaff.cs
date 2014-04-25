using UnityEngine;
using System.Collections;


public class EnemyStaff : WeaponBase
{
	public static Transform player;
	
	float bulletSpeed =  1000.0f;
	private Collider enemyCollider;
	private SphereCollider enemySphere;
	
	// Use this for initialization
	protected override void Start ()
	{
		attackRange = 5.0f;

		base.Start();
		
		enemyCollider = this.collider;

        //Add the audio source for when this unit attacks
        attackSound = gameObject.AddComponent<AudioSource>();
        attackSound.clip = (AudioClip)Resources.Load("Sounds/Fireball");
	}
	
	protected override void attackRoutine (Vector3 startPos, Vector3 faceDir)
	{
		 // Instantiate the projectile at the position and rotation of this transform
    	ProjectileFireball p;
    	GameObject clone = (GameObject)GameObject.Instantiate(Resources.Load("FireballEnemy"), new Vector3(startPos.x, player.position.y, startPos.z), Character.getLookRotation());
		p = (ProjectileFireball)clone.gameObject.AddComponent("ProjectileFireball");
		Physics.IgnoreCollision(clone.collider, enemyCollider);
		
		p.damage = Character.AttackDamage;
		
		
    	// Add force to the cloned object in the object's forward direction
    	clone.rigidbody.AddForce(clone.transform.forward * bulletSpeed);

	}
}
