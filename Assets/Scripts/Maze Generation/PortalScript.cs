using UnityEngine;
using System.Collections;

/// <summary>
/// Manages the end-of-level portal, checking for when the player collides
/// with it and sending them off to the next level.
/// </summary>
public class PortalScript : MonoBehaviour
{
    // Instance of the player, so we can check it collided with our collision box.
	public static Transform player;

	void Start ()
	{
		BoxCollider portalCollider = gameObject.GetComponent<BoxCollider>();
		portalCollider.isTrigger = true;
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
