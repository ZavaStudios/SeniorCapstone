using UnityEngine;
using System.Collections;

/// <summary>
/// A pickaxe used for mining. Equip this on the pickaxe prefab for functionality.
/// </summary>
public class WeaponPickaxe : WeaponBase
{
	public override string strWeaponName {get{return "Pickaxe";}}
	public override string strWeaponType {get{return "WeaponPickaxe";}}

    // Use this for initialization
    protected override void Start ()
    {
        attackRange = 2.5f;

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
       //attack routine is handled by animation events on the pickaxe. This way we can remove ore when the pickaxe 'strikes.'
       //see WeaponPickaxeBlockDestroyerScript
    }
}