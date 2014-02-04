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
	}

	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
		print (other);
        if (gameObject.animation["SwordSwing"].time <= 0.46)
        {
			print ("first if");
            if (gameObject.animation.isPlaying && hitObject == false)
            {
				print ("second if");
                hitObject = true;

                Unit otherObject = other.gameObject.GetComponent<Unit>();

                if (otherObject != null)
                {
					print ("third if");
					print (otherObject);
					
                   	if(otherObject is UnitEnemy)
                    {
						print ("found enemy and doing damage.");
                        otherObject.doDamage(damage);
                    }
                }
            }

            gameObject.animation["SwordSwing"].speed = -0.25f;
        }
    }
}
