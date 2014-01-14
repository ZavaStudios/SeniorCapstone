using UnityEngine;
using System.Collections;

public class WeaponPickaxe : WeaponBase
{
    
    // Use this for initialization
    protected override void Start ()
    {
        attackRange = 10f;
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
        print("mining..");
        //LayerMask mask = LayerMask.NameToLayer("world");// | LayerMask.NameToLayer("enemy");

        if(Physics.Raycast(startPos, faceDir, out rayHit, attackRange, 3<<8)) //layer mask looks at 'world' and 'enemy' layers only on raycast.
        {
            if(rayHit.collider.gameObject.CompareTag("Enemy"))
            {  
                print ("You can't attack enemies with a pickaxe!");
            }
            if(rayHit.collider.gameObject.CompareTag("Ore"))
            {
                MineableBlock resource = rayHit.collider.GetComponent<MineableBlock>();
                resource.doDamage(1);

                print ("Mining for treasure...");
            }
        }
        
        attack = false;
    }
}