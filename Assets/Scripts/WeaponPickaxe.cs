using UnityEngine;
using System.Collections;

public class WeaponPickaxe : WeaponBase
{
    
    // Use this for initialization
    protected override void Start ()
    {
        attackRange = 10f;
        weaponDamage = 20.0f;
        attackDelay = 1.5f;
        base.Start();
    }
    
    // Update is called once per frame
    protected override void Update ()
    {
        base.Update();
    }
    
    protected override void attackRoutine (Vector3 faceDir)
    {
        print("mining..");
//        LayerMask mask = LayerMask.NameToLayer("world");// | LayerMask.NameToLayer("enemy");

        //TODO The raycast doesn't seemm to hit if the player is above the ore
        if(Physics.Raycast(transform.position, faceDir, out rayHit, attackRange, 3<<8)) //layer mask looks at 'world' and 'enemy' layers only on raycast.
        {
            if(rayHit.collider.gameObject.CompareTag("Enemy"))
            {  
                print ("You can't attack enemies with a pickaxe!");
            }
            if(rayHit.collider.gameObject.CompareTag("Ore"))
            {
                //TODO Do some mining
                MineableBlock resource = rayHit.collider.GetComponent<MineableBlock>();
                resource.doDamage(1);

                print ("Mining for treasure...");
            }
        }
        attack = false;
    }
}