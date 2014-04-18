using UnityEngine;
using System.Collections;




public class WeaponTurretAI : WeaponBase {

	public GameObject currentTarget;
    public float projectileSpeed = 1750;
    public bool shouldRotate;
    private const float burstDelay = 0.2f;
    private const float targetingDelay = 0.5f;

	// Use this for initialization
	protected override void Start () 
    {
        base.Start();
        attackRange = 15;
    }
	

    private GameObject getClosestEnemyInSight()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); 
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
                if(rayHit.collider.gameObject.CompareTag("Enemy"))
                {
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



    private IEnumerator firePojectileBurst()
    {
        int count = 3;
     
        float damage = Character.AttackDamage * specialAttackDamageRelative / count;
            
        yield return new WaitForSeconds(targetingDelay); //waiting first gives turret a second to move + target before shot is fired.

        for(int i = 0; i < count; i++)
        {

            ProjectileBouncyBomb p;
    	    GameObject clone = (GameObject)GameObject.Instantiate(Resources.Load("BouncingBomb", typeof(GameObject)), Character.getEyePosition(),Character.getLookRotation());
		    Physics.IgnoreCollision(clone.collider,Character.collider);
		
		    p = clone.GetComponent<ProjectileBouncyBomb>();
		    p.damage = damage;
		
    	    // Add force to the cloned object in the object's forward direction
    	    clone.rigidbody.AddForce(clone.transform.forward * projectileSpeed);

            
            yield return new WaitForSeconds(burstDelay); //waiting here gives turret a second to retarget before next shot, and sets the attack delay
            
        }
        shouldRotate = false;
    }


    protected override void attackRoutine(Vector3 startPos, Vector3 faceDir)
	{        
        if(currentTarget == null)
        {
            currentTarget = getClosestEnemyInSight();
            if (currentTarget == null)
            {
                return;
            }
        }
                
        Vector3 enemyPosition = currentTarget.transform.position;

        //perform a raycast to check if line of sight obstructed && if in range:
        if (Vector3.Distance(transform.position, enemyPosition) <= attackRange)
        {

            shouldRotate = true;
            StartCoroutine(firePojectileBurst());
            return;
        }
        currentTarget = null; //if the thing hit wasn't the current target, start attack routine again next frame and get a new target.
        
	}
}
