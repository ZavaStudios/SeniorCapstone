using UnityEngine;
using System.Collections;

public class SnowburstTimer : MonoBehaviour {


    public float damage = 0.0f;
	// Use this for initialization
	void Start () 
    {
	
        StartCoroutine(explodeInSeconds(3));

	}
	

    private IEnumerator explodeInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);



        //allbutworld mask>> all bits but bit 8->>> ...1111100000000
        
        int allLayersButWorldBitMask = ~(255);//looking at all the blocks is expensive
        Collider[] colliders = Physics.OverlapSphere (transform.position, 15 ,allLayersButWorldBitMask);
		
        foreach ( Collider hit in colliders) 
        {
            Unit toDie = hit.gameObject.GetComponent<UnitEnemy>();
            if(toDie)
            {
                toDie.doDamage(damage);
            }				
        }

        GameObject sparks1 = (GameObject)Instantiate(Resources.Load("BigSnowBurst"), transform.position, transform.rotation);
        
        Destroy(sparks1,3);

    }
	// Update is called once per frame
	void Update () 
    {
	
	}
}
