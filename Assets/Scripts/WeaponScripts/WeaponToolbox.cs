using UnityEngine;
using System.Collections;

public class WeaponToolbox : WeaponBase
{
	//TODO Better way to override the base string?
	//TODO Instead of Using Strings, use an enum
	public override string strWeaponName {get{return "Toolbox";}}
	public override string strWeaponType {get{return "WeaponToolbox";}}

    public GameObject currentTurret;

	// Use this for initialization
	protected override void Start ()
	{
        
		base.Start();
        attackRange = 5;
        currentTurret = GameObject.FindGameObjectWithTag("PlayerTurret"); 
        specialAttackDamageRelative = 1.2f;
	}
	
	// Update is called once per frame
	protected override void Update ()
	{

	}
	
	protected override void attackRoutine (Vector3 startPos, Vector3 faceDir)
	{
		//print("attacking..");
        
        if(Physics.Raycast(startPos, faceDir, out rayHit, attackRange))
        {
            if(rayHit.collider.gameObject.CompareTag("Enemy"))
            {
                Unit enemy = rayHit.collider.GetComponent<Unit>();
                if(!enemy)
                    print ("that is not a real enemy");
                else
                    enemy.doDamage(Character.AttackDamage);
            }
        }
        
	}


    protected override void specialAttackRoutine ()
	{

        
        if(Physics.Raycast(Character.getEyePosition(), Character.getLookDirection(), out rayHit, attackRange))
        {
            //print(rayHit.collider.gameObject);
            //print("tag: " + rayHit.collider.gameObject.tag);
            if(rayHit.collider.gameObject.CompareTag("Floor"))
            {

                if (currentTurret)
                {
                    Destroy(currentTurret);
                }
                
                UnitTurret t;
                currentTurret = (GameObject)GameObject.Instantiate(Resources.Load("Turret"), rayHit.point,Character.transform.rotation);
                t = currentTurret.GetComponent<UnitTurret>();
                t.AttackDamage = Character.AttackDamage * specialAttackDamageRelative;
                return;
            }
        }

        nextSpecialAttack = 0.0f;

	}

}
