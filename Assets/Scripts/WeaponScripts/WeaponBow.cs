using UnityEngine;
using System.Collections;

public class WeaponBow : WeaponBase {

    public float bulletSpeed =  1200.0f;
    private CharacterController capCollider;
    private WeaponBowFiringScript s ;
	// Use this for initialization
	void Start () 
    {
        s = gameObject.GetComponentInChildren<WeaponBowFiringScript>();
        base.Start();
        capCollider = Character.GetComponent<CharacterController>();
        specialRange = 10;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    protected override void attackRoutine(Vector3 startPos, Vector3 faceDir)
    {
    }

    protected override void releaseRoutine()
	{   
        s.weaponBow = this;
        s.releaseShot();        
	}


    public override void attackSpecial ()
    {
        //print ("ZOOM");


        //Vector3 p1 = transform.position + capCollider.center + Vector3.up * -(capCollider.height*0.5f - capCollider.radius);
        //Vector3 p2 = p1 + Vector3.up * (capCollider.height - capCollider.radius);

        Vector3 p1 = transform.position + capCollider.center + Vector3.up * -(capCollider.height*0.5f - capCollider.radius);
		Vector3 p2 = transform.position + capCollider.center + Vector3.up *  (capCollider.height*0.5f - capCollider.radius);
        
        float distance = specialRange;

		// Cast character controller shape 10 meters forward, to see if it is about to hit anything
		if (Physics.CapsuleCast (p1, p2, capCollider.radius, Character.getLookDirection(), out rayHit, specialRange, 1<<8 )) 
        {
            //print (rayHit.transform.gameObject);
            
			distance = rayHit.distance;
        }

        Vector3 distanceVector = Character.getLookDirection() * distance;
        Character.transform.position = new Vector3(Character.transform.position.x + distanceVector.x, Mathf.Clamp(Character.transform.position.y + distanceVector.y,capCollider.height*0.6f,100),Character.transform.position.z + distanceVector.z);
        //if(Physics.Raycast(Character.getEyePosition(), Character.getLookDirection(), out rayHit, specialRange,1<<8)) //layer 8 = WORLD, look at world only, no enemies. Dont care bout dem.
        //{
        //    Character.transform.position = rayHit.point - Character.getLookDirection() * 2 ;
        //}
        //else
        //{
        //    Character.transform.position += Character.getLookDirection()*specialRange;
        //}
	}
}
