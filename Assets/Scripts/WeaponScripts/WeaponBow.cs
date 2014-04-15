using UnityEngine;
using System.Collections;

public class WeaponBow : WeaponBase {

    public const float bulletSpeed =  1200.0f;
    private CharacterController capCollider;
    private WeaponBowFiringScript s ;
	// Use this for initialization
	protected override void Start () 
    {
        s = gameObject.GetComponentInChildren<WeaponBowFiringScript>();
        base.Start();
        capCollider = Character.GetComponent<CharacterController>();
        specialRange = 10;
        specialAttackSpeedRelative = 1.0f;
	}
	
    protected override void attackRoutine(Vector3 startPos, Vector3 faceDir)
    {
    }

    protected override void specialAttackRoutine ()
    {

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
	}
}
