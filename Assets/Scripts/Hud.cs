using UnityEngine;
using System;
using System.Collections;

public class Hud : MonoBehaviour
{
    //Declare public constants
    public const KeyCode keyCodeInventory = KeyCode.I;
    public const KeyCode keyCodeCrafting = KeyCode.C;
    public const KeyCode keyCodeAssembleItems = KeyCode.U;
	public const KeyCode keyCodeMenusMenu = KeyCode.O;

    //Up down left right a.k.a WASD
    public const KeyCode keyCodeInventoryUp = KeyCode.UpArrow;
    public const KeyCode keyCodeInventoryLeft = KeyCode.LeftArrow;
    public const KeyCode keyCodeInventoryDown = KeyCode.DownArrow;
    public const KeyCode keyCodeInventoryRight = KeyCode.RightArrow;
	public const KeyCode keyCodeInventoryAltLeft = KeyCode.A;
	public const KeyCode keyCodeInventoryAltRight = KeyCode.D;
    public const KeyCode keyCodeConfirm = KeyCode.J;

    //Declare instance variables
    protected enum tMenuStates
    {
        MENU_NONE = 0,
        INVENTORY = 1,
        CRAFTING = 2,
        ASSEMBLING = 3,
        MENU_MAIN = 4,
        GAME_OVER = 5
    }

    private tMenuStates menuCode = tMenuStates.MENU_NONE;
    private bool boolEquipWeapon = false;
    Unit player;
    UnitPlayer unitPlayer;

    //TODO Temporary arrays to hold components
    //TODO May have to enumerate All components first and keep all the arrays around to update later. That or bookeeping
	//TODO Start here and combine possible weapons
    string[] arrComponents;
    string[] arrLightHandles ;
    string[] arrNormalHandles;
    string[] arrHeavyHandles ;
    //Grid for component options
    string[][] arrAllComponents ;
    string[] arrAll;
	//Grid for weapon types
	string[] arrWeaponTypes;

	tMenuStates[] arrMenusMenu = {tMenuStates.INVENTORY, tMenuStates.CRAFTING, tMenuStates.ASSEMBLING};

    //Options for laying out the grid
    //The measurements for the item slot representation
    int intSlotWidth;
    int intSlotHeight;

    int intCompTypeWidth;
    int intCompSelCols = 3;

	//Vector 2's to store some x and y components
    Vector2 vec2CompTypeStart;
    Vector2 vec2CompTypeDimensions;
    //Indexes for the selections
	int intMenusMenu = 0;
    int intCompTypeGrid = 0; //Index for the type of component
    int intCompSelGrid = 0; //Index for selecting different components
	int intAssemType = 0; //Index for selecting which type of weapon to assemble
	int intAssemPossible = 0; //Index for selecting which weapon to assemble
    private int intSelectedWeapon = 0; //The index of the selected weapon


    //Current value of the scroll bar
//    float vSbarValue = 0; //TODO Use the scroll bar to move through the inventory

    //How many items can fit on each row/column
    int intItemsPerRow;
    int intItemsPerCol;

    protected void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();
        unitPlayer = player.GetComponent<UnitPlayer>();

		//Assuming some sizes makes other calculations easier
        intSlotWidth = 300; //The width of an item slot
        intSlotHeight = 100; //The height of an item slot
        intItemsPerRow = Screen.width / intSlotWidth;
        intItemsPerCol = Screen.height / intSlotHeight;


        //Initialize component data structures
		int intNumWeapons = Enum.GetNames(typeof(ItemWeapon.tWeaponType)).Length;
		int intNumParts = Enum.GetNames (typeof(ItemComponent.tComponentPart)).Length;

		arrComponents = new string[intNumWeapons * intNumParts];

		//Find all the weapon categories and add them to an array
		int index = 0;
		foreach(ItemWeapon.tWeaponType wepType in (ItemWeapon.tWeaponType[]) Enum.GetValues(typeof(ItemWeapon.tWeaponType)))
		{
			foreach(ItemComponent.tComponentPart partType in (ItemComponent.tComponentPart[]) Enum.GetValues(typeof(ItemComponent.tComponentPart)))
			{
				string code = ItemComponent.getComponentCategoryCode(wepType, partType);
				string name = ItemComponent.getComponentName(code);

				arrComponents[index] = name;
				index += 1;
			}
		}

        arrLightHandles = new string[] { "", "", "", "", "", "?", "Lightened Copper Handle" };
        arrNormalHandles = new string[] { "", "", "", "", "", "?", "Normal Copper Handle" };
        arrHeavyHandles = new string[] { "", "", "", "", "", "?", "Heavy Copper Handle" };
        arrAllComponents = new string[][]{ arrLightHandles, arrNormalHandles, arrHeavyHandles };
        arrAll = new string[arrAllComponents[0].Length + arrAllComponents[1].Length + arrAllComponents[2].Length];


        //NOTE: All component arrays should be the same length
        //TODO For the one layer undiscovered items, show a "?", for undiscovered layers above that, put ""
        int intArrAllIndex = 0;
        for (int i = 0; i < arrLightHandles.Length; i++)
        {
            arrAll[intArrAllIndex] = arrAllComponents[0][i];
            arrAll[intArrAllIndex + 1] = arrAllComponents[1][i];
            arrAll[intArrAllIndex + 2] = arrAllComponents[2][i];

            intArrAllIndex += 3;
        }

		arrWeaponTypes = Enum.GetNames (typeof(ItemWeapon.tWeaponType));
    }

	/// <summary>
	/// Take care of switching through menus when keys are pressed and also moving inside of menus.
	/// </summary>
    protected void Update()
    {
		//Toggle the appropriate menu
		if(Input.GetKeyUp(keyCodeMenusMenu))
		{
//			if(menuCode == tMenuStates.MENU_NONE)
//				menuCode = tMenuStates.MENU_MAIN;
//			else if (menuCode == tMenuStates.MENU_MAIN)
//				menuCode = tMenuStates.MENU_NONE;
		}

		else if (Input.GetKeyUp(keyCodeInventory)) //Allow toggling of the inventory when the corresponding button has been pressed
        {
            if (menuCode == tMenuStates.MENU_NONE)
                menuCode = tMenuStates.INVENTORY;
            else if (menuCode == tMenuStates.INVENTORY)
                menuCode = tMenuStates.MENU_NONE;
        }
        else if(Input.GetKeyUp(keyCodeCrafting)) //Allow toggling on/off of the crafting menu
        {
            if (menuCode == tMenuStates.MENU_NONE)
                menuCode = tMenuStates.CRAFTING;
            else if (menuCode == tMenuStates.CRAFTING)
                menuCode = tMenuStates.MENU_NONE;
        }
        else if (Input.GetKeyUp(keyCodeAssembleItems)) //Allow toggling on/off of the assembling menu
        {
            if (menuCode == tMenuStates.MENU_NONE)
                menuCode = tMenuStates.ASSEMBLING;
            else if (menuCode == tMenuStates.ASSEMBLING)
                menuCode = tMenuStates.MENU_NONE;
        }

		//Take care of movement inside menus
		if(menuCode != tMenuStates.MENU_NONE) //Menu to select menues
		{
			if(Input.GetKeyUp(keyCodeInventoryAltRight))
				intMenusMenu = (arrMenusMenu.Length + intMenusMenu + 1) % arrMenusMenu.Length;
			else if(Input.GetKeyUp(keyCodeInventoryAltLeft))
				intMenusMenu = Math.Max(intMenusMenu - 1, 0);

			menuCode = arrMenusMenu[intMenusMenu];
		}
        switch (menuCode)
        {
            //If the inventory is open, process movement inside the inventory
            case tMenuStates.INVENTORY:
                {
                    //Process menu movement inside the inventory
                    if (Input.GetKeyUp(keyCodeInventoryUp))
                        moveInventorySelectionUp();
                    else if (Input.GetKeyUp(keyCodeInventoryDown))
                        moveInventorySelectionDown();
                    else if (Input.GetKeyUp(keyCodeInventoryLeft))
                        moveInventorySelectionLeft();
                    else if (Input.GetKeyUp(keyCodeInventoryRight))
                        moveInventorySelectionRight();

                    //Process confirmations
                    if (Input.GetKeyUp(keyCodeConfirm))
                        boolEquipWeapon = true;

                    break;
                }
            case tMenuStates.CRAFTING:
                {
                    handleComponentMovement();
                    break;
                }
            case tMenuStates.ASSEMBLING:
                {
                    //TODO make a button that combines all components if they can make a weapon.
					handleAssembleMovement();
                    break;
                }
        }
    }


    void OnGUI()
    {

        //TODO This is a test implementation of a scroll bar that will give values from 0(top) to 9(bottom)
        //		GUI.Label (new Rect(300, 100, 50, 20), Math.Floor(vSbarValue).ToString());
        //		vSbarValue = GUI.VerticalScrollbar(new Rect (100, 100, 100, 100), vSbarValue, 1.0f, 0.0f, 10.0f);
        //
        //		if(GUI.Button(new Rect(250, 250, 50, 50), "Up"))
        //			vSbarValue += 1;
        //
        //		if(GUI.Button(new Rect(250, 350, 50, 50), "Down"))
        //			vSbarValue -= 1;

		if(menuCode != tMenuStates.MENU_NONE)
		{
			showMenuStates();
		}

        switch (menuCode)
        {
            case tMenuStates.MENU_NONE: //Display regular player info
			{
                // Make a health bar
                GUI.Box(new Rect(10, 10, 100, 30), player.Health + "/" + player.MaxHealth);
                GUI.Box(new Rect(10, 40, 100, 30), "Score: " + player.Score);

                break;
			}
            case tMenuStates.INVENTORY:
			{
                layoutInventoryGrid();

                break;
			}
            case tMenuStates.CRAFTING:
			{
                layoutCraftingGUI();

				break;
			}
            case tMenuStates.ASSEMBLING:
            {
                layoutAssembleGrid();
                
				break;
            }
        }
    }

	private void showMenuStates()
	{
		UnityEngine.GUIStyle style = new GUIStyle(GUI.skin.button);
		Texture2D tex2dButtonPassiveBack = new Texture2D(1, 1);

		tex2dButtonPassiveBack = (Texture2D)Resources.Load("InventoryTypeBackground");
		style.normal.background = tex2dButtonPassiveBack;

		GUI.Label (new Rect (0, 0, 150, 50),arrMenusMenu[intMenusMenu].ToString(),style);
	}


	private void layoutAssembleGrid()
	{
		//TODO Make some actual dimensions

		vec2CompTypeDimensions.x = 300;
		vec2CompTypeDimensions.y = 200;

		Texture2D tex2dButtonPassiveBack = new Texture2D(1, 1);
		Texture2D tex2dButtonActiveBack = new Texture2D (1, 1);
		Texture2D tex2dButtonFlashBack = new Texture2D (1, 1);
		UnityEngine.GUIStyle style = new GUIStyle(GUI.skin.button);


		//Set the style for selection screens
		//TODO Currently, can't initialize a style outside of On GUI. Find a  way to call this outside of OnGUI for efficiency
		tex2dButtonPassiveBack = (Texture2D)Resources.Load("InventoryButtonBackground");
		tex2dButtonActiveBack = (Texture2D)Resources.Load("SelectedBackground");
		tex2dButtonFlashBack = (Texture2D)Resources.Load("SelectedBackgroundFlash");
		
		//Backgrounds when I have an active selection
		style.active.background = tex2dButtonActiveBack;
		style.focused.background = tex2dButtonActiveBack;
		style.onFocused.background = tex2dButtonActiveBack;
		style.onNormal.background = tex2dButtonActiveBack;
		
		//Backgrounds for non-active items
		style.normal.background = tex2dButtonPassiveBack;
		style.hover.background = tex2dButtonPassiveBack;
		style.onHover.background = tex2dButtonPassiveBack;


		//Make a label to show which kind of weapon is being assembled
		int intAssemWeaponLabelWidth = Screen.width / 4;
		int intAssemWeaponLabelHeight = Screen.height / 8;
		int intAssemWeaponLabelX = (Screen.width / 2) - (intAssemWeaponLabelWidth / 2); //Start the label at half the screen shifted by half the label width
		int intAssemWeaponLabelY = (Screen.height / 20);
		GUI.Label (new Rect (intAssemWeaponLabelX, intAssemWeaponLabelY, intAssemWeaponLabelWidth, intAssemWeaponLabelHeight), arrWeaponTypes [intAssemType], style);

		//TODO Need some way of enumerating possible weapon build from the components in the inventory
		//TODO Maybe for a given type, ask for the components
		ArrayList arrListAssemblable = GetMakeableItems ();
		string[] arrAssembleStrings = new string[arrListAssemblable.Count];
		for(int i = 0; i < arrListAssemblable.Count; i++)
		{
			arrAssembleStrings[i] = ((ItemWeapon)arrListAssemblable[i]).name;
		}

		intCompTypeGrid = GUI.SelectionGrid(new Rect(vec2CompTypeStart.x, vec2CompTypeStart.y,  vec2CompTypeDimensions.x, vec2CompTypeDimensions.y), 
		                                    intAssemPossible, arrAssembleStrings, 1, style);
	

	}

	/// <summary>
	/// Get a list of ItemWeapons which can be made from the components in the inventory.
	/// </summary>
	/// <returns>The makeable items.</returns>
	private ArrayList GetMakeableItems()
	{
		ArrayList arrListComponents = Inventory.getInstance ().getInventoryComponents();
		ArrayList arrListResults = new ArrayList ();

		//Loop through all pairs of items and see if they can be combined
		for(int i = 0; i < arrListComponents.Count; i++)
		{
			for(int j = i + 1; j < arrListComponents.Count; j++)
			{
				//Create weapon returns null when the components given are unable to form a proper weapon
				ItemWeapon potentialWeapon = ItemFactory.createWeapon((ItemComponent)arrListComponents[i],(ItemComponent)arrListComponents[j]);
				if(potentialWeapon != null)
				{
					arrListResults.Add(potentialWeapon);
				}
			}
		}

		return arrListResults;
	}

    private void layoutCraftingGUI()
    {
        Texture2D tex2dButtonPassiveBack = new Texture2D(1, 1);
        Texture2D tex2dButtonActiveBack = new Texture2D(1, 1);
        Texture2D tex2dButtonFlashBack = new Texture2D(1, 1);

        //Set the style for selection screens
        //TODO Currently, can't initialize a style outside of On GUI. Find a  way to call this outside of OnGUI for efficiency
        UnityEngine.GUIStyle style = new GUIStyle(GUI.skin.button);
        tex2dButtonPassiveBack = (Texture2D)Resources.Load("InventoryButtonBackground");
        tex2dButtonActiveBack = (Texture2D)Resources.Load("SelectedBackground");
        tex2dButtonFlashBack = (Texture2D)Resources.Load("SelectedBackgroundFlash");

        //Backgrounds when I have an active selection
        style.active.background = tex2dButtonActiveBack;
        style.focused.background = tex2dButtonActiveBack;
        style.onFocused.background = tex2dButtonActiveBack;
        style.onNormal.background = tex2dButtonActiveBack;

        //Backgrounds for non-active items
        style.normal.background = tex2dButtonPassiveBack;
        style.hover.background = tex2dButtonPassiveBack;
        style.onHover.background = tex2dButtonPassiveBack;

        //Backgrounds for selecting an item while the button is still being pressed
        style.onActive.background = tex2dButtonFlashBack;

        //TODO Draw background for menu
        //		GUI.Box (new Rect (0, 0, Screen.width, Screen.height), (Texture2D)Resources.Load("SelectedBackground"), style);

		//Initialize options for the menus
		int intWidthPadding = Screen.width / 15; //(1/8) of the screen for width padding
		int intHeightPadding = Screen.height / 8; //(1/8) of the screen for height padding
//		int intCompTypeWidth = (Screen.width - (2 * intWidthPadding)) / 4; //(Screen width - padding on both sides) is how big the components type menu should be

		vec2CompTypeStart = new Vector2(intWidthPadding, (intHeightPadding));
		vec2CompTypeDimensions = new Vector2(2.5f * intWidthPadding, Screen.height - (2f * intHeightPadding));

//        intCompTypeGrid = GUI.SelectionGrid(new Rect(vec2CompTypeStart.x, vec2CompTypeStart.y,  vec2CompTypeDimensions.x, vec2CompTypeDimensions.y),
//		                                    intCompTypeGrid, arrComponents, 1, style);
		//List of categories
		intCompTypeGrid = GUI.SelectionGrid(new Rect(vec2CompTypeStart.x, vec2CompTypeStart.y,  vec2CompTypeDimensions.x, vec2CompTypeDimensions.y),
		                                    intCompTypeGrid, arrComponents, 1, style);

		//List of components
        intCompSelGrid = GUI.SelectionGrid(new Rect(4.5f * intWidthPadding, intHeightPadding,
		                                            (6f * intWidthPadding), Screen.height - (2*intHeightPadding)),
		                                   intCompSelGrid, arrAll,  intCompSelCols, style);
		//Description Menu
		GUI.Label(new Rect(11 * intWidthPadding, vec2CompTypeStart.y,
		                   	vec2CompTypeDimensions.x, vec2CompTypeDimensions.y),
		          "[Insert Requirement Here]", style);

        //Grid for the types of components
        //		intLightenedGrid = GUI.SelectionGrid (new Rect (125, 100, 200, 400), intLightenedGrid, arrAllComponents[1], 1, style);
        //		intNormalGrid = GUI.SelectionGrid (new Rect (325, 100, 200, 400), intNormalGrid, arrAllComponents[2], 1, style);
        //		intHeavyGrid = GUI.SelectionGrid (new Rect (525, 100, 200, 400), intHeavyGrid, arrAllComponents[3], 1, style);
    }

    private void layoutInventoryGrid()
    {
        //Declare the options in laying out the grid
//        int intWidthPadding = Screen.width / 8;
//        int intHeightPadding = Screen.height / 8;

        //The starting position of the inventory
        Vector2 vec2CurrentPos = new Vector2(0, 100);

        //Find the inventory
        Inventory inventory = Inventory.getInstance();

        //TODO Allow scrolling down when weapons don't all fit on the screen

        ArrayList arrListWeapons = inventory.getInventoryWeapons();
        ArrayList arrComponents = inventory.getInventoryComponents();
        //TODO only keep weapons
        ArrayList allItems = new ArrayList();

        foreach (ItemWeapon weapon in arrListWeapons)
        {
            allItems.Add(weapon);
        }

        foreach(ItemBase component in arrComponents)
        {
            allItems.Add(component);
        }

        string[] arrStringWeapons = new string[allItems.Count];

        for (int i = 0; i < allItems.Count; i++)
        {
            //ItemWeapon wep = (ItemWeapon)arrListWeapons[i];
            ItemBase wep = (ItemBase)allItems[i];
            //Grab the names of the item associated with the weapon
            arrStringWeapons[i] = wep.ToString(); //To instead use pictures/textures, make an array of pictures/textures
        }

        intSelectedWeapon = GUI.SelectionGrid(new Rect(vec2CurrentPos.x, vec2CurrentPos.y, Screen.width, intSlotHeight),
                                              intSelectedWeapon, arrStringWeapons, intItemsPerRow);

        if (boolEquipWeapon)
        {
            //TODO Don't try and equip things other than weapons
            unitPlayer.equipWeapon(((ItemWeapon)arrListWeapons[intSelectedWeapon]).weaponType.ToString());
            boolEquipWeapon = false;
        }

        //      //TODO Uncomment this out if we want to use this. A.K.A ALTERNATIVE SCHEME
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

	private void handleAssembleMovement()
	{
		//Left and right change the type being assembled. (With wraparound)
		if(Input.GetKeyUp (keyCodeInventoryLeft)) 
		{
			int intNewType = intAssemType - 1;

			intAssemType = (intNewType < 0 ? arrWeaponTypes.Length - 1 : intNewType);
		}
		else if (Input.GetKeyUp (keyCodeInventoryRight))
		{
			int intNewType = intAssemType + 1;

			intAssemType = (intNewType > arrWeaponTypes.Length - 1 ? 0 : intNewType);
		}
	}

    private void handleComponentMovement()
    {
        //Take care of menu navigation from the buttons
        if (Input.GetKeyUp(keyCodeInventoryUp))
        {
            //move up in the respective menu
            if (intCompSelGrid > 0) //Component selection menu
            {
                int intNewSelection = intCompSelGrid - intCompSelCols;
                intCompSelGrid = Math.Max(intNewSelection, 0);
            }
            else //Component type menu
            {
                int intNewSelection = intCompTypeGrid - 1;
                intCompTypeGrid = Math.Max(intNewSelection, 0);

            }
        }
        else if (Input.GetKeyUp(keyCodeInventoryRight))
        {
            //move right
            //if i'm in the component type menu, switch over to the components
            if (intCompSelGrid >= 0)
            {
                int intNewSelection = intCompSelGrid + 1;
                intCompSelGrid = Math.Min(intNewSelection, arrAll.Length - 1);
            }
            else //I'm in the component type menu
            {
                intCompSelGrid = 0;
            }
        }
        else if (Input.GetKeyUp(keyCodeInventoryDown))
        {
            //Move down in the respective menu
            if (intCompSelGrid >= 0)
            {
                int intNewSelection = intCompSelGrid + intCompSelCols;
                intCompSelGrid = Math.Min(arrAll.Length - 1, intNewSelection);
            }
            else //I'm in the component type menu
            {
                int intNewSelection = intCompTypeGrid + 1;
                intCompTypeGrid = Math.Min(intNewSelection, arrComponents.Length - 1);
            }
        }
        else if (Input.GetKeyUp(keyCodeInventoryLeft))
        {
            //move left
            //if i'm in the components menu, switch over to the component type menu
            if (intCompSelGrid >= 0)
            {
                int intNewSelection = intCompSelGrid - 1;
                intCompSelGrid = intNewSelection;
            }
            else //I'm in the component type menu
            {
                //Don't do anything
            }

        }
        else if (Input.GetKeyUp(keyCodeConfirm))
        {
			//Make the button be selected when the confirm key is pressed
//			SendMessage("onActive");

            //Assume we have ore
			//TODO Actually make the appropriate weapon making call. Outlined below, but not complete
			//ItemComponent cmpnew = ItemFactory.createComponent( ((ItemComponent.tComponentType)getPart(code[grid])), getOre(code[grid])) 
			string cmpNewCode = ItemComponent.generateComponentCode(ItemComponent.tAttributeType.Normal, ItemBase.tOreType.Copper, ItemWeapon.tWeaponType.WeaponSword,
			                                                        ItemComponent.tComponentPart.Blade);
            ItemComponent cmpNew = ItemFactory.createComponent(cmpNewCode);

            Inventory inventory = Inventory.getInstance();
            inventory.inventoryAddItem(cmpNew);
        }
    }


    private void moveInventorySelectionUp()
    {
        //Decrement the index by itemsPerRow and take the maximum of 0 and newIndex
        int intNewSelected = intSelectedWeapon - intItemsPerRow;

        intSelectedWeapon = Mathf.Max(0, intNewSelected);
    }

    private void moveInventorySelectionDown()
    {
        //Increment the index by itemsPerRow and take the min of arrListWeapons.Count and newIndex
        int intNewSelected = intSelectedWeapon + intItemsPerRow;

        Inventory inventory = Inventory.getInstance();

        ArrayList arrListWeapons = inventory.getInventoryWeapons();
        intSelectedWeapon = Mathf.Min(arrListWeapons.Count - 1, intNewSelected);
    }

    private void moveInventorySelectionLeft()
    {
        //Decrement the index by 1 and take the maximum of 0 and newIndex
        int intNewSelected = intSelectedWeapon - 1;

        //Bound checking
        intSelectedWeapon = Mathf.Max(intNewSelected, 0);
    }

    private void moveInventorySelectionRight()
    {
        //Increment the index by 1 and take the min of arrListWeapons.Count and newIndex
        int intNewSelected = intSelectedWeapon + 1;

        Inventory inventory = Inventory.getInstance();

        ArrayList arrListWeapons = inventory.getInventoryWeapons();

        //Pressing right can wraparound to the next row if one exists
        intSelectedWeapon = Mathf.Min(intNewSelected, arrListWeapons.Count - 1);
    }


}