using UnityEngine;
using System.Collections;

public class WeaponSwordCollisionScript : MonoBehaviour {


    public bool hitObject = false;
    public float damage = 0.0f;
    public Collider c;

	// Use this for initialization
	void Start () 
    {
	}

	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnTriggerEnter(Collider other)
    {
        Unit otherObject = other.gameObject.GetComponent<Unit>();
        if (otherObject != null)
        {
		    if(otherObject is UnitEnemy)
            {
				//print ("found enemy and doing damage.");
                otherObject.doDamage(damage);
            }
        }
    }
}

