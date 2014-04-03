using UnityEngine;
using System.Collections;

public class UnitEnemy : Unit
{
	public static Transform player;
	public static Transform healthBarStatic;
	protected Transform healthBar;
	protected CharacterController control;
	protected Vector3 PlayerPosition;
	protected Vector3 dir;
    protected GameObject floatingDamageText;
    protected FloatingDamageText floatingDamageTextScript;
	protected float distance;
	float turnSpeed = 120;
	private float healthLost = 0;

	private float healthBarLength;
//	protected SphereCollider sphere;
	public BossUnit boss = null;
	
	protected override void Start ()
	{
		moveSpeed = 5.0f;
		control = GetComponent<CharacterController>();
        base.Start(); //gets reference to weapon, among other things.
		healthBar = (Transform) GameObject.Instantiate(healthBarStatic, transform.position, Quaternion.identity);
        floatingDamageText = (GameObject) GameObject.Instantiate(Resources.Load("FloatingDamageText"), transform.position, Quaternion.identity);
        floatingDamageTextScript = floatingDamageText.GetComponent<FloatingDamageText>();
        floatingDamageTextScript.parent = transform;
	}
	
	protected override void Update ()
	{	
		distance = Vector3.Distance(transform.position, player.position);
		PlayerPosition = player.position;
		dir = PlayerPosition - transform.position;
		dir.y = transform.position.y;
		dir.Normalize();
		
		//Determine whether to attack or not.
		if(weapon && (distance <= weapon.attackRange))
		{
			weapon.attack();
			float angleToTarget = Mathf.Atan2((PlayerPosition.x - transform.position.x), (PlayerPosition.z - transform.position.z)) * Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3(0, Mathf.MoveTowardsAngle(transform.eulerAngles.y, angleToTarget, Time.deltaTime * turnSpeed), 0);
		}
		else if(distance <= 15.0f || (health != maxHealth && distance <= 25))
		{
			float angleToTarget = Mathf.Atan2((PlayerPosition.x - transform.position.x), (PlayerPosition.z - transform.position.z)) * Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3(0, Mathf.MoveTowardsAngle(transform.eulerAngles.y, angleToTarget, Time.deltaTime * turnSpeed), 0);
			//transform.Rotate(-90,0,0);
	
			control.SimpleMove(dir * moveSpeed);
			
//			enemyMovement();
		}
		else
		{
			//Makes sure that enemies are dropped into the maze correctly.  
			control.SimpleMove(Vector3.zero);
		}

		healthBar.position = transform.position + new Vector3(0, transform.position.y, 0);
		healthBar.rotation = Camera.main.transform.rotation;
		healthLost = health / maxHealth;
		healthBar.localScale = new Vector3(healthLost, 1, 1);
	}
		
	//A method for how the enemy should behave with their movement.
	//This method should be overridden by the super class.
	protected virtual void enemyMovement()
	{

	}
	
    public void equipWeapon(string newWeapon)
    {
        GameObject.Destroy(weapon);
        
        if (newWeapon != null)
        {
            weapon = (WeaponBase) gameObject.AddComponent(newWeapon);
        }
        else
        {
            weapon = null;
        }
    }

    public override void doDamage(float damage)
    {
        base.doDamage(damage);
        floatingDamageTextScript.startText(damage.ToString());
    }

	//Kills the unit by removing the enemy from the screen and give credit to the player.
	protected override void killUnit ()
	{
		//Decrement boss's count of spawned enemies if the boss spawned you.
		if(boss != null)
		{
			boss.decreaseEnemyCount();
		}
		
		//print ("Ow you kilt meh");
		Destroy (gameObject);
		Destroy(healthBar.gameObject);

		// Increment player's score
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		player.transform.SendMessage ("incrementScore", 1);
		
		
	}

	public override Vector3 getLookDirection()
	{
		return base.getLookDirection();
	}
	
	public override Vector3 getEyePosition()
	{
		return base.getEyePosition();
	}
}
