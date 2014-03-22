using UnityEngine;
using System.Collections;

public class WeaponKey : WeaponBase
{
	//TODO Better way to override the base string?
	//TODO Instead of Using Strings, use an enum
	public override string strWeaponName {get{return "Key";}}
	public override string strWeaponType {get{return "WeaponKey";}}

    // Use this for initialization
    protected override void Start ()
    {
        attackRange = 5.0f;
        base.Start();
    }
    
    // Update is called once per frame
    protected override void Update ()
    {
    }
    
    protected override void attackRoutine (Vector3 startPos, Vector3 faceDir)	
    {
		RaycastHit rayHit;
		if(Physics.Raycast(startPos, faceDir, out rayHit, attackRange))
		{
			if(rayHit.collider.gameObject.CompareTag("Door"))
			{
				rayHit.collider.gameObject.GetComponent<DoorScript>().Open();
			}
		}
	}
}