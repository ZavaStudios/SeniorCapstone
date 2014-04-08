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
        specialAttackDamageRelative = 1.75f;
		base.Start();
        s = gameObject.GetComponentInChildren<WeaponSwordCollisionScript>();
	}
	
	// Update is called once per frame
	protected override void Update ()
	{
	}
	
	protected override void attackRoutine (Vector3 startPos, Vector3 faceDir)
	{
        s.damage = Character.AttackDamage;//Character.AttackDamage;
	}

    protected override void specialAttackRoutine ()
    {
        s.damage = Character.AttackDamage * specialAttackDamageRelative; //special gets moar
        s.animation.PlayQueued("SpecialSwordSwing");
    }
}
