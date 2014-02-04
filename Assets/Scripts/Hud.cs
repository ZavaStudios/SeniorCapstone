using UnityEngine;
using System;
using System.Collections;

public class Hud : MonoBehaviour
{
    //Declare public constants
    public const KeyCode keyCodeInventory = KeyCode.I;
    public const KeyCode keyCodeCrafting = KeyCode.C;
    public const KeyCode keyCodeAllItems = KeyCode.H;

    //Up down left right a.k.a WASD
    public const KeyCode keyCodeInventoryUp = KeyCode.UpArrow;
    public const KeyCode keyCodeInventoryLeft = KeyCode.LeftArrow;
    public const KeyCode keyCodeInventoryDown = KeyCode.DownArrow;
    public const KeyCode keyCodeInventoryRight = KeyCode.RightArrow;
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

    string[] arrComponents;
    string[] arrLightHandles ;
    string[] arrNormalHandles;
    string[] arrHeavyHandles ;
    //Grid for component options
    string[][] arrAllComponents ;
    string[] arrAll;


    //Options for laying out the grid
    //The measurements for the item slot representation
    int intSlotWidth;
    int intSlotHeight;


    int intWidthPadding;
    int intHeightPadding;
    int intCompTypeWidth;
    int intCompSelCols = 3;

    Vector2 vec2CompTypeStart;
    Vector2 vec2CompTypeDimensions;
    //Indexes for the selections
    int intCompTypeGrid = 0; //Index for the type of component
    int intCompSelGrid = 0; //Index for selecting different components
    private int intSelectedWeapon = 0; //The index of the selected weapon


    //Current value of the scroll bar
    float vSbarValue = 0;

    //How many items can fit on each row/column
    int intItemsPerRow;
    int intItemsPerCol;

    protected void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();
        unitPlayer = player.GetComponent<UnitPlayer>();

        intSlotWidth = 300;
        intSlotHeight = 100;
        intItemsPerRow = Screen.width / intSlotWidth;
        intItemsPerCol = Screen.height / intSlotHeight;


        //Initialize component data structures
        arrComponents = new string[] { "Sword Handle", "Sword Blade", "Staff", "Staff Power Stone" };
        arrLightHandles = new string[] { "", "", "", "?", "", "?", "Lightened Copper Handle" };
        arrNormalHandles = new string[] { "", "", "", "", "", "?", "Normal Copper Handle" };
        arrHeavyHandles = new string[] { "", "", "", "", "?", "?", "Heavy Copper Handle" };
        arrAllComponents = new string[][]{ arrLightHandles, arrNormalHandles, arrHeavyHandles };
        arrAll = new string[arrAllComponents[0].Length + arrAllComponents[1].Length + arrAllComponents[2].Length];

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


        //Initialize options for the menus
        intWidthPadding = Screen.width / 8;
        intHeightPadding = Screen.height / 8;
        intCompTypeWidth = (Screen.width - (2 * intWidthPadding)) / 4;
        vec2CompTypeStart = new Vector2(intWidthPadding, (intHeightPadding * 1.5f));
        vec2CompTypeDimensions = new Vector2(intCompTypeWidth - (intWidthPadding * 0.25f), Screen.height - (6f * intHeightPadding));


    }

    protected void Update()
    {
        //TODO Merge if statement code together to avoid duplication
        //Allow toggling of the inventory when the corresponding button has been pressed
        if (Input.GetKeyUp(keyCodeInventory))
        {
            if (menuCode == tMenuStates.MENU_NONE)
                menuCode = tMenuStates.INVENTORY;
            else if (menuCode == tMenuStates.INVENTORY)
                menuCode = tMenuStates.MENU_NONE;

        }
        else if(Input.GetKeyUp(keyCodeCrafting))
        {
            if (menuCode == tMenuStates.MENU_NONE)
                menuCode = tMenuStates.CRAFTING;
            else if (menuCode == tMenuStates.CRAFTING)
                menuCode = tMenuStates.MENU_NONE;
        }
        else if (Input.GetKeyUp(keyCodeAllItems))
        {
            if (menuCode == tMenuStates.MENU_NONE)
                menuCode = tMenuStates.ASSEMBLING;
            else if (menuCode == tMenuStates.ASSEMBLING)
                menuCode = tMenuStates.MENU_NONE;
        }

        switch (menuCode)
        {
            //If the inventory is open, process movement inside the inventory
            case tMenuStates.INVENTORY:
                {
                    //Process movement inside the inventory
                    if (Input.GetKeyUp(keyCodeInventoryUp))
                        moveSelectionUp();
                    else if (Input.GetKeyUp(keyCodeInventoryDown))
                        moveSelectionDown();
                    else if (Input.GetKeyUp(keyCodeInventoryLeft))
                        moveSelectionLeft();
                    else if (Input.GetKeyUp(keyCodeInventoryRight))
                        moveSelectionRight();

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



        switch (menuCode)
        {
            case tMenuStates.MENU_NONE:
                // Make a health bar
                GUI.Box(new Rect(10, 10, 100, 30), player.Health + "/" + player.MaxHealth);
                GUI.Box(new Rect(10, 40, 100, 30), "Score: " + player.Score);

                break;

            case tMenuStates.INVENTORY:
                // TODO Show the Inventory and disable other controls such as the crosshair and attacking
                layoutInventoryGrid();

                break;

            case tMenuStates.CRAFTING:
                layoutItemCraftingGUI();
                if(Input.GetKeyUp(keyCodeInventoryRight))
                    Debug.Log("KEYPRESS Right got called");

                break;

            case tMenuStates.ASSEMBLING:
                {
                    //layoutAssemblingGrid();
                    break;
                }
        }

        if(Input.GetKeyUp(keyCodeConfirm))
            Debug.Log("KEYPRESS J was called ");
    }



    private void layoutItemCraftingGUI()
    {
        Texture2D tex2dButtonPassiveBack = new Texture2D(4, 4);
        Texture2D tex2dButtonActiveBack = new Texture2D(4, 4);
        Texture2D tex2dButtonFlashBack = new Texture2D(4, 4);

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


        intCompTypeGrid = GUI.SelectionGrid(new Rect(vec2CompTypeStart.x, vec2CompTypeStart.y,  vec2CompTypeDimensions.x, vec2CompTypeDimensions.y), intCompTypeGrid, arrComponents, 1, style);
        GUI.Label(new Rect(vec2CompTypeStart.x, vec2CompTypeStart.y + vec2CompTypeDimensions.y + (intHeightPadding * 0.5f), vec2CompTypeDimensions.x, vec2CompTypeDimensions.y + (0.5f * intHeightPadding)), "HEllo World", style);
        intCompSelGrid = GUI.SelectionGrid(new Rect(intCompTypeWidth + intWidthPadding, intHeightPadding, Screen.width - intCompTypeWidth - (2 * intWidthPadding) , Screen.height - (2*intHeightPadding)), intCompSelGrid, arrAll,  intCompSelCols, style);

        //Grid for the types of components
        //		intLightenedGrid = GUI.SelectionGrid (new Rect (125, 100, 200, 400), intLightenedGrid, arrAllComponents[1], 1, style);
        //		intNormalGrid = GUI.SelectionGrid (new Rect (325, 100, 200, 400), intNormalGrid, arrAllComponents[2], 1, style);
        //		intHeavyGrid = GUI.SelectionGrid (new Rect (525, 100, 200, 400), intHeavyGrid, arrAllComponents[3], 1, style);
    }

    private void layoutInventoryGrid()
    {
        //Declare the options in laying out the grid
        int intWidthPadding = Screen.width / 8;
        int intHeightPadding = Screen.height / 8;

        //The starting position of the inventory
        Vector2 vec2CurrentPos = new Vector2(0, 100);

        //Find the inventory
        Inventory inventory = ((Unit)(GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>())).inventory;

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
            arrStringWeapons[i] = wep.name; //To instead use pictures/textures, make an array of pictures/textures
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

    private void handleComponentMovement()
    {
        //Take care of menu navigation from the buttons
        if (Input.GetKeyUp(keyCodeInventoryUp))
        {
            //moveUp in the respective menu
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
            Debug.Log("KEYPRESS originalIndex: " + intCompSelGrid);
            if (intCompSelGrid >= 0)
            {
                int intNewSelection = intCompSelGrid + 1;

                intCompSelGrid = Math.Min(intNewSelection, arrAll.Length - 1);
            }
            else //I'm in the component type menu
            {
                intCompSelGrid = 0;
            }
            Debug.Log("KEYPRESS newIndex: " + intCompSelGrid);
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
            //Assume we have ore
            ItemComponent cmpNew = ItemFactory.createComponent(ItemComponent.tComponentType.SwordBladeHeavy, new ItemOre(ItemBase.tOreType.Copper));

            Inventory inventory = Inventory.getInstance();
            inventory.inventoryAddItem(cmpNew);
            

        }
    }


    private void moveSelectionUp()
    {
        //Decrement the index by itemsPerRow and take the maximum of 0 and newIndex
        int intNewSelected = intSelectedWeapon - intItemsPerRow;

        intSelectedWeapon = Mathf.Max(0, intNewSelected);
    }

    private void moveSelectionDown()
    {
        //Increment the index by itemsPerRow and take the min of arrListWeapons.Count and newIndex
        int intNewSelected = intSelectedWeapon + intItemsPerRow;

        Inventory inventory = ((Unit)(GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>())).inventory;

        ArrayList arrListWeapons = inventory.getInventoryWeapons();
        intSelectedWeapon = Mathf.Min(arrListWeapons.Count - 1, intNewSelected);
    }

    private void moveSelectionLeft()
    {
        //Decrement the index by 1 and take the maximum of 0 and newIndex
        int intNewSelected = intSelectedWeapon - 1;

        //Bound checking
        intSelectedWeapon = Mathf.Max(intNewSelected, 0);
    }

    private void moveSelectionRight()
    {
        //Increment the index by 1 and take the min of arrListWeapons.Count and newIndex
        int intNewSelected = intSelectedWeapon + 1;

        Inventory inventory = ((Unit)(GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>())).inventory;

        ArrayList arrListWeapons = inventory.getInventoryWeapons();

        //Pressing right can wraparound to the next row if one exists
        intSelectedWeapon = Mathf.Min(intNewSelected, arrListWeapons.Count - 1);
    }


}