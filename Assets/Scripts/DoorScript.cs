using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour
{
	private float DOOR_CLOSE_DISTANCE = 5.0f;
	private bool isOpen = false;
	public static Transform player;

	// Use this for initialization
	void Start ()
	{
		isOpen = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// TODO
		if (isOpen && (transform.position - player.position).magnitude > DOOR_CLOSE_DISTANCE)
			Close();
	}
	
	public void Open()
	{
		if (!isOpen)
		{
			isOpen = true;
			animation.PlayQueued("doorDown");
		}
	}
	
	public void Close()
	{
		if (isOpen)
		{
			isOpen = false;
			animation.PlayQueued("doorUp");
		}
	}
}
