using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Unit))]

public class WeaponBase : MonoBehaviour
{
	public Unit Character;
	
	protected RaycastHit rayHit;
		
	protected float attackRange = 0f;
	
	public bool attack = false;
	
	// Use this for initialization
	void Start ()
	{
		Character = GetComponent<Unit>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
		Vector3 faceDir = Character.transform.forward; //get direction character is facing.
		
		if (attack)
		{
			
			if(Physics.Raycast(transform.position, faceDir, out rayHit, attackRange,3<<8)) //layer mask looks at 'world' and 'enemy' layers only on raycast.
			{
				if(rayHit.collider.gameObject.CompareTag("Enemy"))
				{
					print ("hit enemy");
					Debug.DrawRay(transform.position,Character.transform.forward, Color.red);
				}
			}
			attack = false;
		}
		
	}
}
