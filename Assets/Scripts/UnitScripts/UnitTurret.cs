using UnityEngine;
using System.Collections;

public class UnitTurret : Unit {

    private Transform bulletOrigin;
    protected Vector3 dir;
    WeaponTurretAI tWeapon;
	Transform weaponModel;
    private float turnSpeed = 150f;

    protected override void Start ()
	{
        base.Start();

       
        tWeapon = (WeaponTurretAI) gameObject.AddComponent("WeaponTurretAI");
        weapon = tWeapon;

        bulletOrigin = transform.Find("Weapon/BulletOrigin");
        
        weaponModel = transform.Find("Weapon");

    }
    
	protected override void Update ()
	{	
        if (tWeapon.shouldRotate)    
        {
            Vector3 enemyPosition = tWeapon.currentTarget.transform.position;
            float angleToTarget = Mathf.Atan2((enemyPosition.x - transform.position.x), (enemyPosition.z - transform.position.z)) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, Mathf.MoveTowardsAngle(transform.eulerAngles.y, angleToTarget, Time.deltaTime * turnSpeed), 0);           
            
            float rotationY = Mathf.Clamp (Vector3.Distance(transform.position, enemyPosition)*4, 0, 70);
            weaponModel.localEulerAngles = new Vector3(-rotationY,0,0);
        }
    }

    //Kills the unit by removing the enemy from the screen and give credit to the player.
    protected override void killUnit ()
    {
        //print ("Ow you kilt meh");
        Destroy (gameObject);
    }

    public override Vector3 getLookDirection()
    {
        return bulletOrigin.forward;
    }
	
    public override Vector3 getEyePosition()
    {
        return bulletOrigin.position;
    }

    public override Quaternion getLookRotation()
	{
		return bulletOrigin.rotation;
	}
}