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

        WeaponPickaxeBlockDestroyerScript s = gameObject.GetComponentInChildren<WeaponPickaxeBlockDestroyerScript>();
        s.damage = 1;
        s.Character = Character;
        s.range = attackRange;

    }
    
    // Update is called once per frame
    protected override void Update ()
    {
    }
    
    protected override void attackRoutine (Vector3 startPos, Vector3 faceDir)	
    {
//        print("mining..");
        //LayerMask mask = LayerMask.NameToLayer("world");// | LayerMask.NameToLayer("enemy");
    }
}