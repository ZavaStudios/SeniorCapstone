using UnityEngine;
using System.Collections;


public class WeaponStaff : WeaponBase
{
	// Use this for initialization
	protected override void Start ()
	{
        attackRange = 10.0f;
		base.Start();
	}
		
	protected override void attackRoutine (Vector3 startPos, Vector3 faceDir)
	{
        //Deprecated. This functionality is handled by animation function calls...
	}
	
	protected override void specialAttackRoutine ()
	{
        if(Physics.Raycast(Character.getEyePosition(), Character.getLookDirection(), out rayHit, attackRange))
        {
            if(rayHit.collider.gameObject.CompareTag("Floor"))
            {
                SnowburstTimer t;
                GameObject snowburst = (GameObject) GameObject.Instantiate(Resources.Load("SnowburstTimer"), rayHit.point,Character.transform.rotation);
                t = snowburst.GetComponent<SnowburstTimer>();
                t.damage = Character.AttackDamage * specialAttackDamageRelative;
                return;
            }
        }

        nextSpecialAttack = 0.0f;

	}
}
