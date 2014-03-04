using UnityEngine;
using System.Collections;

public class SnowburstTimer : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
	
        StartCoroutine(explodeInSeconds(3));

	}
	

    private IEnumerator explodeInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); 
        foreach (GameObject enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < 30)
            {
                enemy.gameObject.GetComponent<Unit>().doDamage(25);
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
