using UnityEngine;
using System.Collections;

public class WeaponSword : WeaponBase
{
	//TODO Better way to override the base string?
	//TODO Instead of Using Strings, use an enum
	public override string strWeaponName {get{return "Sword";}}
	public override string strWeaponType {get{return "WeaponSword";}}
    WeaponSwordCollisionScript s;

	// Use this for initialization
	protected override void Start ()
	{
		attackRange = 2.5f;

		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update ()
	{
	}
	
	protected override void attackRoutine (Vector3 startPos, Vector3 faceDir)
	{
        s = gameObject.GetComponentInChildren<WeaponSwordCollisionScript>();
        s.damage = 10;//Character.AttackDamage;
        s.specialDamage = Character.AttackDamage + 100; //special gets moar
	}

    protected override void specialAttackRoutine ()
    {
        s = gameObject.GetComponentInChildren<WeaponSwordCollisionScript>();
        s.damage = 10;//Character.AttackDamage;
        s.specialDamage = Character.AttackDamage + 100; //special gets moar
        s.animation.PlayQueued("SpecialSwordSwing");
    }
}
