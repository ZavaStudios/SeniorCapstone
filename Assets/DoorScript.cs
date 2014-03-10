using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour
{
	public Transform player;
	private bool isOpen = false;

	// Use this for initialization
	void Start ()
	{
		isOpen = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (isOpen && (transform.position - player.position).magnitude > 5.0f)
			Close();
	}
	
	public void Open()
	{
		if (!isOpen)
		{
			isOpen = true;
			animation.PlayQueued("DoorDown");
		}
	}
	
	public void Close()
	{
		if (isOpen)
		{
			isOpen = false;
			animation.PlayQueued("DoorUp");
		}
	}
}
