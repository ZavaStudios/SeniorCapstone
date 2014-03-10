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
        attackRange = 10.0f;
		weaponDamage = 20.0f;
        attackDelay = 1.2f;

        base.Start();
    }
    
    // Update is called once per frame
    protected override void Update ()
    {
    }
    
    protected override void attackRoutine (Vector3 startPos, Vector3 faceDir)	
    {
		Debug.Log ("startPos: "+ startPos + " | faceDir: " + faceDir);
		RaycastHit rayHit;
		if(Physics.Raycast(startPos, faceDir, out rayHit, attackRange))
		{
			Debug.Log ("hit");
			if(rayHit.collider.gameObject.CompareTag("Door"))
			{
				Debug.Log ("isDoor");
				rayHit.collider.gameObject.animation.PlayQueued("doorDown");
			}
		}
	}
}