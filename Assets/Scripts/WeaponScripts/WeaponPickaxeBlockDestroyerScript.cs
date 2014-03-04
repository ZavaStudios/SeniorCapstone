using UnityEngine;
using System.Collections;

public class WeaponPickaxeBlockDestroyerScript : MonoBehaviour {

    public Unit Character;
    public float damage;
    public float range;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void damageObject()
    {
        RaycastHit rayHit;
        if(Physics.Raycast(Character.getEyePosition(), Character.getLookDirection(), out rayHit, range))
        {

            if(rayHit.collider.gameObject.CompareTag("Ore"))
            {
                MineableBlock resource = rayHit.collider.GetComponent<MineableBlock>();
                resource.doDamage(damage);
            }
        }
        
    }
    
}
