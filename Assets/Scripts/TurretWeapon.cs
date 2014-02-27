using UnityEngine;
using System.Collections;




public class TurretWeapon : WeaponBase {

	public GameObject currentTarget;
    public float projectileSpeed = 100;
    Transform weaponModel;

	// Use this for initialization
	protected override void Start () 
    {
        base.Start();
        attackRange = 10;
        attackDelay = 0.75f;
    }
	

    private GameObject getClosestEnemyInSight()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player"); 
	    GameObject closestValidTarget = null;
        float closestDistance = attackRange;
        foreach (GameObject enemy in enemies)
        {
            Vector3 enemyPosition = enemy.transform.position;
            Vector3 direction = enemyPosition - Character.transform.position;
            RaycastHit rayHit;
            
            //perform a raycast to check if line of sight obstructed && if out of range && to find closest enemy:
            if(Physics.Raycast(Character.transform.position, direction, out rayHit, closestDistance))
            {
                if(rayHit.collider.gameObject.CompareTag("Player"))
                {
                    print("got enemy");
                    closestDistance = Vector3.Distance(transform.position, enemyPosition);
                    closestValidTarget = enemy;
                }
            }
        }
        return closestValidTarget;

    }

	// Update is called once per frame
	protected override void Update () 
    {
	    attack();
	}

    protected override void attackRoutine(Vector3 startPos, Vector3 faceDir)
	{        
        if(currentTarget == null)
        {
            print ("ERRUH");
            currentTarget = getClosestEnemyInSight();
            if (currentTarget == null)
            {
                return;
            }

        }
                
        Vector3 enemyPosition = currentTarget.transform.position;
        Vector3 direction = enemyPosition - Character.getEyePosition();
        RaycastHit rayHit;

        ////perform a raycast to check if line of sight obstructed && if in range:
        //if(Physics.Raycast(Character.getEyePosition(), direction, out rayHit, attackRange))
        //{
        //    if(rayHit.collider.gameObject == currentTarget)
        //    {

        if (Vector3.Distance(transform.position, enemyPosition)<=attackRange)
        {

                print("bouncebomb..");
		        ProjectileBouncyBomb p;
    	        GameObject clone = (GameObject)GameObject.Instantiate(Resources.Load("BouncingBomb", typeof(GameObject)), Character.getEyePosition(),Character.getLookRotation());
		        clone.gameObject.AddComponent("ProjectileBouncyBomb");
		        Physics.IgnoreCollision(clone.collider,Camera.main.collider);
		        Physics.IgnoreCollision(clone.collider,Character.collider);
		
		        p = clone.GetComponent<ProjectileBouncyBomb>();
		        p.damage = 5; //real damage later
		
    	        // Add force to the cloned object in the object's forward direction
    	        clone.rigidbody.AddForce(clone.transform.forward * 1750);

                return;
        }

        //    }
        //}
        currentTarget = null; //if the thing hit wasn't the current target, start attack routine again next frame and get a new target.
        
	}
}
