using UnityEngine;
using System.Collections;

public class WeaponStaffFiringScript : MonoBehaviour {
    

    private UnitPlayer Character;
    private Transform bulletOrigin;
    private const float bulletSpeed = 1000;

    // Use this for initialization
	void Start () 
    {
        bulletOrigin = transform.Find("BulletOrigin");
        Character = GameObject.FindGameObjectWithTag("Player").GetComponent<UnitPlayer>();
	}

    void fire()
    {
     // Instantiate the projectile at the position and rotation of this transform
    	ProjectileFireball p;
    	GameObject clone = (GameObject)GameObject.Instantiate(Resources.Load("Fireball"), bulletOrigin.position,Character.getLookRotation());
		p = (ProjectileFireball) clone.gameObject.AddComponent("ProjectileFireball");
		//Physics.IgnoreCollision(clone.collider,Character.collider);
		
		p.damage = Character.AttackDamage;
		
		// Add force to the cloned object in the object's forward direction
    	clone.rigidbody.AddForce(Character.getLookDirection() * bulletSpeed);
    }
}
