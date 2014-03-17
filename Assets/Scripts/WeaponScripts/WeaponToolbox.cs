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
		attackRange = 2.0f;
		weaponDamage = 20.0f;
		attackDelay = 2.0f;
        attackRange = 5;
        currentTurret = GameObject.FindGameObjectWithTag("PlayerTurret"); 
		base.Start();
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
                    enemy.doDamage(weaponDamage);
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
                
                currentTurret = (GameObject)GameObject.Instantiate(Resources.Load("Turret"), rayHit.point,Character.transform.rotation);
                return;
            }
        }

        nextSpecialAttack = 0.0f;

	}

}
