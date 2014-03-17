using UnityEngine;
using System.Collections;

public class WeaponSwordCollisionScript : MonoBehaviour {


    public bool hitObject = false;
    public float damage = 0.0f;
    public float specialDamage = 0.0f;
    private BoxCollider collider;
    private TrailRenderer trail;

	// Use this for initialization
	void Start () 
    {
        collider = gameObject.GetComponent<BoxCollider>();
        trail = gameObject.GetComponentInChildren<TrailRenderer>();
               
        collider.isTrigger = true;
        trail.enabled = false;
	}

	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        Unit otherObject = other.gameObject.GetComponent<Unit>();
        float damageToDo;
        if (animation["SwordSwing"].time > 0)
        {
            damageToDo = damage;
        }
        else
        {
            damageToDo = specialDamage;
        }


        if (otherObject != null)
        {
		    if(otherObject is UnitEnemy)
            {
				//print ("found enemy and doing damage.");
                otherObject.doDamage(damageToDo);
            }
        }
    }

    void collisionEnable()
    {
        collider.enabled = true;
        trail.enabled= true;
    }

    void collisionDisable()
    {
        collider.enabled = false;
        trail.enabled = false;
    }
}

