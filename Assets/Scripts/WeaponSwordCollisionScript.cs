using UnityEngine;
using System.Collections;

public class WeaponSwordCollisionScript : MonoBehaviour {


    public bool hitObject = false;
    public float damage = 0.0f;
	// Use this for initialization
	void Start () 
    {
        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        collider.isTrigger = true;
        print(collider.name);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (gameObject.animation["SwordSwing"].time <= 0.46)
        {

            if (gameObject.animation.isPlaying && hitObject == false)
            {
                print("WHEEE SWORD COLLISION!!!");
                print(gameObject.animation["SwordSwing"].time);
                hitObject = true;

                Unit otherObject = other.gameObject.GetComponent<Unit>();

                if (otherObject != null)
                {
                    if (otherObject.GetType() == typeof(UnitEnemy))
                    {

                        otherObject.doDamage(damage);
                    }
                }
            }

            gameObject.animation["SwordSwing"].speed = -0.25f;
        }
    }
}
