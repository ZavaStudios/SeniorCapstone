using UnityEngine;
using System.Collections;

public class WeaponPickaxeBlockDestroyerScript : MonoBehaviour {

    public Unit Character;
    public float damage;
    public float range;
    private int layer8_bitmask = 1 << 8; //0b10000000;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void damageObject()
    {

        RaycastHit rayHit;
        if(Physics.Raycast(Character.getEyePosition(), Character.getLookDirection(), out rayHit, range, layer8_bitmask))//world is on layer 8
        {
            if(rayHit.collider.gameObject.CompareTag("Ore"))
            {
                MineableBlock resource = rayHit.collider.GetComponent<MineableBlock>();
                resource.doDamage(damage);
            }
        }
        
    }
    
}
