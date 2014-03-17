using UnityEngine;
using System.Collections;
using MazeGeneration;

public class MineableBlock : MonoBehaviour
{
	public float _health = 4;
	public int _quantity = 2;
	public Cube _cube;

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
		_cube.Parent.DestroyCube(_cube);
        if (_cube.Type != ItemBase.tOreType.Stone && _cube.Type != ItemBase.tOreType.NOT_ORE)
            Inventory.getInstance().inventoryAddItem(new ItemOre(_cube.Type));
	    Destroy (gameObject);
	}

	void AddToPlayerInventory ()
	{
//			GameObject player = GameObject.FindWithTag ("Player");

    //TODO Cast to a Player type and access the inventory
//		Inventory i = Inventory.getInstance ();
//    	player.inventory.add(ore, quantity)



    //inventory.transform.SendMessage ("AddToInventory", new Vector2(Random.value * 5.0f, 2.0f), SendMessageOptions.DontRequireReceiver);
		
	}
}
