using UnityEngine;
using System.Collections;

public class WeaponPickaxe : WeaponBase
{
	//TODO Better way to override the base string?
	//TODO Instead of Using Strings, use an enum
	public override string strWeaponName {get{return "Pickaxe";}}
	public override string strWeaponType {get{return "WeaponPickaxe";}}

    // Use this for initialization
    protected override void Start ()
    {
        attackRange = 2.5f;
        weaponDamage = 20.0f;
        attackDelay = 1.2f;

        base.Start();
    }
    
    // Update is called once per frame
    protected override void Update ()
    {
        base.Update();
    }
    
    protected override void attackRoutine (Vector3 startPos, Vector3 faceDir)	
    {
//        print("mining..");
        //LayerMask mask = LayerMask.NameToLayer("world");// | LayerMask.NameToLayer("enemy");

        if(Physics.Raycast(startPos, faceDir, out rayHit, attackRange, 1<<10)) //layer mask looks at 'world' only on raycast.
        {
            if(rayHit.collider.gameObject.CompareTag("Enemy"))
            {  
                print ("You can't attack enemies with a pickaxe!");
            }
            if(rayHit.collider.gameObject.CompareTag("Ore"))
            {
                MineableBlock resource = rayHit.collider.GetComponent<MineableBlock>();

                WeaponPickaxeBlockDestroyerScript s = gameObject.GetComponentInChildren<WeaponPickaxeBlockDestroyerScript>();
                s.damage = 1;
                s.hitBlock = resource;

                print ("Mining for treasure...");
            }
        }
        
        attack = false;
    }
}