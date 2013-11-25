using UnityEngine;
using System.Collections;

public class UnitPlayer : Unit {

	// Use this for initialization
	void Start () 
	{
		weapon = gameObject.GetComponent<WeaponBase>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		if(Input.GetKeyDown(KeyCode.Mouse0))
		{
			print ("mouse clicked....");
			if (weapon != null)
				weapon.attack = true;
		}
	
	}
}
