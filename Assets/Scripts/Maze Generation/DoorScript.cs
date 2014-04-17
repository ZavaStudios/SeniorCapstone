using UnityEngine;
using System.Collections;

/// <summary>
/// Script handling locked door for the Boss Room. Determines when the door should slide
/// down and slide back up. Expects an animation for each of these actions to also be
/// attached to the prefab.
/// </summary>
public class DoorScript : MonoBehaviour
{
    // Distance the player must stray from the door before it closes itself again.
	private float DOOR_CLOSE_DISTANCE = 25.0f;
    // Holds whether or not the door is presently open.
	private bool isOpen = false;
    // Player of the game, so we can track their distance from the door.
	public static Transform player;

	void Start ()
	{
		isOpen = false;
	}
	
	void Update ()
	{
		if (isOpen && (transform.position - player.position).sqrMagnitude > DOOR_CLOSE_DISTANCE)
			Close();
	}
	
    /// <summary>
    /// Called when the door to the boss room should be opened. Does nothing
    /// if the door is already open.
    /// </summary>
	public void Open()
	{
		if (!isOpen)
		{
			isOpen = true;
			animation.PlayQueued("doorDown");
		}
	}
	
    /// <summary>
    /// Called when the door to the boss room should be closed. Does nothing
    /// if the door is already closed.
    /// </summary>
	public void Close()
	{
		if (isOpen)
		{
			isOpen = false;
			animation.PlayQueued("doorUp");
		}
	}
}
