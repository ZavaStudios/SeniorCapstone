using UnityEngine;
using System.Collections;


/// <summary>
/// A key weapon and how it can be used. Attack this to keys.
/// </summary>
public class WeaponKey : WeaponBase
{
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