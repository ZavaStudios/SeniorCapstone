using UnityEngine;

/// <summary>
/// Script managing when the key should be picked up by the player. Should only be attached
/// to the prefab which is generated in the maze for pickup, not all key prefabs.
/// </summary>
public class KeyPickup : MonoBehaviour
{
    // Player of the game, so we can track when they collide with our object
	public static Transform player;
	
	public void Start()
	{
        BoxCollider pickupCollider = gameObject.GetComponent<BoxCollider>();
		pickupCollider.isTrigger = true;
	}

	public void OnTriggerEnter(Collider other)
	{
        if (other.transform == player)
        {
            Inventory.getInstance().inventoryAddItem(new ItemKey("Key"));
            Destroy(gameObject);
        }
	}
}

