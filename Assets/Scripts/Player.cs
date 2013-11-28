using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	// TODO: make this a real thing
	public int[] inventory;

	// Use this for initialization
	void Start ()
	{
		inventory = new int[5];
	}

	void AddToInventory (Vector2 parameters)
	{
		int item = (int)parameters.x;
		int qty = (int)parameters.y;
		inventory[item] += qty;
	}

	// Update is called once per frame
	void Update ()
	{

	}
}
