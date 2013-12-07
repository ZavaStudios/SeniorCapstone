using UnityEngine;
using System.Collections;

public class MineableBlock : MonoBehaviour
{
		public float _health = 4;
		public int _quantity = 2;

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
			
		}

        public void doDamage(float damage)
		{
			_health -= damage;

            if (_health <= 0)
            {
                killUnit();
                AddToPlayerInventory ();
            }
		}

		void killUnit()
		{
		    Destroy (gameObject);
		}

		void AddToPlayerInventory ()
		{
			GameObject player = GameObject.FindWithTag ("Player");

            //TODO Cast to a Player type and access the inventory
            //player.inventory.add(ore, quantity)



            //inventory.transform.SendMessage ("AddToInventory", new Vector2(Random.value * 5.0f, 2.0f), SendMessageOptions.DontRequireReceiver);
			
		}
}
