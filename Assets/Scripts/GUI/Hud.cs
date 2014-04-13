using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Hud : MonoBehaviour
{
    //A private class to represent an inventory slot
    private class ItemSlot
    {
        public ItemBase item;
        public bool unlocked;
        public ItemOre oreNeeded;

        public ItemSlot()
        {
            item = null;
            unlocked = false;

            oreNeeded = new ItemOre(ItemBase.tOreType.NOT_ORE);
            oreNeeded.Quantity = 0;
            oreNeeded.neededPoints = 0;
        }
    }

    //Declare instance variables
    public enum tMenuStates
    {
        MENU_NONE = 0,
        INVENTORY = 1,
        CRAFTING = 2,
        ASSEMBLING = 3,
        MENU_MAIN = 4,
        GAME_OVER = 5,
    }

    public static tMenuStates menuCode = tMenuStates.MENU_NONE;

    //Keep a reference to the player
    Unit player;
    UnitPlayer unitPlayer;

    //Keep a reference to the inventory
    Inventory inventory = Inventory.getInstance();

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
		tMenuStates.ASSEMBLING,
	};

    //Options for laying out the grid
    readonly int screenWidth = (int)(Screen.width * 0.8);
    readonly int screenHeight = (int)(Screen.height * 0.8);
    //Take half of (100 - scale factor) and add that to 0 for the new start positions 
    readonly int screenX0 = (int)(Screen.width * 0.1);
    readonly int screenY0 = (int)(Screen.height * 0.1);

    int intInvSlotWidth; //The width of an inventory item slot
    int intInvSlotHeight; //The height of an inventory item slot
    int intInvItemsPerRow;
    int intInvItemsPerCol;
    int intCompTypeWidth;
    int intCompSelCols = (Enum.GetNames(typeof(ItemComponent.tAttributeType)).Length);
    int intCompSelRows = (Enum.GetNames(typeof(ItemBase.tOreType)).Length - ItemBase.getNonCraftingOres().Count);

    //Vector 2's to store some x and y components
    Vector2 vec2CompTypeStart;
    Vector2 vec2CompTypeDimensions;

    //Indexes for the selections
    private int intMenusMenu = 0; //Index for which menu is selected
    private int intCompTypeGrid = 0; //Index for the type(category) of component
    private int intCompSelGrid = 0; //Index for selecting different components
    private int intAssembleType = 0; //Index for selecting which type of weapon to assemble
    private int intAssembleWeapon = 0; //Index for selecting which weapon to assemble
    private int intInventoryItem = 0; //The index of the selected inventory item

    //Texture for the crosshair
    public Texture2D crosshairTexture;

    protected void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();
        unitPlayer = player.GetComponent<UnitPlayer>();

        //Assuming some sizes makes other calculations easier
        int intSlotWidth = 300; //The width of an item slot
        int intSlotHeight = 100; //The height of an item slot

        //Declare the options in laying out the inventory grid
        intInvSlotWidth = 150; //The width of an item slot
        intInvSlotHeight = 150; //The height of an item slot

        intInvItemsPerRow = screenWidth / intInvSlotWidth;
        intInvItemsPerCol = screenHeight / intInvSlotHeight;

        //Initialize component data structures
        int intNumWeapons = Enum.GetNames(typeof(ItemWeapon.tWeaponType)).Length - ItemWeapon.getNonCraftingWeapons().Count;
        int intNumParts = Enum.GetNames(typeof(ItemComponent.tComponentPart)).Length;

        arrWepPartNames = new string[intNumWeapons * intNumParts];

        int intNumOres = Enum.GetNames(typeof(ItemComponent.tOreType)).Length;
        int intNumAtts = Enum.GetNames(typeof(ItemComponent.tAttributeType)).Length;

        arrAllComponents = new ItemComponent[intNumOres * intNumAtts];

        //Find all the weapon categories and add them to an array
        arrComponentGrids = new ItemSlot[arrWepPartNames.Length][,];
        int intWepCategoryindex = 0;
        foreach (ItemWeapon.tWeaponType wepType in (ItemWeapon.tWeaponType[])Enum.GetValues(typeof(ItemWeapon.tWeaponType)))
        {
            if (ItemWeapon.getNonCraftingWeapons().Contains(wepType))
                continue;
            //Weapon categories are a combination of the Weapon type and the weapon part e.g. sword blade and staff arrow
            foreach (ItemComponent.tComponentPart partType in (ItemComponent.tComponentPart[])Enum.GetValues(typeof(ItemComponent.tComponentPart)))
            {
                string code = ItemComponent.getComponentCategoryCode(wepType, partType);
                string name = ItemComponent.getComponentName(code);

                arrWepPartNames[intWepCategoryindex] = name; //An array to store the names for the weapon parts

                //Get the craftable components based on which category we're in e.g. Lightened copper sword handle
                arrMakeableComps = new ItemSlot[intNumAtts, intNumOres - ItemBase.getNonCraftingOres().Count]; //Account for non-useable types
                int intAttIndex = 0;
                foreach (ItemComponent.tAttributeType attType in (ItemComponent.tAttributeType[])Enum.GetValues(typeof(ItemComponent.tAttributeType)))
                {
                    //Components will be a combination of the attribute, ore, weapon, and the part
                    int intOreIndex = 0;
                    foreach (ItemComponent.tOreType oreType in (ItemComponent.tOreType[])Enum.GetValues(typeof(ItemComponent.tOreType)))
                    {
                        //Ignore NOT_ORE & Stone types
                        if (ItemBase.getNonCraftingOres().Contains(oreType))
                            continue;

                        string newCompCode = ItemComponent.generateComponentCode(attType, oreType, wepType, partType);
                        ItemComponent newComponent = ItemFactory.createComponent(newCompCode);

                        ItemSlot slot = new ItemSlot();
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

                        arrMakeableComps[intAttIndex, intOreIndex] = slot;

                        intOreIndex++;
                    }
                    intAttIndex++;
                }

                arrComponentGrids[intWepCategoryindex] = arrMakeableComps;
                intWepCategoryindex++; //The weapon category index is needed since crafting is done by selecting category and then attribute+ore
            }
        }

        //Get a list of weapon types to cycle through during assembling
        arrWeaponTypes = Enum.GetNames(typeof(ItemWeapon.tWeaponType));
    }

    /// <summary>
    /// Take care of switching through menus when keys are pressed and also moving inside of menus.
    /// </summary>
    protected void Update()
    {
        //Toggle the appropriate menu
        if (InputContextManager.isMAIN_MENU_PUSHED())
        {
            if (menuCode == tMenuStates.MENU_MAIN)
                menuCode = tMenuStates.MENU_NONE;
            else
                menuCode = tMenuStates.MENU_MAIN;
        }

        switch (menuCode)
        {
            case tMenuStates.MENU_MAIN:
                {
                    if (InputContextManager.isMENU_LEFT())
                        menuCode = tMenuStates.CRAFTING;
                    else if (InputContextManager.isMENU_UP())
                        menuCode = tMenuStates.INVENTORY;
                    else if (InputContextManager.isMENU_RIGHT())
                        menuCode = tMenuStates.ASSEMBLING;
                    else if (InputContextManager.isITEM_MENU_PUSHED())
                        menuCode = tMenuStates.MENU_NONE;
                    break;
                }

            //If the inventory is open, process movement inside the inventory
            case tMenuStates.INVENTORY:
                {
                    if (InputContextManager.isMENU_SWITCH_LEFT())
                    {
                        menuCode = tMenuStates.CRAFTING;
                        break;
                    }
                    else if(InputContextManager.isMENU_SWITCH_RIGHT())
                    {
                        menuCode = tMenuStates.ASSEMBLING;
                        break;
                    }

                    //Process menu movement inside the inventory
                    handleInventoryMovement();

                    break;
                }
            case tMenuStates.CRAFTING:
                {
                    if (InputContextManager.isMENU_SWITCH_LEFT())
                    {
                        menuCode = tMenuStates.ASSEMBLING;
                        break;
                    }
                    else if (InputContextManager.isMENU_SWITCH_RIGHT())
                    {
                        menuCode = tMenuStates.INVENTORY;
                        break;
                    }

                    handleCraftingMovement();
                    break;
                }
            case tMenuStates.ASSEMBLING:
                {
                    if (InputContextManager.isMENU_SWITCH_LEFT())
                    {
                        menuCode = tMenuStates.INVENTORY;
                        break;
                    }
                    else if (InputContextManager.isMENU_SWITCH_RIGHT())
                    {
                        menuCode = tMenuStates.CRAFTING;
                        break;
                    }
                    handleAssembleMovement();
                    break;
                }
        }
    }

    void OnGUI()
    {

        switch (menuCode)
        {
            case tMenuStates.MENU_MAIN: //Display context of which direction moves into which menu
                {
                    int intMenuContextWidth = screenWidth / 8;
                    int intMenuContextHeight = screenHeight / 8;

                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    Texture2D tex2dButtonPassiveBack = new Texture2D(1, 1);

                    tex2dButtonPassiveBack = (Texture2D)Resources.Load("InventoryButtonBackground");
                    style.normal.background = tex2dButtonPassiveBack;

                    GUI.BeginGroup(new Rect((Screen.width/ 2) - (intMenuContextWidth / 2), (screenHeight / 2) - (intMenuContextHeight / 2),
                                            2 * intMenuContextWidth , 3 * intMenuContextHeight), style);

                    //GUI.Label(new Rect((screenWidth / 2) - (intMenuContextWidth / 2), 0, intMenuContextWidth, intMenuContextHeight), "Assemble", style);//Top
                    //GUI.Label(new Rect(0, (screenHeight / 2), intMenuContextWidth, intMenuContextHeight), "Inventory", style);//Left
                    //GUI.Label(new Rect((screenWidth - intMenuContextWidth / 5), (screenHeight / 2), intMenuContextWidth, intMenuContextHeight), "Crafting", style); //Right
                    //GUI.Label(new Rect(0, 0, 0, 0), "Armor", style);

                    GUI.Label(new Rect(0.5f * intMenuContextWidth, 0, intMenuContextWidth, intMenuContextHeight), "Inventory", style);//Top
                    GUI.Label(new Rect(0, intMenuContextHeight, intMenuContextWidth, intMenuContextHeight), "Crafting", style);//Left
                    GUI.Label(new Rect(intMenuContextWidth, intMenuContextHeight, intMenuContextWidth, intMenuContextHeight), "Assembling", style); //Right
                    //GUI.Label(new Rect(0.5f * intMenuContextWidth, 2.0f * intMenuContextHeight, intMenuContextWidth, intMenuContextHeight), "Options"); //Down  

                    GUI.EndGroup();

                    break;
                }

            case tMenuStates.MENU_NONE: //Display regular player info
                {
                    // Make a health bar
                    GUI.Box(new Rect(screenX0 + 10, screenY0 + 10, 100, 30), player.Health + "/" + player.MaxHealth);
                    GUI.Box(new Rect(screenX0 + 10, screenY0 + 40, 200, 30), "Crafting Points: " + player.Score);

                    //Draw the crosshair
                    Rect center = new Rect((Screen.width - crosshairTexture.width) / 2,
                  (Screen.height - crosshairTexture.height) / 2,
                  crosshairTexture.width,
                  crosshairTexture.height);
                    GUI.DrawTexture(center, crosshairTexture);

                    break;
                }
            case tMenuStates.INVENTORY:
                {
                    layoutInventoryGrid();

                    break;
                }
            case tMenuStates.CRAFTING:
                {
                    layoutCraftingGrid();

                    break;
                }
            case tMenuStates.ASSEMBLING:
                {
                    layoutAssembleGrid();

                    break;
                }

        }
    }

    //Variables for crafting armor
    Vector2 itemScrollPosition = Vector2.zero;
    int intCraftingCategory = 0;
    int intTypesInCategory = 0;
    int intCraftingAttributes = 0;
    int intCraftingOres = 0;

    //Categories we can craft
    List<ItemBase.tItemType> craftableCategories = new List<ItemBase.tItemType> { ItemBase.tItemType.Armor, ItemBase.tItemType.Component };

    //List of types of armors
    List<ItemArmor.tArmorPart> armors = new List<ItemArmor.tArmorPart> { ItemArmor.tArmorPart.Chest, ItemArmor.tArmorPart.Head, ItemArmor.tArmorPart.Legs };

    //List of types of weapon components
    List<ItemComponent.tComponentPart> components = new List<ItemComponent.tComponentPart> { ItemComponent.tComponentPart.Handle, ItemComponent.tComponentPart.Blade };

    //An untyped list that will either reference amors or components. Basically this list acts as context to whichever this list gets referenced to
    IList craftingTypeInCategory;

    //List armor attributes
    List<ItemArmor.tAttributeType> craftingAttributes = new List<ItemArmor.tAttributeType> { ItemArmor.tAttributeType.Heavy, ItemArmor.tAttributeType.Normal, ItemArmor.tAttributeType.Light };

    //List ores that armors can be made from
    List<ItemBase.tOreType> craftingOres = new List<ItemBase.tOreType>();

    private void layoutCraftingGrid()
    {
        craftingOres = new List<ItemBase.tOreType>();

        foreach (ItemBase.tOreType ore in Enum.GetValues(typeof(ItemBase.tOreType)))
        {
            if (ItemBase.getNonCraftingOres().Contains(ore))
                continue;

            craftingOres.Add(ore);
        }

        //Layout variables
        GUIStyle infoStyle = new GUIStyle(GUI.skin.label);
        infoStyle.alignment = TextAnchor.UpperCenter;
        infoStyle.fontSize = 16;

        //Split the screen into 4 groups with padding on the edges(1 groups worth)
        int labelHeight = 100;
        int groupWidth = screenWidth / 5;
        int groupHeight = screenHeight - labelHeight;

        //Description Area (non-active)
        GUIStyle categoryStyle = new GUIStyle(GUI.skin.label);
        categoryStyle.alignment = TextAnchor.MiddleCenter;
        categoryStyle.fontStyle = FontStyle.Bold;
        categoryStyle.fontSize = 20;

        Texture2D tex2dLabelBack = new Texture2D(1, 1);
        tex2dLabelBack = (Texture2D)Resources.Load("InventoryButtonBackground");
        //Backgrounds for non-active items
        categoryStyle.normal.background = tex2dLabelBack;
        categoryStyle.wordWrap = true;

        //Description Area(active)
        GUIStyle categoryStyleActive = new GUIStyle(GUI.skin.label);
        categoryStyleActive.alignment = TextAnchor.MiddleCenter;
        categoryStyleActive.fontStyle = FontStyle.Bold;
        categoryStyleActive.fontSize = 20;

        Texture2D tex2dLabelBackSelected = new Texture2D(1, 1);
        tex2dLabelBackSelected = (Texture2D)Resources.Load("SelectedBackground");

        //Background for active items
        categoryStyleActive.normal.background = tex2dLabelBackSelected;
        categoryStyleActive.wordWrap = true;

        //Info at the Bottom of the screen.
        GUI.BeginGroup(new Rect(screenX0, screenY0 + groupHeight, screenWidth, labelHeight));
        //TODO Make labels work for OUYA Controls            
        GUI.Label(new Rect(0, 0, screenWidth, screenHeight), "Make all your selections and then confirm." + "\n" +
            "Change option group: Left,Right" + "\n" +
            "Switch Option: Up, Down" + "\n" +
            "Confirm:Enter", infoStyle);
        GUI.EndGroup();


        //Category Groups
        GUI.BeginGroup(new Rect(screenX0, screenY0, groupWidth, groupHeight));
        if(craftingState.Equals(tCraftingState.CATEGORY_SELECTION))
            GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftableCategories[intCraftingCategory].ToString(), categoryStyleActive);
        else
            GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftableCategories[intCraftingCategory].ToString(), categoryStyle);
        GUI.EndGroup();


        //Items from the selected category
        GUI.BeginGroup(new Rect(1 * groupWidth + screenX0, screenY0, groupWidth, groupHeight));
        if (craftableCategories[intCraftingCategory].Equals(ItemBase.tItemType.Armor))
            craftingTypeInCategory = armors;
        else
            craftingTypeInCategory = components;
        
        //Make sure when switching between categories, we readjust the max option index if we need to
        intTypesInCategory = Math.Min(intTypesInCategory, craftingTypeInCategory.Count - 1);
        if(craftingState.Equals(tCraftingState.ITEM_TYPE))
            GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftingTypeInCategory[intTypesInCategory].ToString(), categoryStyleActive);
        else
            GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftingTypeInCategory[intTypesInCategory].ToString(), categoryStyle);
        GUI.EndGroup();


        //Attribute choice for your item
        GUI.BeginGroup(new Rect(2 * groupWidth + screenX0, screenY0, groupWidth, groupHeight));
        if(craftingState.Equals(tCraftingState.ATTRIBUTE_SELECTION))
            GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftingAttributes[intCraftingAttributes].ToString(), categoryStyleActive);
        else
            GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftingAttributes[intCraftingAttributes].ToString(), categoryStyle);
        GUI.EndGroup();



        //Ore choice for your item
        GUI.BeginGroup(new Rect(3 * groupWidth + screenX0, screenY0, groupWidth, groupHeight));
        if(craftingState.Equals(tCraftingState.ORE_SELECTION))
            GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftingOres[intCraftingOres].ToString(), categoryStyleActive);
        else
            GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftingOres[intCraftingOres].ToString(), categoryStyle);
        GUI.EndGroup();

        //Description Area
        GUIStyle descriptionStyle = new GUIStyle(GUI.skin.label);
        descriptionStyle.alignment = TextAnchor.MiddleCenter;
        descriptionStyle.fontStyle = FontStyle.Bold;
        descriptionStyle.fontSize = 20;

        //Details about what the current selection would craft into
        GUI.BeginGroup(new Rect(4 * groupWidth + screenX0, screenY0, groupWidth, groupHeight));
        String armorCode = ItemArmor.generateArmorCode(craftingAttributes[intCraftingAttributes], craftingOres[intCraftingOres], armors[intTypesInCategory]);
        ItemArmor madeArmor = ItemFactory.createArmor(armorCode);
        String fullDescription = "\n" +
                    madeArmor.description + "\n" +
                    "Stats" + "\n" +
                    "Damage: " + madeArmor.damage + "\n" +
                    "Armor: " + madeArmor.armor + "\n" +
                    "Attack Speed: " + madeArmor.atkspd + "\n" +
                    "\n" + "\n" +
                    "Materials(Have <-> Needed): " + "\n" +
                    madeArmor.oreType + ": " + inventory.getOreQuantity(madeArmor.oreType) + " <-> " + madeArmor.neededOreQuantity + "\n" +
                    "Crafting Points: " + " x " + madeArmor.neededPoints + "\n" +
                    "\n" + "\n";

        GUI.Label(new Rect(0, 0, groupWidth, groupHeight), fullDescription, descriptionStyle);
        GUI.EndGroup();
    }

    private enum tCraftingState
    {
        CATEGORY_SELECTION = 0,
        ITEM_TYPE = 1,
        ATTRIBUTE_SELECTION = 2,
        ORE_SELECTION = 3
    };

    private tCraftingState craftingState = tCraftingState.CATEGORY_SELECTION;
    private void handleCraftingMovement()
    {
        if (InputContextManager.isMENU_UP())
        {
            switch (craftingState)
            {
                case tCraftingState.CATEGORY_SELECTION:
                    intCraftingCategory = Math.Max(0, intCraftingCategory - 1);
                    break;
                case tCraftingState.ITEM_TYPE:
                    intTypesInCategory = Math.Max(0, intTypesInCategory - 1);
                    break;
                case tCraftingState.ATTRIBUTE_SELECTION:
                    intCraftingAttributes = Math.Max(0, intCraftingAttributes - 1);
                    break;
                case tCraftingState.ORE_SELECTION:
                    intCraftingOres = Math.Max(0, intCraftingOres - 1);
                    break;
            };

        }
        else if (InputContextManager.isMENU_DOWN())
        {
            switch (craftingState)
            {
                case tCraftingState.CATEGORY_SELECTION:
                    intCraftingCategory = Math.Min(craftableCategories.Count - 1, intCraftingCategory + 1);
                    break;
                case tCraftingState.ITEM_TYPE:
                    intTypesInCategory = Math.Min(craftingTypeInCategory.Count - 1, intTypesInCategory + 1);
                    break;
                case tCraftingState.ATTRIBUTE_SELECTION:
                    intCraftingAttributes = Math.Min(craftingAttributes.Count - 1, intCraftingAttributes + 1);
                    break;
                case tCraftingState.ORE_SELECTION:
                    intCraftingOres = Math.Min(craftingOres.Count - 1, intCraftingOres + 1);
                    break;
            };
        }
        else if (InputContextManager.isMENU_LEFT())
        {
            craftingState = (tCraftingState)Math.Max(0, (int)craftingState - 1);
        }
        else if (InputContextManager.isMENU_RIGHT())
        {
            int numStates = Enum.GetNames(typeof(tCraftingState)).Length;
            craftingState = (tCraftingState)Math.Min(numStates - 1, (int)craftingState + 1);
        }
        else if (InputContextManager.isMENU_SELECT())
        {
            String armorCode = ItemArmor.generateArmorCode(craftingAttributes[intCraftingAttributes], craftingOres[intCraftingOres], armors[intTypesInCategory]);
            ItemArmor madeArmor = ItemFactory.createArmor(armorCode);

            ItemSlot armorSlot = new ItemSlot();
            ItemOre oreRequirement = new ItemOre(madeArmor.oreType);
            oreRequirement.neededOreQuantity = madeArmor.neededOreQuantity;
            armorSlot.oreNeeded = oreRequirement;
            if (playerCanCraft(armorSlot))
            {
                inventory.inventoryRemoveItem(oreRequirement, armorSlot.oreNeeded.neededOreQuantity);
                inventory.inventoryAddItem(madeArmor);
            }
        }
    }


    private void showMenuStates()
    {
        UnityEngine.GUIStyle style = new GUIStyle(GUI.skin.button);
        Texture2D tex2dButtonPassiveBack = new Texture2D(1, 1);

        tex2dButtonPassiveBack = (Texture2D)Resources.Load("InventoryTypeBackground");
        style.normal.background = tex2dButtonPassiveBack;

        GUI.Label(new Rect(0, 0, 150, 50), arrMenusMenu[intMenusMenu].ToString(), style);
    }

    private void layoutAssembleGrid()
    {
        //int intAssembleWidthPadding = screenWidth / 8;
        //int intAssembleHeightPadding = screenHeight / 8;

        //Texture2D tex2dButtonPassiveBack = new Texture2D(1, 1);
        //Texture2D tex2dButtonActiveBack = new Texture2D(1, 1);
        //Texture2D tex2dButtonFlashBack = new Texture2D(1, 1);
        //UnityEngine.GUIStyle style = new GUIStyle(GUI.skin.button);

        ////Set the style for selection screens
        //tex2dButtonPassiveBack = (Texture2D)Resources.Load("InventoryTypeBackground");

        ////Backgrounds for non-active items
        //style.normal.background = tex2dButtonPassiveBack;
        //style.hover.background = tex2dButtonPassiveBack;
        //style.onHover.background = tex2dButtonPassiveBack;

        ////Make a label to show which kind of weapon is being assembled
        //int intAssemWeaponLabelWidth = 2 * intAssembleWidthPadding;
        //int intAssemWeaponLabelHeight = intAssembleHeightPadding;
        //int intAssemWeaponLabelX = (screenWidth / 2) - (intAssemWeaponLabelWidth / 2); //Start the label at half the screen shifted by half the label width
        //int intAssemWeaponLabelY = (screenHeight / 20);

        //GUI.Label(new Rect(intAssemWeaponLabelX, intAssemWeaponLabelY, intAssemWeaponLabelWidth, intAssemWeaponLabelHeight), arrWeaponTypes[intAssembleType], style);

        //ArrayList arrListAssemblable = getMakeableItems();
        //ArrayList temp = new ArrayList();

        ////Filter the makeable items by their type
        //for (int i = 0; i < arrListAssemblable.Count; i++)
        //{
        //    ItemWeapon wepCurrent = ItemFactory.createWeapon(((ItemComponent[])arrListAssemblable[i])[0], ((ItemComponent[])arrListAssemblable[i])[1]);

        //    if (wepCurrent.weaponType.ToString().Equals(arrWeaponTypes[intAssembleType]))
        //        temp.Add(arrListAssemblable[i]);
        //}

        ////Copy the valid items into an array
        //string[] arrAssembleStrings = new string[temp.Count];
        //arrAssembleWeapons = new ItemComponent[temp.Count][];
        //for (int i = 0; i < temp.Count; i++)
        //{
        //    arrAssembleWeapons[i] = (ItemComponent[])temp[i];
        //    arrAssembleStrings[i] = ItemFactory.createWeapon(arrAssembleWeapons[i][0], arrAssembleWeapons[i][1]).name;
        //}

        //intCompTypeGrid = GUI.SelectionGrid(new Rect(intAssembleWidthPadding, intAssemWeaponLabelY + intAssemWeaponLabelHeight + 10,
        //                                     6 * intAssembleWidthPadding, 6 * intAssembleHeightPadding),
        //                            intAssembleWeapon, arrAssembleStrings, 3, style);
        GUIStyle styleNormal = new GUIStyle(GUI.skin.label);
        Texture2D tex2Normal = new Texture2D(1, 1);
        tex2Normal = (Texture2D)Resources.Load("InventoryTypeBackground");
        styleNormal.normal.background = tex2Normal;
        styleNormal.alignment = TextAnchor.UpperCenter;

        int groupWidth = screenWidth / 3;
        int groupHeight = screenHeight;

        //Component 1 selection
        GUI.BeginGroup(new Rect(screenX0, screenY0, groupWidth, groupHeight));
        GUI.Label(new Rect(0, 0, groupWidth, groupHeight), "Component1", styleNormal);
        GUI.EndGroup();

        //Component 2 selection
        GUI.BeginGroup(new Rect(screenX0 + groupWidth, screenY0, groupWidth, groupHeight));
        GUI.Label(new Rect(0, 0, groupWidth, groupHeight), "Component2", styleNormal);
        GUI.EndGroup();

        //Description area
        GUI.BeginGroup(new Rect(screenX0 + 2 * groupWidth, screenY0, groupWidth, groupHeight));
        GUI.Label(new Rect(0, 0, groupWidth, groupHeight), "Description", styleNormal);
        GUI.EndGroup();
    }

    /// <summary>
    /// Get a list of ItemWeapons which can be made from the components in the inventory.
    /// </summary>
    /// <returns>The makeable items.</returns>
    private ArrayList getMakeableItems()
    {
        ArrayList arrListComponents = inventory.getInventoryComponents();
        ArrayList arrListResults = new ArrayList();

        //Loop through all pairs of items and see if they can be combined
        for (int i = 0; i < arrListComponents.Count; i++)
        {
            for (int j = i + 1; j < arrListComponents.Count; j++)
            {
                //Create weapon returns null when the components given are unable to form a proper weapon
                ItemWeapon potentialWeapon = ItemFactory.createWeapon((ItemComponent)arrListComponents[i], (ItemComponent)arrListComponents[j]);
                if (potentialWeapon != null)
                {
                    //TODO Use a tuple instead of an array w/ 2 items. I think the compiler won't compile against .net 4.0+
                    arrListResults.Add(new ItemComponent[2] {
												(ItemComponent)arrListComponents [i],
												(ItemComponent)arrListComponents [j]
										});
                }
            }
        }

        return arrListResults;
    }


    private Vector2 getComponentCoordinateFromIndex(int index)
    {
        //Assume that all weapon types have the same # of cols a.k.a same # of attributes
        int cols = (Enum.GetNames(typeof(ItemComponent.tAttributeType))).Length;
        int tiers = (Enum.GetNames(typeof(ItemBase.tOreType))).Length - ItemBase.getNonCraftingOres().Count - 1; //-1 to translate to 0 based indexing
        int x = index % cols;
        int y = (tiers - (int)(index / cols));

        return new Vector2(x, y);
    }

    private int getIndexFromCoordinate(int x, int y, int intNumItems, int intNumCols)
    {
        return (intNumItems - (y * intNumCols) - (intNumCols - x));
    }

    ArrayList arrInventoryItems;
    Vector2 inventoryScrollPosition = Vector2.zero;
    int inventoryItemsFullHeight = 0; //Global so that movement can move a fraction of the list up or down
    int inventorySlotHeight = 110;

    /// <summary>
    /// This method handles laying out the inventory menu 
    /// </summary>
    private void layoutInventoryGrid()
    {
        ArrayList arrListWeapons = inventory.getInventoryWeapons();
        ArrayList arrArmors = inventory.getInventoryArmors();
        ArrayList arrComponents = inventory.getInventoryComponents();
        ArrayList arrItems = inventory.getInventoryItems();
        ArrayList arrOres = inventory.getInventoryOres();
        arrInventoryItems = new ArrayList();

        foreach (ItemWeapon weapon in arrListWeapons)
        {
            arrInventoryItems.Add(weapon);
        }

        foreach (ItemEquipment armor in arrArmors)
        {
            arrInventoryItems.Add(armor);
        }

        foreach (ItemBase item in arrItems)
        {
            arrInventoryItems.Add(item);
        }

        foreach (ItemBase component in arrComponents)
        {
            arrInventoryItems.Add(component);
        }

        foreach (ItemOre ore in arrOres)
        {
            arrInventoryItems.Add(ore);
        }

        string[] arrInventoryStrings = new string[arrInventoryItems.Count];

        for (int i = 0; i < arrInventoryItems.Count; i++)
        {
            ItemBase wep = (ItemBase)arrInventoryItems[i];
            //Grab the names of the item associated with the weapon
            arrInventoryStrings[i] = wep.ToString(); //To instead use pictures/textures, make an array of pictures/textures
        }

        UnityEngine.GUIStyle style = new GUIStyle(GUI.skin.button);
        Texture2D tex2dButtonPassiveBack = new Texture2D(1, 1);
        Texture2D tex2dButtonActiveBack = new Texture2D(1, 1);
        tex2dButtonPassiveBack = (Texture2D)Resources.Load("InventoryButtonBackground");
        tex2dButtonActiveBack = (Texture2D)Resources.Load("SelectedBackground");

        //Backgrounds for non-active items
        style.normal.background = tex2dButtonPassiveBack;
        style.wordWrap = true;

        //Backgrounds when I have an active selection
        style.onNormal.background = tex2dButtonActiveBack;

        float itemLayoutWidth = screenWidth * 0.8f;
        float itemLayoutHeight = screenHeight * 0.8f;

        int itemLabelHeight = 40;

        //Area for inventory items
        GUI.BeginGroup(new Rect(screenX0, screenY0, itemLayoutWidth, itemLayoutHeight));
        GUI.Label(new Rect(0, 0, itemLayoutWidth, itemLabelHeight), "ITEMS!!!!!!!!!!!!!!!!!!!!!!!!");

        inventoryItemsFullHeight = arrInventoryItems.Count * inventorySlotHeight;

        //Each slot is a fixed height, and so higher resolutions may see more of the inventory at one time
        int inventoryItemsDisplayHeight = ((int)(itemLayoutHeight / inventorySlotHeight)) * inventorySlotHeight;

        inventoryScrollPosition = GUI.BeginScrollView(new Rect(0, 60, itemLayoutWidth, inventoryItemsDisplayHeight),
                                    inventoryScrollPosition,
                                    new Rect(0, 0, itemLayoutWidth - 50, inventoryItemsFullHeight), false, true); //Pad 50 for the scrollbar

        intInventoryItem = GUI.SelectionGrid(new Rect(0, 0, itemLayoutWidth, arrInventoryItems.Count * inventorySlotHeight),
                                    intInventoryItem, arrInventoryStrings, 1, style);

        GUI.EndScrollView();
        GUI.EndGroup();


        float playerStatsWidth = screenWidth;
        float playerStatsHeight = (screenHeight - itemLayoutHeight);

        //Area for player's stats
        GUI.BeginGroup(new Rect(screenX0, screenY0 + inventoryItemsDisplayHeight + itemLabelHeight + 30, playerStatsWidth, playerStatsHeight)); //An extra 30 pixels for padding
        String strPlayerStats = "\n" +
                    "Player's Stats:" + "\n" +
                    "Max Health: " + player.MaxHealth + "\n" +
                    "Move Speed: " + player.MoveSpeed + "\n" +
                    "Attack: " + player.AttackDamage + "\n" +
                    "Attack Speed: " + player.AttackDelay + "\n" +
                    "Armor: " + player.Armor + "\n" +
                    "\n" + "\n";

        GUI.Label(new Rect(0, 0, playerStatsWidth, playerStatsHeight), strPlayerStats, style);
        GUI.EndGroup();


        float itemStatsWidth = (screenWidth - itemLayoutWidth);
        float itemStatsHeight = itemLayoutHeight;

        //Area for the selected item's stats
        GUI.BeginGroup(new Rect(itemLayoutWidth + screenX0, screenY0, itemStatsWidth, itemStatsHeight));
        GUI.Label(new Rect(0, 0, itemStatsWidth, 100), "Selected Item Stats!!!!!!!!!!!!!!!!!!!!!", style);

        ItemEquipment itemSelected = (ItemEquipment)arrInventoryItems[intInventoryItem];
        String fullDescription = "\n" +
                    itemSelected._description + "\n" +
                     "\n" +
                    "Damage: " + itemSelected.damage + "\n" +
                    "Armor: " + itemSelected.armor + "\n" +
                    "Attack Speed: " + itemSelected.atkspd + "\n" +
                    "\n" + "\n";
        GUI.Label(new Rect(0, 100, itemStatsWidth, itemStatsHeight), fullDescription, style);

        GUI.EndGroup();



        //intInventoryItem = GUI.SelectionGrid(new Rect(vec2CurrentPos.x, vec2CurrentPos.y, screenWidth, intInvSlotHeight),
        //                                                intInventoryItem, arrInventoryStrings, intInvItemsPerRow, style);

        //GUI.BeginGroup(new Rect(screenWidth - 300, 100, 300, screenHeight));

        //ItemArmor equippedHelmet = inventory.getEquippedHelmet();
        //ItemArmor equippedChest = inventory.getEquippedChest();
        //ItemArmor equippedLegs = inventory.getEquippedLegs();

        //GUI.Label(new Rect(0, 0, 300, 200), "Head: " + (equippedHelmet == null ? "not equipped" : equippedHelmet.ToString()));
        //GUI.Label(new Rect(0, 200, 300, 200), "Chest: " + (equippedChest == null ? "not equipped" : equippedChest.ToString()));
        //GUI.Label(new Rect(0, 2 * 200, 300, 200), "Legs: " + (equippedLegs == null ? "not equipped" : equippedLegs.ToString()));

        //GUI.EndGroup();

    }

    private void handleAssembleMovement()
    {
        //Left and right change the type being assembled. (With wraparound)
        if (InputContextManager.isMENU_LEFT())
        {
            int intNewType = intAssembleType - 1;

            intAssembleType = (intNewType < 0 ? arrWeaponTypes.Length - 1 : intNewType);
        }
        else if (InputContextManager.isMENU_RIGHT())
        {
            int intNewType = intAssembleType + 1;

            intAssembleType = (intNewType > arrWeaponTypes.Length - 1 ? 0 : intNewType);
        }
        //Up and down change which item to assemble (without wraparound)
        else if (InputContextManager.isMENU_UP())
        {
            intAssembleWeapon = Math.Max(intAssembleWeapon - 1, 0);
        }
        else if (InputContextManager.isMENU_DOWN())
        {
            intAssembleWeapon = Math.Min(arrAssembleWeapons.Length - 1, intAssembleWeapon + 1);
        }
        else if (InputContextManager.isMENU_SELECT())
        {
            //Time to craft an item

            //Add the item to the player's inventory
            inventory.inventoryAddItem(ItemFactory.createWeapon(arrAssembleWeapons[intAssembleWeapon][0], arrAssembleWeapons[intAssembleWeapon][1]));

            //Remove the components from the inventory
            inventory.inventoryRemoveItem(arrAssembleWeapons[intAssembleWeapon][0]);
            inventory.inventoryRemoveItem(arrAssembleWeapons[intAssembleWeapon][1]);

            //Reset the component index
            intAssembleWeapon = 0;
        }
    }

    private void handleInventoryMovement()
    {
        int itemsEquippable = inventory.getInventoryArmors().Count + inventory.getInventoryWeapons().Count;

        if (InputContextManager.isMENU_UP())
        {
            //Decrement the index by 1 and take the maximum of 0 and newIndex
            int intNewSelected = intInventoryItem - 1;

            //Bound checking
            intInventoryItem = Mathf.Max(intNewSelected, 0);

            //If we have a new selection, move the scroll bar
            if (intInventoryItem == intNewSelected && (intInventoryItem != itemsEquippable - 2))
                inventoryScrollPosition.y -= inventoryItemsFullHeight / itemsEquippable; //Instead of using an itemSlot height, use fractional dimensions to account for padding


        }
        else if (InputContextManager.isMENU_DOWN())
        {
            //Increment the index by 1 and take the min of arrListWeapons.Count and newIndex
            int intNewSelected = intInventoryItem + 1;

            intInventoryItem = Mathf.Min(intNewSelected, itemsEquippable - 1);

            //We have a new selection
            if (intInventoryItem == intNewSelected && intInventoryItem != 1) //If we were @ 0 and now @ 1, don't move to center the shelection
                inventoryScrollPosition.y += inventoryItemsFullHeight / itemsEquippable;
        }
        else if (InputContextManager.isMENU_SELECT())
        {
            ItemBase itemToEquip = (ItemBase)arrInventoryItems[intInventoryItem];

            switch (itemToEquip.type)
            {
                case ItemBase.tItemType.Weapon:
                    {
                        inventory.inventoryEquipWeapon((ItemWeapon)itemToEquip);

                        break;
                    }
                case ItemBase.tItemType.Armor:
                    {
                        ItemArmor armorToEquip = (ItemArmor)itemToEquip;
                        switch (armorToEquip.armorPart)
                        {
                            case ItemArmor.tArmorPart.Chest:
                                inventory.inventoryEquipChest(armorToEquip);
                                break;

                            case ItemArmor.tArmorPart.Head:
                                inventory.inventoryEquipHelmet(armorToEquip);
                                break;

                            case ItemArmor.tArmorPart.Legs:
                                inventory.inventoryEquipLegs(armorToEquip);
                                break;
                        }

                        break;
                    }
            }
        }
    }

    private bool playerCanCraft(ItemSlot desired)
    {
        ArrayList arrOres = inventory.getInventoryOres();
        bool hasOres = false;
        foreach (ItemOre ore in arrOres)
        {
            if (ore.oreType.Equals(desired.oreNeeded.oreType))
            {
                if (ore.Quantity >= desired.oreNeeded.Quantity)
                {
                    ItemOre oreToRemove = new ItemOre(ore.oreType);
                    oreToRemove.Quantity = desired.oreNeeded.Quantity;
                    inventory.inventoryRemoveItem(oreToRemove, desired.oreNeeded.Quantity);

                    hasOres = true;
                    break;
                }
            }
        }

        bool hasPoints = desired.oreNeeded.neededPoints >= player.Score;

        return hasOres & hasPoints;
    }
}