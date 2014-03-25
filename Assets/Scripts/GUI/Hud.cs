using UnityEngine;
using System;
using System.Collections;

public class Hud : MonoBehaviour
{
		//A private class to represent an inventory slot
		private class ItemSlot
		{
				public ItemBase item;
				public bool unlocked;
				public ItemOre oreNeeded;

				public ItemSlot ()
				{
						item = null;
						unlocked = false;

						oreNeeded = new ItemOre (ItemBase.tOreType.NOT_ORE);
						oreNeeded.Quantity = 0;
						oreNeeded.neededPoints = 0;
				}
		}

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

		//Keep a reference to the player
		Unit player;
		UnitPlayer unitPlayer;

		//Keep a reference to the inventory
		Inventory inventory = Inventory.getInstance ();

		//TODO Clean up all this code. Maybe separate into different classes.
		//Enumerate All components first and keep all the arrays around to update later.
		ItemSlot[,] arrMakeableComps;
		string[] arrWepPartNames;
		ItemSlot[][,] arrComponentGrids; //An array that holds 2d arrays of itemcomponents. These 2d arrays are the tiered components that the player sees

		//Grid for component options
		ItemComponent[] arrAllComponents;
		string[] arrSelectedComponentNames;

		//Grid for possible assembled items
		ItemComponent[][] arrAssembleWeapons;

		//Grid for weapon types
		string[] arrWeaponTypes;

		//The available options for selecting menus
		tMenuStates[] arrMenusMenu = {
		tMenuStates.INVENTORY,
		tMenuStates.CRAFTING,
		tMenuStates.ASSEMBLING
	};

		//Options for laying out the grid
		int intInvSlotWidth; //The width of an inventory item slot
		int intInvSlotHeight; //The height of an inventory item slot
		int intInvItemsPerRow;
		int intInvItemsPerCol;
		int intCompTypeWidth;
		int intCompSelCols = (Enum.GetNames (typeof(ItemComponent.tAttributeType)).Length);
		int intCompSelRows = (Enum.GetNames (typeof(ItemBase.tOreType)).Length - ItemBase.getNonCraftingOres ().Count);

		//Vector 2's to store some x and y components
		Vector2 vec2CompTypeStart;
		Vector2 vec2CompTypeDimensions;
    
		//Indexes for the selections
		private int intMenusMenu = 0; //Index for which menu is selected
		private int intCompTypeGrid = 0; //Index for the type(category) of component
		private int intCompSelGrid = 0; //Index for selecting different components
		private int intAssembleType = 0; //Index for selecting which type of weapon to assemble
		private int intAssembleWeapon = 0; //Index for selecting which weapon to assemble
		private int intSelectedWeapon = 0; //The index of the selected weapon

		//Texture for the crosshair
		public Texture2D crosshairTexture;


		//Current value of the scroll bar
//    float vSbarValue = 0; //TODO Use the scroll bar to move through the inventory


		protected void Start ()
		{
				player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Unit> ();
				unitPlayer = player.GetComponent<UnitPlayer> ();

				//Assuming some sizes makes other calculations easier
				int intSlotWidth = 300; //The width of an item slot
				int intSlotHeight = 100; //The height of an item slot

				//Declare the options in laying out the inventory grid
				intInvSlotWidth = 150; //The width of an item slot
				intInvSlotHeight = 150; //The height of an item slot
		
				intInvItemsPerRow = Screen.width / intInvSlotWidth;
				intInvItemsPerCol = Screen.height / intInvSlotHeight;

				//Initialize component data structures
				int intNumWeapons = Enum.GetNames (typeof(ItemWeapon.tWeaponType)).Length - ItemWeapon.getNonCraftingWeapons ().Count;
				int intNumParts = Enum.GetNames (typeof(ItemComponent.tComponentPart)).Length;

				arrWepPartNames = new string[intNumWeapons * intNumParts];

				int intNumOres = Enum.GetNames (typeof(ItemComponent.tOreType)).Length;
				int intNumAtts = Enum.GetNames (typeof(ItemComponent.tAttributeType)).Length;

				arrAllComponents = new ItemComponent[intNumOres * intNumAtts];

				//Find all the weapon categories and add them to an array
				arrComponentGrids = new ItemSlot[arrWepPartNames.Length][,];
				int intWepCategoryindex = 0;
				foreach (ItemWeapon.tWeaponType wepType in (ItemWeapon.tWeaponType[]) Enum.GetValues(typeof(ItemWeapon.tWeaponType))) {
						if (ItemWeapon.getNonCraftingWeapons ().Contains (wepType))
								continue;
						//Weapon categories are a combination of the Weapon type and the weapon part e.g. sword blade and staff arrow
						foreach (ItemComponent.tComponentPart partType in (ItemComponent.tComponentPart[]) Enum.GetValues(typeof(ItemComponent.tComponentPart))) {
								string code = ItemComponent.getComponentCategoryCode (wepType, partType);
								string name = ItemComponent.getComponentName (code);

								arrWepPartNames [intWepCategoryindex] = name; //An array to store the names for the weapon parts

								//Get the craftable components based on which category we're in e.g. Lightened copper sword handle
								arrMakeableComps = new ItemSlot[intNumAtts, intNumOres - ItemBase.getNonCraftingOres ().Count]; //Account for non-useable types
								int intAttIndex = 0;
								foreach (ItemComponent.tAttributeType attType in (ItemComponent.tAttributeType[])Enum.GetValues(typeof(ItemComponent.tAttributeType))) {
										//Components will be a combination of the attribute, ore, weapon, and the part
										int intOreIndex = 0;
										foreach (ItemComponent.tOreType oreType in (ItemComponent.tOreType[])Enum.GetValues(typeof(ItemComponent.tOreType))) {
												//Ignore NOT_ORE & Stone types
												if (ItemBase.getNonCraftingOres ().Contains (oreType))
														continue;

												string newCompCode = ItemComponent.generateComponentCode (attType, oreType, wepType, partType);
												ItemComponent newComponent = ItemFactory.createComponent (newCompCode);

												ItemSlot slot = new ItemSlot ();
												slot.item = newComponent;

												//TODO Ask Ari what exact quantities should be needed
												slot.oreNeeded.oreType = oreType;
												slot.oreNeeded.Quantity = 3;

												slot.oreNeeded.neededPoints = (int)oreType;

												//Unlock the very first tier of components
												if (intOreIndex == 0)
														slot.unlocked = true;
												else
														slot.unlocked = false;

												arrMakeableComps [intAttIndex, intOreIndex] = slot;

												intOreIndex++;
										}
										intAttIndex++;
								}

								arrComponentGrids [intWepCategoryindex] = arrMakeableComps;
								intWepCategoryindex++; //The weapon category index is needed since crafting is done by selecting category and then attribute+ore
						}
				}

				//Get a list of weapon types to cycle through during assembling
				arrWeaponTypes = Enum.GetNames (typeof(ItemWeapon.tWeaponType));
		}

		/// <summary>
		/// Take care of switching through menus when keys are pressed and also moving inside of menus.
		/// </summary>
		protected void Update ()
		{
				//Toggle the appropriate menu
				if (InputContextManager.isMAIN_MENU_PUSHED ()) {
						if (menuCode == tMenuStates.MENU_MAIN)
								menuCode = tMenuStates.MENU_NONE;
						else
								menuCode = tMenuStates.MENU_MAIN;


				}

//				//Take care of movement inside menus
//				if (menuCode != tMenuStates.MENU_NONE) { //Menu to select menues
//						if (InputContextManager.isMENU_RIGHT ())
//								intMenusMenu = (arrMenusMenu.Length + intMenusMenu + 1) % arrMenusMenu.Length; //Loop to the beginning if we're at the end
//						else if (InputContextManager.isMENU_LEFT ())
//								intMenusMenu = intMenusMenu - 1 < 0 ? arrMenusMenu.Length - 1 : intMenusMenu - 1; //Loop to the end if we're at the beginning
//
//						menuCode = arrMenusMenu [intMenusMenu];
//				}

				switch (menuCode) {
				case tMenuStates.MENU_MAIN:
						{
								if (InputContextManager.isMENU_LEFT ())
										menuCode = tMenuStates.INVENTORY;
								else if (InputContextManager.isMENU_UP ())
										menuCode = tMenuStates.ASSEMBLING;
								else if (InputContextManager.isMENU_RIGHT ())
										menuCode = tMenuStates.CRAFTING;
								else if (InputContextManager.isITEM_MENU_PUSHED ())
										menuCode = tMenuStates.MENU_NONE;
								break;
						}

				//If the inventory is open, process movement inside the inventory
				case tMenuStates.INVENTORY:
						{
								//Process menu movement inside the inventory
								handleInventoryMovement ();

								//Process confirmations
								if (InputContextManager.isMENU_SELECT ())
										boolEquipWeapon = true;

								break;
						}
				case tMenuStates.CRAFTING:
						{
								handleCraftingMovement ();
								break;
						}
				case tMenuStates.ASSEMBLING:
						{
								handleAssembleMovement ();
								break;
						}
				}
		}

		void OnGUI ()
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

//				if (menuCode != tMenuStates.MENU_NONE) {
//						showMenuStates ();
//				}

				switch (menuCode) {
				case tMenuStates.MENU_MAIN: //Display context of which direction moves into which menu
						{
								int intMenuContextWidth = Screen.width / 6;
								int intMenuContextHeight = Screen.height / 6;

								GUIStyle style = new GUIStyle (GUI.skin.label);
								Texture2D tex2dButtonPassiveBack = new Texture2D (1, 1);
			
								tex2dButtonPassiveBack = (Texture2D)Resources.Load ("InventoryButtonBackground");
								style.normal.background = tex2dButtonPassiveBack;

								GUI.Label (new Rect ((Screen.width / 2) - (intMenuContextWidth / 2), 0, intMenuContextWidth, intMenuContextHeight), "Assemble", style);//Top
								GUI.Label (new Rect (0, (Screen.height / 2), intMenuContextWidth, intMenuContextHeight), "Inventory", style);//Left
								GUI.Label (new Rect ((Screen.width - intMenuContextWidth / 5), (Screen.height / 2), intMenuContextWidth, intMenuContextHeight), "Crafting", style); //Right

								break;
						}
			
				case tMenuStates.MENU_NONE: //Display regular player info
						{
								// Make a health bar
								GUI.Box (new Rect (10, 10, 100, 30), player.Health + "/" + player.MaxHealth);
								GUI.Box (new Rect (10, 40, 200, 30), "Crafting Points: " + player.Score);

								//Draw the crosshair
								Rect center = new Rect ((Screen.width - crosshairTexture.width) / 2,
			                  (Screen.height - crosshairTexture.height) / 2,
			                  crosshairTexture.width,
			                  crosshairTexture.height);
								GUI.DrawTexture (center, crosshairTexture);

								break;
						}
				case tMenuStates.INVENTORY:
						{
								layoutInventoryGrid ();

								break;
						}
				case tMenuStates.CRAFTING:
						{
								layoutCraftingGrid ();

								break;
						}
				case tMenuStates.ASSEMBLING:
						{
								layoutAssembleGrid ();
                
								break;
						}
				}
		}

		private void showMenuStates ()
		{
				UnityEngine.GUIStyle style = new GUIStyle (GUI.skin.button);
				Texture2D tex2dButtonPassiveBack = new Texture2D (1, 1);

				tex2dButtonPassiveBack = (Texture2D)Resources.Load ("InventoryTypeBackground");
				style.normal.background = tex2dButtonPassiveBack;

				GUI.Label (new Rect (0, 0, 150, 50), arrMenusMenu [intMenusMenu].ToString (), style);
		}

		private void layoutAssembleGrid ()
		{
				int intAssembleWidthPadding = Screen.width / 8;
				int intAssembleHeightPadding = Screen.height / 8;

				Texture2D tex2dButtonPassiveBack = new Texture2D (1, 1);
				Texture2D tex2dButtonActiveBack = new Texture2D (1, 1);
				Texture2D tex2dButtonFlashBack = new Texture2D (1, 1);
				UnityEngine.GUIStyle style = new GUIStyle (GUI.skin.button);

				//Set the style for selection screens
				tex2dButtonPassiveBack = (Texture2D)Resources.Load ("InventoryTypeBackground");
				
				//Backgrounds for non-active items
				style.normal.background = tex2dButtonPassiveBack;
				style.hover.background = tex2dButtonPassiveBack;
				style.onHover.background = tex2dButtonPassiveBack;
		
				//Make a label to show which kind of weapon is being assembled
				int intAssemWeaponLabelWidth = 2 * intAssembleWidthPadding;
				int intAssemWeaponLabelHeight = intAssembleHeightPadding;
				int intAssemWeaponLabelX = (Screen.width / 2) - (intAssemWeaponLabelWidth / 2); //Start the label at half the screen shifted by half the label width
				int intAssemWeaponLabelY = (Screen.height / 20);

				GUI.Label (new Rect (intAssemWeaponLabelX, intAssemWeaponLabelY, intAssemWeaponLabelWidth, intAssemWeaponLabelHeight), arrWeaponTypes [intAssembleType], style);

				ArrayList arrListAssemblable = getMakeableItems ();
				ArrayList temp = new ArrayList ();

				//Filter the makeable items by their type
				for (int i = 0; i < arrListAssemblable.Count; i++) {
						ItemWeapon wepCurrent = ItemFactory.createWeapon (((ItemComponent[])arrListAssemblable [i]) [0], ((ItemComponent[])arrListAssemblable [i]) [1]);

						if (wepCurrent.weaponType.ToString ().Equals (arrWeaponTypes [intAssembleType]))
								temp.Add (arrListAssemblable [i]);
				}

				//Copy the valid items into an array
				string[] arrAssembleStrings = new string[temp.Count];
				arrAssembleWeapons = new ItemComponent[temp.Count][];
				for (int i = 0; i < temp.Count; i ++) {
						arrAssembleWeapons [i] = (ItemComponent[])temp [i];
						arrAssembleStrings [i] = ItemFactory.createWeapon (arrAssembleWeapons [i] [0], arrAssembleWeapons [i] [1]).name;
				}

				intCompTypeGrid = GUI.SelectionGrid (new Rect (intAssembleWidthPadding, intAssemWeaponLabelY + intAssemWeaponLabelHeight + 10,
		                                             6 * intAssembleWidthPadding, 6 * intAssembleHeightPadding), 
		                                    intAssembleWeapon, arrAssembleStrings, 3, style);

		}

		/// <summary>
		/// Get a list of ItemWeapons which can be made from the components in the inventory.
		/// </summary>
		/// <returns>The makeable items.</returns>
		private ArrayList getMakeableItems ()
		{
				ArrayList arrListComponents = Inventory.getInstance ().getInventoryComponents ();
				ArrayList arrListResults = new ArrayList ();

				//Loop through all pairs of items and see if they can be combined
				for (int i = 0; i < arrListComponents.Count; i++) {
						for (int j = i + 1; j < arrListComponents.Count; j++) {
								//Create weapon returns null when the components given are unable to form a proper weapon
								ItemWeapon potentialWeapon = ItemFactory.createWeapon ((ItemComponent)arrListComponents [i], (ItemComponent)arrListComponents [j]);
								if (potentialWeapon != null) {
										//TODO Use a tuple instead of an array w/ 2 items. I think the compiler won't compile against .net 4.0+
										arrListResults.Add (new ItemComponent[2] {
												(ItemComponent)arrListComponents [i],
												(ItemComponent)arrListComponents [j]
										});
								}
						}
				}

				return arrListResults;
		}

		private void layoutCraftingGrid ()
		{
				Texture2D tex2dButtonPassiveBack = new Texture2D (1, 1);
				Texture2D tex2dButtonActiveBack = new Texture2D (1, 1);
				Texture2D tex2dButtonFlashBack = new Texture2D (1, 1);

				//Set the style for selection screens
				//Currently, can't initialize a style outside of On GUI. Find a  way to call this outside of OnGUI for efficiency
				UnityEngine.GUIStyle style = new GUIStyle (GUI.skin.button);
				tex2dButtonPassiveBack = (Texture2D)Resources.Load ("InventoryButtonBackground");
				tex2dButtonActiveBack = (Texture2D)Resources.Load ("SelectedBackground");
				tex2dButtonFlashBack = (Texture2D)Resources.Load ("SelectedBackgroundFlash");

				//Backgrounds when I have an active selection
				style.active.background = tex2dButtonActiveBack;
				style.focused.background = tex2dButtonActiveBack;
				style.onFocused.background = tex2dButtonActiveBack;
				style.onNormal.background = tex2dButtonActiveBack;

				//Backgrounds for non-active items
				style.normal.background = tex2dButtonPassiveBack;
				style.hover.background = tex2dButtonPassiveBack;
				style.onHover.background = tex2dButtonPassiveBack;

				//Word wrap around inside of a box
				style.wordWrap = true;

				//Backgrounds for selecting an item while the button is still being pressed
				style.onActive.background = tex2dButtonFlashBack;

				//TODO Draw background for menu
				//		GUI.Box (new Rect (0, 0, Screen.width, Screen.height), (Texture2D)Resources.Load("SelectedBackground"), style);

				//Initialize options for the menus
				int intWidthPadding = Screen.width / 15; //(1/15) of the screen for width padding
				int intHeightPadding = Screen.height / 8; //(1/8) of the screen for height padding
//		int intCompTypeWidth = (Screen.width - (2 * intWidthPadding)) / 4; //(Screen width - padding on both sides) is how big the components type menu should be

				vec2CompTypeStart = new Vector2 (intWidthPadding, (intHeightPadding));
				vec2CompTypeDimensions = new Vector2 (2.5f * intWidthPadding, Screen.height - (2f * intHeightPadding));

//        intCompTypeGrid = GUI.SelectionGrid(new Rect(vec2CompTypeStart.x, vec2CompTypeStart.y,  vec2CompTypeDimensions.x, vec2CompTypeDimensions.y),
//		                                    intCompTypeGrid, arrComponents, 1, style);
				//List of categories
				intCompTypeGrid = GUI.SelectionGrid (new Rect (vec2CompTypeStart.x, vec2CompTypeStart.y, vec2CompTypeDimensions.x, vec2CompTypeDimensions.y),
		                                    intCompTypeGrid, arrWepPartNames, 1, style);

				//List of components
				ItemSlot[,] selectedComponents = arrComponentGrids [intCompTypeGrid];

				int intNumItems = selectedComponents.Length;
				int intNumAtts = selectedComponents.GetLength (0);
				int intNumOres = selectedComponents.GetLength (1);

				//Loop through our 2d array of components and store the names in a 1d arary to display
				arrSelectedComponentNames = new string[intNumItems];

				for (int i = 0; i < intNumAtts; i++) {
						for (int j=0; j < intNumOres; j++) {
								//Do a little math to translate from (0,0) in the top left and indexes increasing right to
								//	(0,0) in botton left and indexes increasing up. In other words, rotate the array ccw by 90.

								if (selectedComponents [i, j].unlocked)
										arrSelectedComponentNames [getIndexFromCoordinate (i, j, intNumItems, intNumAtts)] = selectedComponents [i, j].item.ToString ();
								else
										arrSelectedComponentNames [getIndexFromCoordinate (i, j, intNumItems, intNumAtts)] = "?";

//				arrSelctedComponentNames[intNumItems - (j * intNumAtts) - (intNumAtts - i)] = selectedComponents[i,j].ToString();

						}
				}

				//Handle selection grid stuff
				intCompSelGrid = GUI.SelectionGrid (new Rect (4.5f * intWidthPadding, intHeightPadding,
		                                            (6f * intWidthPadding), Screen.height - (2 * intHeightPadding)),
		                                   intCompSelGrid, arrSelectedComponentNames, intNumAtts, style);


				try {
						//Description Area
						Vector2 vec2Description = getComponentCoordinateFromIndex (intCompSelGrid);
						//Only show the description if we've unlocked the item
						string description = "Unlock me by crafting this component in a lower tier";
				

						if (vec2Description.x >= 0 && vec2Description.y >= 0 && 
								selectedComponents [(int)vec2Description.x, (int)vec2Description.y].unlocked) {
								ItemSlot selectedComponent = selectedComponents [(int)vec2Description.x, (int)vec2Description.y]; 

								description = selectedComponents [(int)vec2Description.x, (int)vec2Description.y].item.getDescription ();
								description += "\n";
								description += "This component requires " + selectedComponent.oreNeeded.Quantity + " pieces of " + selectedComponent.oreNeeded.oreType;
								description += "\n";
								description += "You currently have " + inventory.getOreQuantity (selectedComponent.oreNeeded.oreType) + " pieces of " + selectedComponent.oreNeeded.oreType;
						}

						GUI.Label (new Rect (11 * intWidthPadding, vec2CompTypeStart.y,
		                   	vec2CompTypeDimensions.x, vec2CompTypeDimensions.y),
		           description, style);
				} catch (IndexOutOfRangeException e) {
						Vector2 vec2Description = getComponentCoordinateFromIndex (intCompSelGrid);
						Debug.Log ("X: " + vec2Description.x);
						Debug.Log ("Y: " + vec2Description.x);
				}
		}

		private Vector2 getComponentCoordinateFromIndex (int index)
		{
				//Assume that all weapon types have the same # of cols a.k.a same # of attributes
				int cols = (Enum.GetNames (typeof(ItemComponent.tAttributeType))).Length;
				int tiers = (Enum.GetNames (typeof(ItemBase.tOreType))).Length - ItemBase.getNonCraftingOres ().Count - 1; //-1 to translate to 0 based indexing
				int x = index % cols;
				int y = (tiers - (int)(index / cols));

				return new Vector2 (x, y);
		}

		private int getIndexFromCoordinate (int x, int y, int intNumItems, int intNumCols)
		{
				return (intNumItems - (y * intNumCols) - (intNumCols - x));
		}

		private void layoutInventoryGrid ()
		{
				//TODO Show equipped items. Helmet on top, sword and armor in the middle, and boots @ bottom
//        int intWidthPadding = Screen.width / 8;
//        int intHeightPadding = Screen.height / 8;

				//The starting position of the inventory
				Vector2 vec2CurrentPos = new Vector2 (0, 100);

				//Find the inventory
				Inventory inventory = Inventory.getInstance ();

				//TODO Allow scrolling down when weapons don't all fit on the screen

				ArrayList arrListWeapons = inventory.getInventoryWeapons ();
				ArrayList arrArmors = inventory.getInventoryArmors ();
				ArrayList arrComponents = inventory.getInventoryComponents ();
				ArrayList arrItems = inventory.getInventoryItems ();
				ArrayList arrOres = inventory.getInventoryOres ();

				ArrayList allItems = new ArrayList ();

				foreach (ItemWeapon weapon in arrListWeapons) {
						allItems.Add (weapon);
				}

				foreach (ItemEquipment armor in arrArmors) {
						allItems.Add (armor);
				}

				foreach (ItemBase item in arrItems) {
						allItems.Add (item);
				}

				foreach (ItemBase component in arrComponents) {
						allItems.Add (component);
				}

				foreach (ItemOre ore in arrOres) {
						allItems.Add (ore);
				}

				string[] arrStringWeapons = new string[allItems.Count];

				for (int i = 0; i < allItems.Count; i++) {
						//ItemWeapon wep = (ItemWeapon)arrListWeapons[i];
						ItemBase wep = (ItemBase)allItems [i];
						//Grab the names of the item associated with the weapon
						arrStringWeapons [i] = wep.ToString (); //To instead use pictures/textures, make an array of pictures/textures
				}

				UnityEngine.GUIStyle style = new GUIStyle (GUI.skin.button);
				Texture2D tex2dButtonPassiveBack = new Texture2D (1, 1);
				Texture2D tex2dButtonActiveBack = new Texture2D (1, 1);
				tex2dButtonPassiveBack = (Texture2D)Resources.Load ("InventoryButtonBackground");
				tex2dButtonActiveBack = (Texture2D)Resources.Load ("SelectedBackground");
		
				//Backgrounds for non-active items
				style.normal.background = tex2dButtonPassiveBack;
				style.wordWrap = true;
		
				//Backgrounds when I have an active selection
				style.onNormal.background = tex2dButtonActiveBack;


				intSelectedWeapon = GUI.SelectionGrid (new Rect (vec2CurrentPos.x, vec2CurrentPos.y, Screen.width, intInvSlotHeight),
                                              intSelectedWeapon, arrStringWeapons, intInvItemsPerRow, style);

				if (boolEquipWeapon) {
						//Not allowing equipping of things other than weapons by bounding the index
						unitPlayer.equipWeapon (((ItemWeapon)arrListWeapons [intSelectedWeapon]).weaponType.ToString ());
						boolEquipWeapon = false;
				}

				//      // Uncomment this out if we want to use this. A.K.A ALTERNATIVE SCHEME
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

		private void handleAssembleMovement ()
		{
				//Left and right change the type being assembled. (With wraparound)
				if (InputContextManager.isMENU_LEFT ()) {
						int intNewType = intAssembleType - 1;

						intAssembleType = (intNewType < 0 ? arrWeaponTypes.Length - 1 : intNewType);
				} else if (InputContextManager.isMENU_RIGHT ()) {
						int intNewType = intAssembleType + 1;

						intAssembleType = (intNewType > arrWeaponTypes.Length - 1 ? 0 : intNewType);
				}
					//Up and down change which item to assemble (without wraparound)
			else if (InputContextManager.isMENU_UP ()) {
						intAssembleWeapon = Math.Max (intAssembleWeapon - 1, 0);
				} else if (InputContextManager.isMENU_DOWN ()) {
						intAssembleWeapon = Math.Min (arrAssembleWeapons.Length - 1, intAssembleWeapon + 1);
				} else if (InputContextManager.isMENU_SELECT ()) {
						//Time to craft an item

						Inventory i = Inventory.getInstance ();

						//Add the item to the player's inventory
						i.inventoryAddItem (ItemFactory.createWeapon (arrAssembleWeapons [intAssembleWeapon] [0], arrAssembleWeapons [intAssembleWeapon] [1]));

						//Remove the components from the inventory
						i.inventoryRemoveItem (arrAssembleWeapons [intAssembleWeapon] [0]);
						i.inventoryRemoveItem (arrAssembleWeapons [intAssembleWeapon] [1]);

						//Reset the component index
						intAssembleWeapon = 0;
				}
		}

		private void handleCraftingMovement ()
		{
				//Take care of menu navigation from the buttons
				if (InputContextManager.isMENU_UP ()) {
						//move up in the respective menu
						if (intCompSelGrid > 0) { //Component selection menu
								int intNewSelection = intCompSelGrid - intCompSelCols;
								intCompSelGrid = Math.Max (intNewSelection, 0);
						} else { //Component type menu
								int intNewSelection = intCompTypeGrid - 1;
								intCompTypeGrid = Math.Max (intNewSelection, 0);

						}
				} else if (InputContextManager.isMENU_RIGHT ()) {
						//move right
						//if i'm in the component type menu, switch over to the components
						if (intCompSelGrid >= 0) {
								int intNewSelection = intCompSelGrid + 1;
								intCompSelGrid = Math.Min (intNewSelection, arrSelectedComponentNames.Length - 1);
						} else { //I'm in the component type menu
								intCompSelGrid = 0;
						}
				} else if (InputContextManager.isMENU_DOWN ()) {
						//TODO Move to the next col instead of to the end
						//Move down in the respective menu
						if (intCompSelGrid >= 0) {
								int intNewSelection = intCompSelGrid + intCompSelCols;

								//If we need to move to the top of the next col
								if (intNewSelection > arrSelectedComponentNames.Length - 1 &&
										intCompSelGrid != arrSelectedComponentNames.Length - 1) { 
										intNewSelection = (intNewSelection % intCompSelCols) + 1;
								}

								intCompSelGrid = Math.Min (arrSelectedComponentNames.Length - 1, intNewSelection);
						} else { //I'm in the component type menu
								int intNewSelection = intCompTypeGrid + 1;
								intCompTypeGrid = intNewSelection % (arrWepPartNames.Length);
						}
				} else if (InputContextManager.isMENU_LEFT ()) {
						//move left
						//if i'm in the components menu, switch over to the component type menu
						if (intCompSelGrid >= 0) {
								int intNewSelection = intCompSelGrid - 1;
								if ((intCompSelGrid + intCompSelCols) % intCompSelCols == 0)
										intNewSelection = -1;

								intCompSelGrid = intNewSelection;
						} else { //I'm in the component type menu
								//Don't do anything
						}

				} else if (InputContextManager.isMENU_SELECT ()) {
						//TODO Make the button be selected when the confirm key is pressed
//			SendMessage("onActive");

						//Translate what is being selected
						Vector2 vec2SelectedComponent = getComponentCoordinateFromIndex (intCompSelGrid);

						//Get the item slot of what we want
						ItemSlot desired = arrComponentGrids [intCompTypeGrid] [(int)vec2SelectedComponent.x, (int)vec2SelectedComponent.y];

						//Only allow crafting if the player has unlocked the item
						if (!desired.unlocked)
								return;

						//Make sure the player has the necessary amounts of ore
						if (!playerCanCraft (desired))
								return;

						//Get the new component
						ItemComponent cmpNew = (ItemComponent)(desired.item);

						//Unlock the crafted component that is one tier above
						try {
								arrComponentGrids [intCompTypeGrid] [(int)vec2SelectedComponent.x, (int)vec2SelectedComponent.y + 1].unlocked = true;
						} catch (IndexOutOfRangeException e) {
								//Ignore index out of range exceptions. Basically a lazy way to handle unlocking items at the max tier
						}

						inventory.inventoryAddItem (cmpNew);
				}
		}

		private void handleInventoryMovement ()
		{
				if (InputContextManager.isMENU_UP ()) {
						//Decrement the index by itemsPerRow and take the maximum of 0 and newIndex
						int intNewSelected = intSelectedWeapon - intInvItemsPerRow;
			
						intSelectedWeapon = Mathf.Max (0, intNewSelected);
				} else if (InputContextManager.isMENU_DOWN ()) {
						//Increment the index by itemsPerRow and take the min of arrListWeapons.Count and newIndex
						int intNewSelected = intSelectedWeapon + intInvItemsPerRow;
			
						Inventory inventory = Inventory.getInstance ();
			
						ArrayList arrListWeapons = inventory.getInventoryWeapons ();
						intSelectedWeapon = Mathf.Min (arrListWeapons.Count - 1, intNewSelected);
				} else if (InputContextManager.isMENU_LEFT ()) {
						//Decrement the index by 1 and take the maximum of 0 and newIndex
						int intNewSelected = intSelectedWeapon - 1;
			
						//Bound checking
						intSelectedWeapon = Mathf.Max (intNewSelected, 0);
				} else if (InputContextManager.isMENU_RIGHT ()) {
						//Increment the index by 1 and take the min of arrListWeapons.Count and newIndex
						int intNewSelected = intSelectedWeapon + 1;
			
						Inventory inventory = Inventory.getInstance ();
			
						ArrayList arrListWeapons = inventory.getInventoryWeapons ();
			
						//Pressing right can wraparound to the next row if one exists
						intSelectedWeapon = Mathf.Min (intNewSelected, arrListWeapons.Count - 1);
				}
		}

		private bool playerCanCraft (ItemSlot desired)
		{
				ArrayList arrOres = inventory.getInventoryOres ();
				bool hasOres = false;
				foreach (ItemOre ore in arrOres) {
						if (ore.oreType.Equals (desired.oreNeeded.oreType)) {
								if (ore.Quantity >= desired.oreNeeded.Quantity) {
										ItemOre oreToRemove = new ItemOre (ore.oreType);
										oreToRemove.Quantity = desired.oreNeeded.Quantity;
										inventory.inventoryRemoveItem (oreToRemove, desired.oreNeeded.Quantity);
					
										hasOres = true;
										break;
								}
						}
				}

				bool hasPoints = desired.oreNeeded.neededPoints >= player.Score;

				return hasOres & hasPoints;
		}

}