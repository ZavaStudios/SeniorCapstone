using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour 
{
	//Declare public constants
	public const KeyCode keyCodeInventory = KeyCode.I;

	//Declare instance variables
	private bool boolInventory = false;
	Unit player;
	UnitPlayer unitPlayer;

	private int intSelectedWeapon = 0;

	protected void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();
		unitPlayer = player.GetComponent<UnitPlayer>();
	}

	protected void Update()
	{
		//Allow toggling of the inventory when the corresponding button has been pressed
		if(Input.GetKeyUp(keyCodeInventory))
		{
			boolInventory = !boolInventory;
		}
	}


    void OnGUI () 
    {
        // Make a health bar
        GUI.Box(new Rect(10,10,100,30), player.Health + "/" + player.MaxHealth);
		GUI.Box (new Rect (10, 40, 100, 30), "Score: " + player.Score);

		// TODO Show the Inventory and disable other controls such as the crosshair and attacking
		if(boolInventory)
		{
			layoutInventoryGrid();
			//TODO Continually switching weapons pushes the models off screen
			//TODO Add some textures

//			if(GUI.Button (new Rect (0, 0, 100, 100), "Equip Pickaxe"))
//			{
//				//Equip the pickaxe
//				unitPlayer.equipWeapon("WeaponPickaxe");
//			}
//
//			if( GUI.Button (new Rect (100, 100, 100, 100), "Equip Sword"))
//			{
//				unitPlayer.equipWeapon("WeaponSword");
//			}
		}
    }

	private void layoutInventoryGrid()
	{
		//Declare the options in laying out the grid
		//The measurements for the item slot representation
		int intSlotWidth = 100;
		int intSlotHeight = 100;
		//The starting position of the inventory
		Vector2 vec2CurrentPos = new Vector2 (0, 100);


		//Find the inventory
		Inventory inventory = ((Unit)(GameObject.FindGameObjectWithTag ("Player").GetComponent<Unit>())).inventory;

		//How many items can fit on each row/column
		int intItemsPerRow = Screen.width / intSlotWidth;
		int intItemsPerCol = Screen.height / intSlotHeight;

		//TODO Allow scrolling down when weapons don't all fit on the screen


		ArrayList arrListWeapons = inventory.getInventoryWeapons();
		string[] arrStringWeapons = new string[arrListWeapons.Count];

		for(int i = 0; i < arrListWeapons.Count; i++)
		{
			arrStringWeapons[i] = arrListWeapons[i].ToString();
		}

		intSelectedWeapon = GUI.SelectionGrid (new Rect (vec2CurrentPos.x, vec2CurrentPos.y, Screen.width, intSlotHeight),
		                                      intSelectedWeapon, arrStringWeapons, intItemsPerRow);
		unitPlayer.equipWeapon (arrStringWeapons [intSelectedWeapon]);


		//TODO Uncomment this out if we want to use this
		//Loop through all the items and lay them out simply
//		foreach(string weapon in arrListWeapons)
//		{
//			if(GUI.Button(new Rect(vec2CurrentPos.x, vec2CurrentPos.y, intSlotWidth, intSlotHeight), weapon))
//			   unitPlayer.equipWeapon(weapon);
//
//			//Update the current position
//			vec2CurrentPos.x += intSlotWidth;
//			if(vec2CurrentPos.x >= Screen.width)
//			{
//				//Start at the beginning of the next row
//				vec2CurrentPos.x = 0;
//
//				//Move down a row
//				vec2CurrentPos.y += intSlotHeight;
//			}
//
//		}

	}

}
