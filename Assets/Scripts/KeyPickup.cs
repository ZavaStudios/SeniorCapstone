using UnityEngine;

public class KeyPickup : MonoBehaviour
{
	private const float PICKUP_DISTANCE = 1.0f;
	public static Transform player;
	private BoxCollider pickupCollider;
	
	public void Start()
	{
		pickupCollider = gameObject.GetComponent<BoxCollider>();
		pickupCollider.isTrigger = true;
	}

	public void Update()
	{
		// TODO: better
		if (Physics.Raycast(transform.position, player.position, PICKUP_DISTANCE))
		{
			Inventory.getInstance().inventoryAddItem(new ItemKey("It's a key! :D"));
			Destroy(gameObject);
		}
	}

	public void OnTriggerEnter(Collider other)
	{

	}
}

