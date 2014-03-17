using UnityEngine;
using System.Collections;

public class PortalScript : MonoBehaviour
{
	public static Transform player;
	private BoxCollider portalCollider;

	// Use this for initialization
	void Start ()
	{
		portalCollider = gameObject.GetComponent<BoxCollider>();
		portalCollider.isTrigger = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void OnTriggerEnter(Collider other)
	{
		if(other.transform == player)
		{
			//Debug.Log("Win!");
			LevelHolder.Level++;	// We're going to the next level guys! :D
			Application.LoadLevel("mainGame");
		}
	}
}
