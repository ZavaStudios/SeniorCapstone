using UnityEngine;
using System.Collections;

public class WeaponPickaxeBlockDestroyerScript : MonoBehaviour {

    public MineableBlock hitBlock;
    public float damage;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void damageObject()
    {
        if (hitBlock)
            hitBlock.doDamage(damage);
    }
    
}
