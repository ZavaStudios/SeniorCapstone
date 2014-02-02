using UnityEngine;
using System;
using System.Collections;

public class Hud : MonoBehaviour 
{
	//Declare public constants
	public const KeyCode keyCodeInventory = KeyCode.I;
    //Up down left right a.k.a WASD
    public const KeyCode keyCodeInventoryUp = KeyCode.UpArrow;
    public const KeyCode keyCodeInventoryLeft = KeyCode.LeftArrow;
    public const KeyCode keyCodeInventoryDown = KeyCode.DownArrow;
    public const KeyCode keyCodeInventoryRight = KeyCode.RightArrow;
	public const KeyCode keyCodeConfirm = KeyCode.J;

	//Declare instance variables
	private bool boolInventory = false;
	private bool boolEquipWeapon = false;
	Unit player;
	UnitPlayer unitPlayer;

	private int intSelectedWeapon = 0; //The index of the selected weapon

    //Options for laying out the grid
    //The measurements for the item slot representation
    int intSlotWidth;
    int intSlotHeight;

    //How many items can fit on each row/column
    int intItemsPerRow;
    int intItemsPerCol;

	protected void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();
		unitPlayer = player.GetComponent<UnitPlayer>();

        intSlotWidth = 100;
        intSlotHeight = 100;
        intItemsPerRow = Screen.width / intSlotWidth;
        intItemsPerCol = Screen.height / intSlotHeight;

	}

	protected void Update()
	{
		//Allow toggling of the inventory when the corresponding button has been pressed
		if(Input.GetKeyUp(keyCodeInventory))
		{
			boolInventory = !boolInventory;
		}

        //If the inventory is open, process movement inside the inventory
        if (boolInventory)
        {
			//Process movement inside the inventory
            if(Input.GetKeyUp(keyCodeInventoryUp))
               moveSelectionUp();
            else if (Input.GetKeyUp(keyCodeInventoryDown))
                moveSelectionDown();
            else if (Input.GetKeyUp(keyCodeInventoryLeft))
               moveSelectionLeft();
            else if (Input.GetKeyUp (keyCodeInventoryRight))
               moveSelectionRight();

			//Process confirmations
			if(Input.GetKeyUp(keyCodeConfirm))
				boolEquipWeapon = true;
        }
	}

	//Indexes for the selections
	int intCompTypeGrid = 0;
	int intLightenedGrid = 1;
	int intNormalGrid = -1;
	int intHeavyGrid = -1;

	//Current value of the scroll bar
	float vSbarValue = 0;

    void OnGUI () 
    {

		string[] arrComponents = new string[] {"Sword Handle", "Sword Blade", "Bow", "Bow String"};

		//TODO May have to enumerate All components first and keep all the arrays around to update later. That or bookeeping
		string[] arrLightHandles = new string[]{"?", "Lightened Gold Handle", "Lightened Silver Handle", "Lightened Copper Handle"};
		string[] arrNormalHandles = new string[]{"", 		"",							"?",					"Normal Copper Handle"};
		string[] arrHeavyHandles = new string[]{"", 		"?",						"Heavy Silver Handle", "Heavy Copper Handle"};
		
		Texture2D tex2dButtonPassiveBack = new Texture2D(4,4);
		Texture2D tex2dButtonActiveBack = new Texture2D(4,4);

		//Set the style for selection screens
		//TODO Currently, can't initialize a style outside of On GUI. Find a  way to call this outside of OnGUI for efficiency
		UnityEngine.GUIStyle style = new GUIStyle (GUI.skin.button);
		tex2dButtonPassiveBack = (Texture2D)Resources.Load ("InventoryButtonBackground");
		tex2dButtonActiveBack = (Texture2D)Resources.Load ("SelectedBackground");

		//Backgrounds when I have an active selection
		style.active.background = tex2dButtonActiveBack;
		style.focused.background = tex2dButtonActiveBack;
		style.onActive.background = tex2dButtonActiveBack;
		style.onFocused.background = tex2dButtonActiveBack;
		style.onNormal.background = tex2dButtonActiveBack;

		//Backgrounds for non-active items
		style.normal.background = tex2dButtonPassiveBack;
		style.hover.background = tex2dButtonPassiveBack;
		style.onHover.background = tex2dButtonPassiveBack;

		//Draw background for menu
//		GUI.Box (new Rect (0, 0, Screen.width, Screen.height), (Texture2D)Resources.Load("SelectedBackground"), style);

		//Grid for component options
		string[][] arrAllComponents = {arrLightHandles, arrNormalHandles, arrHeavyHandles};
		string[] arrAll = new string[arrAllComponents[0].Length + arrAllComponents[1].Length + arrAllComponents[2].Length] ;

		//NOTE: All component arrays should be the same length
		//For the one layer undiscovered items, show a "?", for undiscovered layers above that, put ""
		int intArrAllIndex = 0;
		for (int i = 0; i < arrLightHandles.Length; i++) 
		{
			arrAll[intArrAllIndex] = arrAllComponents[0][i];
			arrAll[intArrAllIndex + 1] = arrAllComponents[1][i];
			arrAll[intArrAllIndex + 2] = arrAllComponents[2][i];

			intArrAllIndex += 3;

		}
				

		intCompTypeGrid = GUI.SelectionGrid (new Rect (25, 100, 100, 100), intCompTypeGrid, arrComponents, 1, style);
		intLightenedGrid = GUI.SelectionGrid (new Rect (25, 300, 500, 500), intLightenedGrid, arrAll, 3, style);


		//Grid for the types of components
//		intLightenedGrid = GUI.SelectionGrid (new Rect (125, 100, 200, 400), intLightenedGrid, arrAllComponents[1], 1, style);
//		intNormalGrid = GUI.SelectionGrid (new Rect (325, 100, 200, 400), intNormalGrid, arrAllComponents[2], 1, style);
//		intHeavyGrid = GUI.SelectionGrid (new Rect (525, 100, 200, 400), intHeavyGrid, arrAllComponents[3], 1, style);


		//TODO This is a test implementation of a scroll bar that will give values from 0(top) to 9(bottom)
//		GUI.Label (new Rect(300, 100, 50, 20), Math.Floor(vSbarValue).ToString());
//		vSbarValue = GUI.VerticalScrollbar(new Rect (100, 100, 100, 100), vSbarValue, 1.0f, 0.0f, 10.0f);
//
//		if(GUI.Button(new Rect(250, 250, 50, 50), "Up"))
//			vSbarValue += 1;
//
//		if(GUI.Button(new Rect(250, 350, 50, 50), "Down"))
//			vSbarValue -= 1;











        // Make a health bar
        GUI.Box(new Rect(10,10,100,30), player.Health + "/" + player.MaxHealth);
		GUI.Box (new Rect (10, 40, 100, 30), "Score: " + player.Score);

		// TODO Show the Inventory and disable other controls such as the crosshair and attacking
		if(boolInventory)
		{
			//layoutInventoryGrid();

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


    private void layoutItemCraftingGUI()
    {


    }

	private void layoutInventoryGrid()
	{
		//Declare the options in laying out the grid

		//The starting position of the inventory
		Vector2 vec2CurrentPos = new Vector2 (0, 100);
	
		//Find the inventory
		Inventory inventory = ((Unit)(GameObject.FindGameObjectWithTag ("Player").GetComponent<Unit>())).inventory;

		//TODO Allow scrolling down when weapons don't all fit on the screen

		ArrayList arrListWeapons = inventory.getInventoryWeapons();

		string[] arrStringWeapons = new string[arrListWeapons.Count];

		for(int i = 0; i < arrListWeapons.Count; i++)
		{
			//Grab the names of the item associated with the weapon
			arrStringWeapons[i] = ((WeaponBase)arrListWeapons[i]).strWeaponType; //To instead use pictures/textures, make an array of pictures/textures

		}

		intSelectedWeapon = GUI.SelectionGrid (new Rect (vec2CurrentPos.x, vec2CurrentPos.y, Screen.width, intSlotHeight),
		                                      intSelectedWeapon, arrStringWeapons, intItemsPerRow);

		if(boolEquipWeapon)
		{
			unitPlayer.equipWeapon (arrStringWeapons [intSelectedWeapon]);
			boolEquipWeapon = false;
		}

//      //TODO Uncomment this out if we want to use this
//		//Loop through all the items and lay them out simply
//
//		//Store the current weapon being worked with
//		string weapon;
//		for(int i = 0; i < arrListWeapons.Count; i++)
//		{
//			weapon = arrListWeapons[i].ToString();
//
//
//            //Set the clicked button to have a special style
//			if(GUI.Button(new Rect(vec2CurrentPos.x, vec2CurrentPos.y, intSlotWidth, intSlotHeight), weapon))
//			{
//				unitPlayer.equipWeapon(weapon);
//                intSelectedWeapon = i;
//			}
//
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

    private void moveSelectionUp()
    {
      //Decrement the index by itemsPerRow and take the maximum of 0 and newIndex
		int intNewSelected = intSelectedWeapon - intItemsPerRow;

		intSelectedWeapon = Mathf.Max (0, intNewSelected);
    }

    private void moveSelectionDown()
    {
        //Increment the index by itemsPerRow and take the min of arrListWeapons.Count and newIndex
		int intNewSelected = intSelectedWeapon + intItemsPerRow;

		Inventory inventory = ((Unit)(GameObject.FindGameObjectWithTag ("Player").GetComponent<Unit>())).inventory;
		
		ArrayList arrListWeapons = inventory.getInventoryWeapons();
		intSelectedWeapon = Mathf.Min (arrListWeapons.Count -1, intNewSelected);
    }

    private void moveSelectionLeft()
    {
        //Decrement the index by 1 and take the maximum of 0 and newIndex
        int intNewSelected = intSelectedWeapon - 1;

        //Bound checking
        intSelectedWeapon = Mathf.Max(intNewSelected,0);
    }

    private void moveSelectionRight()
    {
        //Increment the index by 1 and take the min of arrListWeapons.Count and newIndex
        int intNewSelected = intSelectedWeapon + 1;

        Inventory inventory = ((Unit)(GameObject.FindGameObjectWithTag ("Player").GetComponent<Unit>())).inventory;
        
        ArrayList arrListWeapons = inventory.getInventoryWeapons();

        //Pressing right can wraparound to the next row if one exists
        intSelectedWeapon = Mathf.Min(intNewSelected, arrListWeapons.Count - 1);
    }


}