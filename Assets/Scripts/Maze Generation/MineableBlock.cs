using UnityEngine;
using System.Collections;
using MazeGeneration;

public class MineableBlock : MonoBehaviour
{
    public float _maxHealth = 4;
    public float _health = 4;
	public int _quantity = 2;
	public Cube _cube;

    public static Texture2D[] crackTextures;

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
            AddToPlayerInventory();
        }

        else
        {
            int index = (int)(crackTextures.Length * Mathf.Max(((_maxHealth - _health) / _maxHealth), 0.0f));
            Debug.Log("((_maxHealth - _health) / _maxHealth): " + ((_maxHealth - _health) / _maxHealth));
            renderer.material.SetTexture("_DecalTex", crackTextures[index]);
        }
	}

	void killUnit()
	{
		//_cube.Parent.DestroyCube(_cube);
        // Surface level parent should be a rogue room, so assume:
        renderer.material.SetTexture("_DecalTex", crackTextures[0]);
		((RogueRoom)_cube.Parent).DestroyMineableBlock(this);
		if (_cube.Type != ItemBase.tOreType.Stone && _cube.Type != ItemBase.tOreType.NOT_ORE)
            Inventory.getInstance().inventoryAddItem(new ItemOre(_cube.Type));
	    //Destroy (gameObject);
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
