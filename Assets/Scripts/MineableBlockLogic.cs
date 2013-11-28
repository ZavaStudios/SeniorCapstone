using UnityEngine;
using System.Collections;

public class MineableBlockLogic : MonoBehaviour
{
		public int _health = 4;
		public int _quantity = 2;

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (_health <= 0)
				{
						//TODO Destroy the block and send out an update
						DestroyBlock ();
						AddToPlayerInventory ();
				}
		}

		void MineAction (int damage)
		{
				_health -= damage;
		}

		void DestroyBlock ()
		{
				Destroy (gameObject);
		}

		void AddToPlayerInventory ()
		{
			GameObject inventory = GameObject.FindWithTag ("Player");
			inventory.transform.SendMessage ("AddToInventory", new Vector2(Random.value * 5.0f, 2.0f), SendMessageOptions.DontRequireReceiver);
		}
}
