using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Hud : MonoBehaviour
{
    /// <summary>
    /// A private class to represent an inventory slot.
    /// </summary>
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

    /// <summary>
    /// This enum defines the possible menu states that a player can be in.
    /// </summary>
    public enum tMenuStates
    {
        MENU_NONE = 0,
        INVENTORY = 1,
        CRAFTING = 2,
        ASSEMBLING = 3,
        MENU_MAIN = 4,
        GAME_OVER = 5,
    }

    //A variable to keep track of which menu we're in
    public static tMenuStates menuCode = tMenuStates.MENU_NONE;

    //Keep a reference to the player
    Unit player;

    //Keep a reference to the inventory
    Inventory inventory = Inventory.getInstance();

    //Enumerate All components first and keep all the arrays around to update later.
    ItemSlot[,] arrMakeableComps;
    string[] arrWepPartNames;
    ItemSlot[][,] arrComponentGrids; //An array that holds 2d arrays of itemcomponents. These 2d arrays are the tiered components that the player sees

    //Grid for component options
    string[] arrSelectedComponentNames;

    //Grid for possible assembled items
    ItemComponent[][] arrAssembleWeapons;


    //Options for laying out the grid
    readonly int screenWidth = (int)(Screen.width * 0.8);
    readonly int screenHeight = (int)(Screen.height * 0.8);
    //Take half of (100 - scale factor) and add that to 0 for the new start positions 
    readonly int screenX0 = (int)(Screen.width * 0.1);
    readonly int screenY0 = (int)(Screen.height * 0.1);

    int intCompTypeWidth;

    //Vector 2's to store some x and y components
    Vector2 vec2CompTypeStart;
    Vector2 vec2CompTypeDimensions;

    //Indexes for the selection  inside of a menu
    private int intInventoryItem = 0; //The index of the selected inventory item

    //Texture for the crosshair
    public Texture2D crosshairTexture;

    /// <summary>
    /// The initializer menu when this object is created.
    /// </summary>
    protected void Start()
    {
        //Start off outside of a menu
        menuCode = tMenuStates.MENU_NONE;

        //Initialize the player
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();
        
        //Initialize component data structures
        int intNumWeapons = Enum.GetNames(typeof(ItemWeapon.tWeaponType)).Length - ItemWeapon.getNonCraftingWeapons().Count;
        int intNumParts = Enum.GetNames(typeof(ItemComponent.tComponentPart)).Length;

        arrWepPartNames = new string[intNumWeapons * intNumParts];

        int intNumOres = Enum.GetNames(typeof(ItemComponent.tOreType)).Length;
        int intNumAtts = Enum.GetNames(typeof(ItemComponent.tAttributeType)).Length;

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

    }

    /// <summary>
    /// Take care of switching and moving through menus when the corresponding keys are pressed.
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

            //See which menu we're in and process movement. Handle switching between menus first
            case tMenuStates.INVENTORY:
                {
                    if (InputContextManager.isMENU_SWITCH_LEFT())
                    {
                        menuCode = tMenuStates.CRAFTING;
                        break;
                    }
                    else if (InputContextManager.isMENU_SWITCH_RIGHT())
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

    /// <summary>
    /// This method lays out GUI's based on the state the player is in.
    /// </summary>
    void OnGUI()
    {
        switch (menuCode)
        {
            case tMenuStates.MENU_MAIN: //Display context of which direction moves into which menu
                {
                    int intMenuContextWidth = screenWidth / 8;
                    int intMenuContextHeight = screenHeight / 8;
                    
                    //The crafting menu will use this style for appearances
                    GUIStyle styleCrafting = new GUIStyle(GUI.skin.label);
                    styleCrafting.normal.background = (Texture2D)Resources.Load("CraftingTest");

                    //The inventory menu will use this style for appearances
                    GUIStyle styleInventory = new GUIStyle(GUI.skin.label);
                    styleInventory.normal.background = (Texture2D)Resources.Load("InventoryTest");

                    //The assembly menu will use this style for appearances
                    GUIStyle styleAssemble = new GUIStyle(GUI.skin.label);
                    styleAssemble.normal.background = (Texture2D)Resources.Load("AssemblyTest");

                    //Start laying out the options
                    GUI.BeginGroup(new Rect((Screen.width / 2) - (intMenuContextWidth / 2), (screenHeight / 2) - (intMenuContextHeight / 2),
                                            2 * intMenuContextWidth, 3 * intMenuContextHeight));

                    GUI.Label(new Rect(0.5f * intMenuContextWidth, 0, intMenuContextWidth, intMenuContextHeight), "", styleInventory);//Top
                    GUI.Label(new Rect(0, intMenuContextHeight, intMenuContextWidth, intMenuContextHeight), "", styleCrafting);//Left
                    GUI.Label(new Rect(intMenuContextWidth, intMenuContextHeight, intMenuContextWidth, intMenuContextHeight), "", styleAssemble); //Right

                    GUI.EndGroup();

                    break;
                }

            case tMenuStates.MENU_NONE: //Display regular player info
                {
                    // Make a health bar
                    GUI.Box(new Rect(screenX0 + 10, screenY0 + 10, 100, 30), Math.Round(player.Health) + "/" + player.MaxHealth);
                    GUI.Box(new Rect(screenX0 + 10, screenY0 + 40, 200, 30), "Crafting Points: " + player.CraftingPoints);

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
    int intCraftingCategory = 0;
    int intCraftingTypesInCategory = 0;
    int intCraftingAttributes = 0;
    int intCraftingOres = 0;
    int intCraftingWeaponTypes = 0;

    //Categories we can craft
    List<ItemBase.tItemType> craftingCategories = new List<ItemBase.tItemType> { ItemBase.tItemType.Armor, ItemBase.tItemType.Component };

    //List of types of armors
    List<ItemArmor.tArmorPart> armors = new List<ItemArmor.tArmorPart> { ItemArmor.tArmorPart.Chest, ItemArmor.tArmorPart.Head, ItemArmor.tArmorPart.Legs };

    //List of types of weapon components
    List<ItemComponent.tComponentPart> components = new List<ItemComponent.tComponentPart> { ItemComponent.tComponentPart.Handle, ItemComponent.tComponentPart.Blade };

    //An untyped list that will either reference amors or components. Basically this list acts as context to whichever this list gets referenced to
    IList craftingTypeInCategory;

    //List armor attributes
    List<ItemComponent.tAttributeType> craftingAttributes = new List<ItemComponent.tAttributeType> { ItemComponent.tAttributeType.Heavy, ItemComponent.tAttributeType.Normal, ItemComponent.tAttributeType.Light };

    //List of weapon component types
    List<ItemWeapon.tWeaponType> craftingWeaponTypes = new List<ItemWeapon.tWeaponType> { ItemWeapon.tWeaponType.WeaponBow, ItemWeapon.tWeaponType.WeaponStaff,           
                                                            ItemWeapon.tWeaponType.WeaponSword, ItemWeapon.tWeaponType.WeaponToolbox};

    //List ores that armors can be made from
    List<ItemBase.tOreType> craftingOres = new List<ItemBase.tOreType>();

    /// <summary>
    /// The method to lay out the crafting menu
    /// </summary>
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

        //Split the screen into 5 groups with padding on the edges(1 groups worth)
        int labelHeight = 200;
        int groupWidth = screenWidth / 6;
        int groupHeight = screenHeight - labelHeight;

        //Category Area (non-active) style
        GUIStyle categoryStyle = new GUIStyle(GUI.skin.label);
        categoryStyle.alignment = TextAnchor.MiddleCenter;
        categoryStyle.fontStyle = FontStyle.Bold;
        categoryStyle.fontSize = 16;
        categoryStyle.normal.textColor = new Color(202 / 255f, 121 / 255f, 33 / 255f); //Divide by 255f to get a range between 0 to 1
        categoryStyle.wordWrap = true;

        //Description Area(active)
        GUIStyle categoryStyleActive = new GUIStyle(GUI.skin.label);
        categoryStyleActive.alignment = TextAnchor.MiddleCenter;
        categoryStyleActive.fontStyle = FontStyle.Bold;
        categoryStyleActive.fontSize = 22;
        categoryStyleActive.normal.textColor = new Color(202 / 255f, 121 / 255f, 33 / 255f); //Divide by 255f to get a range between 0 to 1


        //Background for active items
        categoryStyleActive.normal.background = (Texture2D)Resources.Load("CellBackground");
        categoryStyleActive.wordWrap = true;

        //Screen background
        GUI.DrawTexture(new Rect(screenX0, screenY0, screenWidth, screenHeight), (Texture2D)Resources.Load("Crafting"));


        //Info at the Bottom of the screen.
        GUI.BeginGroup(new Rect(screenX0, screenY0 + groupHeight + labelHeight / 2, screenWidth, labelHeight));
        //TODO Make labels work for OUYA Controls            
        GUI.Label(new Rect(0, 0, screenWidth, screenHeight), "Make all your selections and then confirm." + "\n" +
            "Change option group: Left,Right" + "\n" +
            "Switch Option: Up, Down" + "\n" +
            "Confirm:Enter", infoStyle);
        GUI.EndGroup();


        //Category Groups
        GUI.BeginGroup(new Rect(screenX0, screenY0, groupWidth, groupHeight));
        if (craftingState.Equals(tCraftingState.CATEGORY_SELECTION))
            GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftingCategories[intCraftingCategory].ToString(), categoryStyleActive);
        else
            GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftingCategories[intCraftingCategory].ToString(), categoryStyle);
        GUI.EndGroup();


        //Items from the selected category
        GUI.BeginGroup(new Rect(1 * groupWidth + screenX0, screenY0, groupWidth, groupHeight));
        if (craftingCategories[intCraftingCategory].Equals(ItemBase.tItemType.Armor))
            craftingTypeInCategory = armors;
        else
            craftingTypeInCategory = components;

        //Make sure when switching between categories, we readjust the max option index if we need to
        intCraftingTypesInCategory = Math.Min(intCraftingTypesInCategory, craftingTypeInCategory.Count - 1);
        if (craftingState.Equals(tCraftingState.ITEM_TYPE))
            GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftingTypeInCategory[intCraftingTypesInCategory].ToString(), categoryStyleActive);
        else
            GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftingTypeInCategory[intCraftingTypesInCategory].ToString(), categoryStyle);
        GUI.EndGroup();


        //Attribute choice for your item
        GUI.BeginGroup(new Rect(2 * groupWidth + screenX0, screenY0, groupWidth, groupHeight));
        if (craftingState.Equals(tCraftingState.ATTRIBUTE_SELECTION))
            GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftingAttributes[intCraftingAttributes].ToString(), categoryStyleActive);
        else
            GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftingAttributes[intCraftingAttributes].ToString(), categoryStyle);
        GUI.EndGroup();



        //Ore choice for your item
        GUI.BeginGroup(new Rect(3 * groupWidth + screenX0, screenY0, groupWidth, groupHeight));
        if (craftingState.Equals(tCraftingState.ORE_SELECTION))
            GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftingOres[intCraftingOres].ToString(), categoryStyleActive);
        else
            GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftingOres[intCraftingOres].ToString(), categoryStyle);
        GUI.EndGroup();

        //Type of weapon component(if making a component)
        if (craftingCategories[intCraftingCategory].Equals(ItemBase.tItemType.Component))
        {
            GUI.BeginGroup(new Rect(4 * groupWidth + screenX0, screenY0, groupWidth, groupHeight));
            if (craftingState.Equals(tCraftingState.WEAPON_TYPE))
                GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftingWeaponTypes[intCraftingWeaponTypes].ToString(), categoryStyleActive);
            else
                GUI.Label(new Rect(0, labelHeight, groupWidth, groupHeight - labelHeight), craftingWeaponTypes[intCraftingWeaponTypes].ToString(), categoryStyle);
            GUI.EndGroup();
        }

        //Description Area
        GUIStyle descriptionStyle = new GUIStyle(GUI.skin.label);
        descriptionStyle.alignment = TextAnchor.MiddleCenter;
        descriptionStyle.fontStyle = FontStyle.Bold;
        descriptionStyle.fontSize = 20;
        descriptionStyle.normal.textColor = new Color(202 / 255f, 121 / 255f, 33 / 255f); //Gold from the mockups

        //Details about what the current selection would craft into
        GUI.BeginGroup(new Rect(5 * groupWidth + screenX0, screenY0 + labelHeight / 2, groupWidth, screenHeight));

        string fullDescription = ""; //The description of what we're crafting
        if (craftingCategories[intCraftingCategory].Equals(ItemBase.tItemType.Armor))
        {
            //Make the item and display the requirements
            String armorCode = ItemArmor.generateArmorCode(craftingAttributes[intCraftingAttributes], craftingOres[intCraftingOres], armors[intCraftingTypesInCategory]);
            ItemArmor madeArmor = ItemFactory.createArmor(armorCode);
            fullDescription = "\n" +
                        madeArmor.description + "\n" +
                        "Stats" + "\n" +
                        "Damage: " + madeArmor.damage + "\n" +
                        "Armor: " + madeArmor.armor + "\n" +
                        "Attack Speed: " + madeArmor.atkspd + "\n" +
                        "\n" + "\n" +
                        "Materials(Have <-> Needed): " + "\n" +
                        madeArmor.oreType + ": " + inventory.getOreQuantity(madeArmor.oreType) + " <-> " + madeArmor.neededOreQuantity + "\n" +
                        "Crafting Points: " + player.CraftingPoints + " <-> " + madeArmor.neededPoints + "\n" +
                        "\n" + "\n";
        }
        else
        {
            //Make the item and display the requirements
            String componentCode = ItemComponent.generateComponentCode(craftingAttributes[intCraftingAttributes], craftingOres[intCraftingOres],
                                                craftingWeaponTypes[intCraftingWeaponTypes], (ItemComponent.tComponentPart)craftingTypeInCategory[intCraftingTypesInCategory]);
            ItemComponent madeComponent = ItemFactory.createComponent(componentCode);

            fullDescription = "\n" +
                        madeComponent.description + "\n\n" +
                        "Stats" + "\n\n" +
                        "Damage: " + madeComponent.damage + "\n" +
                        "Armor: " + madeComponent.armor + "\n" +
                        "Attack Speed: " + madeComponent.atkspd + "\n" +
                        "\n" + "\n" +
                        "Materials(Have <-> Needed): " + "\n" +
                        madeComponent.oreType + ": " + inventory.getOreQuantity(madeComponent.oreType) + " <-> " + madeComponent.neededOreQuantity + "\n" +
                        "Crafting Points: " + player.CraftingPoints +  "  <-> " + madeComponent.neededPoints + "\n" +
                        "\n" + "\n";
        }
        
        //Display the description which shows stats and requirements
        GUI.Label(new Rect(0, 0, groupWidth, groupHeight), fullDescription, descriptionStyle);
        GUI.EndGroup();
    }

    /// <summary>
    /// This enum lists the states you can be in while using the crafting menu
    /// </summary>
    private enum tCraftingState
    {
        CATEGORY_SELECTION = 0,
        ITEM_TYPE = 1,
        ATTRIBUTE_SELECTION = 2,
        ORE_SELECTION = 3,
        WEAPON_TYPE = 4
    };

    //Keep track of the state inside the crafting menu
    private tCraftingState craftingState = tCraftingState.CATEGORY_SELECTION; 

    /// <summary>
    /// This method handles the button presses relevant to crafting while in the crafting menu
    /// </summary>
    private void handleCraftingMovement()
    {
        if (InputContextManager.isMENU_UP())
        {
            switch (craftingState)
            {
                case tCraftingState.CATEGORY_SELECTION:
                    //Decrement the crafting index w/o going negative
                    intCraftingCategory = Math.Max(0, intCraftingCategory - 1);
                    break;
                case tCraftingState.ITEM_TYPE:
                    //Decrement the type index w/o going negative
                    intCraftingTypesInCategory = Math.Max(0, intCraftingTypesInCategory - 1);
                    break;
                case tCraftingState.ATTRIBUTE_SELECTION:
                    //Decrement the attribute index w/o going negative
                    intCraftingAttributes = Math.Max(0, intCraftingAttributes - 1);
                    break;
                case tCraftingState.ORE_SELECTION:
                    //Decrement the ore type index w/o going negative
                    intCraftingOres = Math.Max(0, intCraftingOres - 1);
                    break;
                case tCraftingState.WEAPON_TYPE:
                    //Decrement the weapon type index w/o going negative
                    intCraftingWeaponTypes = Math.Max(0, intCraftingWeaponTypes - 1);
                    break;
            };

        }
        else if (InputContextManager.isMENU_DOWN())
        {
            switch (craftingState)
            {
                case tCraftingState.CATEGORY_SELECTION:
                    //Increment the category index w/o going over the total states
                    intCraftingCategory = Math.Min(craftingCategories.Count - 1, intCraftingCategory + 1);
                    break;
                case tCraftingState.ITEM_TYPE:
                    //Increment the category types index w/o going over the total states
                    intCraftingTypesInCategory = Math.Min(craftingTypeInCategory.Count - 1, intCraftingTypesInCategory + 1);
                    break;
                case tCraftingState.ATTRIBUTE_SELECTION:
                    //Increment the attributes index w/o going over the total states
                    intCraftingAttributes = Math.Min(craftingAttributes.Count - 1, intCraftingAttributes + 1);
                    break;
                case tCraftingState.ORE_SELECTION:
                    //Increment the ore type index w/o going over the total states
                    intCraftingOres = Math.Min(craftingOres.Count - 1, intCraftingOres + 1);
                    break;
                case tCraftingState.WEAPON_TYPE:
                    //Increment the weapon type index w/o going over the total states
                    intCraftingWeaponTypes = Math.Min(craftingWeaponTypes.Count - 1, intCraftingWeaponTypes + 1);
                    break;
            };
        }
        else if (InputContextManager.isMENU_LEFT())
        {
            //Go left one state w/o going negative
            craftingState = (tCraftingState)Math.Max(0, (int)craftingState - 1);
        }
        else if (InputContextManager.isMENU_RIGHT())
        {
            //Go right one state w/o going off the end of the states
            int numStates = Enum.GetNames(typeof(tCraftingState)).Length;

            if (craftingCategories[intCraftingCategory].Equals(ItemBase.tItemType.Component))
            {
                craftingState = (tCraftingState)Math.Min(numStates - 1, (int)craftingState + 1);
            }
            else
            { //If we're not making components, we have one less group and so subtract by 2 instaed of 1
                craftingState = (tCraftingState)Math.Min(numStates - 2, (int)craftingState + 1);
            }
        }
        else if (InputContextManager.isMENU_SELECT())
        {
            //Check what we're making
            if (craftingCategories[intCraftingCategory].Equals(ItemBase.tItemType.Armor)) //We're making armor
            {
                //Create the item
                String armorCode = ItemArmor.generateArmorCode(craftingAttributes[intCraftingAttributes], craftingOres[intCraftingOres], armors[intCraftingTypesInCategory]);
                ItemArmor madeArmor = ItemFactory.createArmor(armorCode);

                //Check the requirements
                ItemSlot armorSlot = new ItemSlot();
                ItemOre oreRequirement = new ItemOre(madeArmor.oreType);
                oreRequirement.neededOreQuantity = madeArmor.neededOreQuantity;
                oreRequirement.neededPoints = madeArmor.neededPoints;
                armorSlot.oreNeeded = oreRequirement;
                
                if (playerCanCraft(armorSlot))
                {
                    //Remove the required parts and add the selected item to the inventory
                    inventory.inventoryRemoveItem(oreRequirement, armorSlot.oreNeeded.neededOreQuantity);
                    player.CraftingPoints -= armorSlot.oreNeeded.neededPoints;
                    inventory.inventoryAddItem(madeArmor);
                }
            }
            else //We're making components
            {
                //Create the item
                String componentCode = ItemComponent.generateComponentCode(craftingAttributes[intCraftingAttributes], craftingOres[intCraftingOres],
                                        craftingWeaponTypes[intCraftingWeaponTypes], (ItemComponent.tComponentPart)craftingTypeInCategory[intCraftingTypesInCategory]);
                ItemComponent madeComponent = ItemFactory.createComponent(componentCode);

                //Check the item requirements
                ItemSlot componentSlot = new ItemSlot();
                ItemOre oreRequirement = new ItemOre(madeComponent.oreType);
                oreRequirement.neededOreQuantity = madeComponent.neededOreQuantity;
                componentSlot.oreNeeded = oreRequirement;

                if (playerCanCraft(componentSlot))
                {
                    //Remove the required parts and add the selected item to the inventory
                    inventory.inventoryRemoveItem(oreRequirement, componentSlot.oreNeeded.neededOreQuantity);
                    player.CraftingPoints -= componentSlot.oreNeeded.neededPoints;
                    inventory.inventoryAddItem(madeComponent);
                }
            }
        }
    }

    //Keep track of all the blade and handle components in the inventory
    List<ItemComponent> bladeComponents = new List<ItemComponent>();
    List<ItemComponent> handleComponents = new List<ItemComponent>();

    //Keep indexes of which blade/handle is selected
    int intBladeComponents = 0;
    int intHandleComponents = 0;

    /// <summary>
    /// This enum lists the states inside of the assembling menu
    /// </summary>
    private enum tAssemState
    {
        BLADE = 0,
        HANDLE = 1
    }

    //Start off at the blade state
    tAssemState assemState = tAssemState.BLADE;

    /// <summary>
    /// This method takes care of drawing the assembly GUI
    /// </summary>
    private void layoutAssembleGrid()
    {
        //Filter and separate blades from handles
        bladeComponents = new List<ItemComponent>();
        handleComponents = new List<ItemComponent>();
        foreach(ItemComponent comp in inventory.getInventoryComponents())
        {
            if (ItemComponent.getComponentPart(comp.strComponentCode).Equals(ItemComponent.tComponentPart.Blade))
            {
                bladeComponents.Add(comp);
            }
            else //Otherwise, we have a handle
            {
                handleComponents.Add(comp);
            }
        }

        //Draw the menu background
        GUI.DrawTexture(new Rect(screenX0, screenY0, screenWidth, screenHeight), (Texture2D)Resources.Load("Assembly"));

        //A style specifying how non-selected options look
        GUIStyle styleNormal = new GUIStyle(GUI.skin.label);
        Texture2D tex2Normal = new Texture2D(1, 1);
        tex2Normal = (Texture2D)Resources.Load("Transparent");
        styleNormal.normal.background = tex2Normal;
        styleNormal.alignment = TextAnchor.MiddleCenter;
        styleNormal.fontSize = 18;
        styleNormal.wordWrap = true;

        //A style specifying how selected items look
        GUIStyle styleSelection = new GUIStyle(GUI.skin.label);
        styleSelection.normal.background = (Texture2D)Resources.Load("CellBackground");
        styleSelection.fontSize = 24;
        styleSelection.alignment = TextAnchor.MiddleCenter;
        styleSelection.wordWrap = true;
        styleSelection.normal.textColor = new Color(202 / 255f, 121 / 255f, 33 / 255f); //A golden yellow

        //Since the background has a label, we don't need to have one, so just measure the label
        //NOTE: Since the label is engrained in the texture, I won't know exactly how much space the label actually
        //      takes up, but since I'm assuming 720 resolutions, 200 seems to work
        int labelHeight = 200;

        //Split the screen into 3 separate groups: blades, handles, description
        int groupWidth = screenWidth / 3;
        int groupHeight = ((int)(screenHeight * 0.9)) - labelHeight; //Take off some for the label on the texture


        //If no item combinations are possible
        if (getMakeableItems().Count == 0)
        {
            //Display a message saying that no combinations are possible
            GUI.Label(new Rect(screenX0, screenY0, screenWidth, screenHeight),
                "No weapon combinations possible. Make sure you get a blade and a handle for the same weapon that are the same type.",
                styleNormal);

            //Exit since we can't make anything
            return;
        }
        //After here, we can assume that we have at least 1 blade and handle that can be assembled


        //Component 1 selection
        GUI.BeginGroup(new Rect(screenX0, screenY0 + labelHeight, groupWidth, groupHeight));
        if (assemState.Equals(tAssemState.BLADE))   //Make the selected blade selected
            GUI.Label(new Rect(0, 0, groupWidth, groupHeight), bladeComponents[intBladeComponents].ToString(), styleSelection);
        else   //Not selected
            GUI.Label(new Rect(0, 0, groupWidth, groupHeight), bladeComponents[intBladeComponents].ToString(), styleNormal);

        GUI.EndGroup();



        //Component 2 a.k.a. handle selection
        GUI.BeginGroup(new Rect(screenX0 + groupWidth, screenY0 + labelHeight, groupWidth, groupHeight));
        if (assemState.Equals(tAssemState.HANDLE))
            GUI.Label(new Rect(0, 0, groupWidth, groupHeight), handleComponents[intHandleComponents].ToString(), styleSelection);
        else
            GUI.Label(new Rect(0, 0, groupWidth, groupHeight), handleComponents[intHandleComponents].ToString(), styleNormal);

        GUI.EndGroup();



        //3rd area, a.k.a. description area
        GUI.BeginGroup(new Rect(screenX0 + 2 * groupWidth, screenY0 + labelHeight, groupWidth, groupHeight));
        string fullDescription = "";

        //What the selected blade and handle could form; may be null if the combination is not possible
        ItemWeapon potentialWeapon = ItemFactory.createWeapon(bladeComponents[intBladeComponents], handleComponents[intHandleComponents]);
        
        //If we can't combine, tell the user such
        if (potentialWeapon == null)
            fullDescription = "Cannot Combine";
        else
        {
            //If we can make an item, show it's stats
            fullDescription = "\n" +
                    potentialWeapon._description + "\n" +
                    "\n" +
                    "Damage: " + potentialWeapon.damage + "\n" +
                    "Armor: " + potentialWeapon.armor + "\n" +
                    "Attack Speed: " + potentialWeapon.atkspd + "\n" +
                    "\n" + "\n";
        }
        GUI.Label(new Rect(0, 0, groupWidth, groupHeight), fullDescription, styleNormal);
        GUI.EndGroup();
    }

    /// <summary>
    /// Handles the button presses inside the assembling menu while the assembling menu is open
    /// </summary>
    private void handleAssembleMovement()
    {
        //No components, no movement
        if (handleComponents.Count == 0 || bladeComponents.Count == 0)
            return;

        //Left and right change the type being assembled. (With wraparound)
        if (InputContextManager.isMENU_LEFT())
        {
            assemState = (tAssemState)Math.Max(0, (int)assemState - 1);
        }
        else if (InputContextManager.isMENU_RIGHT())
        {
            int numStates = Enum.GetNames(typeof(tAssemState)).Length - 1;

            assemState = (tAssemState)Math.Min(numStates, (int)assemState + 1);
        }
        //Up and down change which item to assemble (without wraparound)
        else if (InputContextManager.isMENU_UP())
        {
            switch (assemState)
            {
                case tAssemState.BLADE:
                    intBladeComponents = Math.Max(0, intBladeComponents - 1);
                    break;
                case tAssemState.HANDLE:
                    intHandleComponents = Math.Max(0, intHandleComponents - 1);
                    break;
            }
        }
        else if (InputContextManager.isMENU_DOWN())
        {
            switch (assemState)
            {
                case tAssemState.BLADE:
                    intBladeComponents = Math.Min(bladeComponents.Count - 1, intBladeComponents + 1);
                    break;
                case tAssemState.HANDLE:
                    intHandleComponents = Math.Min(handleComponents.Count - 1, intHandleComponents + 1);
                    break;
            }
        }
        else if (InputContextManager.isMENU_SELECT())
        {
            //Time to craft an item
            ItemWeapon madeWeapon = ItemFactory.createWeapon(bladeComponents[intBladeComponents], handleComponents[intHandleComponents]);

            //See if the two selected types are actually compatible
            if (madeWeapon == null)
                return;

            //Since we have a weapon, add it to the inventory
            inventory.inventoryAddItem(madeWeapon);

            //Remove the components from the inventory
            inventory.inventoryRemoveItem(bladeComponents[intBladeComponents]);
            inventory.inventoryRemoveItem(handleComponents[intHandleComponents]);
        }
    }


    ArrayList arrInventoryItems; //All the items in the inventory
    Vector2 inventoryScrollPosition = Vector2.zero; //Where the scroll bar of the inventory should be
    int inventoryItemsFullHeight = 0; //Global variable so that movement can move a fraction of the list up or down and be affected by button presses
    int inventorySlotHeight = 110; //The height that a slot should be

    /// <summary>
    /// This method handles drawingthe inventory GUI 
    /// </summary>
    private void layoutInventoryGrid()
    {
        //Get individual types from the inventory and add them to the displayed items list
        arrInventoryItems = new ArrayList();

        //Display weapons
        ArrayList arrListWeapons = inventory.getInventoryWeapons();
        foreach (ItemWeapon weapon in arrListWeapons)
        {
            arrInventoryItems.Add(weapon);
        }

        //Display armors
        ArrayList arrArmors = inventory.getInventoryArmors();
        foreach (ItemEquipment armor in arrArmors)
        {
            arrInventoryItems.Add(armor);
        }

        //Display components
        ArrayList arrComponents = inventory.getInventoryComponents();
        foreach (ItemBase component in arrComponents)
        {
            arrInventoryItems.Add(component);
        }

        //Display useable items
        ArrayList arrItems = inventory.getInventoryItems();
        foreach (ItemBase item in arrItems)
        {
            arrInventoryItems.Add(item);
        }

        //Display ores
        ArrayList arrOres = inventory.getInventoryOres();
        foreach (ItemOre ore in arrOres)
        {
            arrInventoryItems.Add(ore);
        }

        //Mirror the inventory, but instead of items, just keep track of strings.
        //Need an array of strings for the selection grid
        string[] arrInventoryStrings = new string[arrInventoryItems.Count];

        for (int i = 0; i < arrInventoryItems.Count; i++)
        {
            ItemBase item = (ItemBase)arrInventoryItems[i];

            //Grab the names of the item associated with the weapon
            if (item is ItemOre)
                arrInventoryStrings[i] = item.ToString() + " x " + inventory.getOreQuantity(item.oreType); //To instead use pictures/textures, make an array of pictures/textures
            else
                arrInventoryStrings[i] = item.ToString(); //To instead use pictures/textures, make an array of pictures/textures
        }

        //Style for non-active items
        UnityEngine.GUIStyle style = new GUIStyle(GUI.skin.label);
        style.wordWrap = true;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 15;
        style.onNormal.background = (Texture2D)Resources.Load("Transparent");

        //Style for when I have an active selection
        style.onNormal.textColor = new Color(202 / 255f, 121 / 255f, 33 / 255f); //Divide by 255f to get a range from 0 to 1

        float itemLayoutWidth = screenWidth * 0.8f;
        float itemLayoutHeight = screenHeight * 0.7f;

        int itemLabelHeight = 100;

        //Background image
        GUI.DrawTexture(new Rect(screenX0, screenY0, screenWidth, screenHeight), (Texture2D)Resources.Load("Inventory"));

        //Area for inventory items
        GUI.BeginGroup(new Rect(screenX0, screenY0 + itemLabelHeight, itemLayoutWidth, itemLayoutHeight));
        //GUI.Label(new Rect(0, 0, itemLayoutWidth, itemLabelHeight), "ITEMS!!!!!!!!!!!!!!!!!!!!!!!!"); //There is instead text on the texture

        //Each slot is a fixed height, and so higher resolutions may see more of the inventory at one time
        int itemsDisplayable = ((int)(itemLayoutHeight / inventorySlotHeight));
        int inventoryItemsDisplayHeight = itemsDisplayable * inventorySlotHeight;

        //Normalize so that we always have at least the displayable number of items
        int neededEmptySlots = (itemsDisplayable - arrInventoryStrings.Length);
        int previousLength = arrInventoryStrings.Length;
        if (neededEmptySlots > 0)
        {
            Array.Resize(ref arrInventoryStrings, neededEmptySlots + previousLength);
        }

        //Pad the inventory with empty slots if there aren't enough items to fit on one page of the inventory
        for (int i = 0; i < neededEmptySlots; i++)
        {
            arrInventoryStrings[previousLength + i] = "[You require more minerals (to build more equipment)]";
        }

        //Start to actually layout things
        inventoryItemsFullHeight = arrInventoryStrings.Length * inventorySlotHeight;

        inventoryScrollPosition = GUI.BeginScrollView(new Rect(0, 60, itemLayoutWidth, inventoryItemsDisplayHeight),
                                    inventoryScrollPosition,
                                    new Rect(0, 0, itemLayoutWidth - 50, inventoryItemsFullHeight), false, true); //Pad 50 for the scrollbar

        intInventoryItem = GUI.SelectionGrid(new Rect(0, 0, itemLayoutWidth, arrInventoryStrings.Length * inventorySlotHeight),
                                    intInventoryItem, arrInventoryStrings, 1, style);

        GUI.EndScrollView();
        GUI.EndGroup();

        UnityEngine.GUIStyle descriptionStyle = new GUIStyle(GUI.skin.label);
        descriptionStyle.wordWrap = true;
        descriptionStyle.alignment = TextAnchor.MiddleCenter;
        descriptionStyle.fontSize = 16;
        //descriptionStyle.normal.background = (Texture2D)Resources.Load("Transparent"); //Won't apply other onNormal settings if there's no background?

        //Style for when I have an active selection
        descriptionStyle.normal.textColor = new Color(202 / 255f, 121 / 255f, 33 / 255f); //Divide by 255f to get a range from 0 to 1 for colors

        float playerStatsWidth = screenWidth;
        float playerStatsHeight = (screenHeight - itemLayoutHeight);

        //Area for player's stats
        GUI.BeginGroup(new Rect(screenX0, screenY0 + inventoryItemsDisplayHeight + itemLabelHeight, playerStatsWidth, playerStatsHeight)); //An extra 30 pixels for padding
        String strPlayerStats = "\n" +
                    "Player's Stats:" + "\n" +
                    "Max Health: " + player.MaxHealth + "\t\t" +
                    "Move Speed: " + player.MoveSpeed + "\n" +
                    "Attack: " + player.AttackDamage + "\t\t" +
                    "Attack Speed: " + player.AttackDelay + "\t\t" +
                    "Armor: " + player.Armor + "\t";

        GUI.Label(new Rect(0, 0, playerStatsWidth, playerStatsHeight), strPlayerStats, descriptionStyle);
        GUI.EndGroup();


        float itemStatsWidth = (screenWidth - itemLayoutWidth);
        float itemStatsHeight = itemLayoutHeight;

        //Area for the selected item's stats
        GUI.BeginGroup(new Rect(itemLayoutWidth + screenX0, screenY0, itemStatsWidth, itemStatsHeight));
        //GUI.Label(new Rect(0, 0, itemStatsWidth, 100), "Selected Item Stats!!!!!!!!!!!!!!!!!!!!!", style);
        ItemBase currentItem = (ItemBase) arrInventoryItems[intInventoryItem];
        String fullDescription = "";

        //What to display for ores
        if (currentItem is ItemOre)
        {
            fullDescription = currentItem.oreType.ToString() + " to make items with.";
        }
        else
        { //What to display for everything else in the inventory
            ItemEquipment itemSelected = (ItemEquipment)arrInventoryItems[intInventoryItem];
            fullDescription = "\n" +
                        itemSelected._description + "\n" +
                         "\n" +
                        "Damage: " + itemSelected.damage + "\n" +
                        "Armor: " + itemSelected.armor + "\n" +
                        "Attack Speed: " + itemSelected.atkspd + "\n" +
                        "\n" + "\n";
        }
        
        GUI.Label(new Rect(0, 100, itemStatsWidth, itemStatsHeight), fullDescription, descriptionStyle);

        GUI.EndGroup();
    }

    /// <summary>
    /// Handles the button presses inside the assembling menu while the assembling menu is open
    /// </summary>
    private void handleInventoryMovement()
    {
        int itemsEquippable = inventory.getInventoryArmors().Count + inventory.getInventoryWeapons().Count + 
                               inventory.getInventoryComponents().Count + inventory.getInventoryOres().Count;

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
            //Get the item the player wanted to use
            ItemBase itemToEquip = (ItemBase)arrInventoryItems[intInventoryItem];

            //Check what using an item should actually do
            switch (itemToEquip.type)
            {
                //Weapons should be equipped
                case ItemBase.tItemType.Weapon:
                    {
                        inventory.inventoryEquipWeapon((ItemWeapon)itemToEquip);

                        break;
                    }
                //Armors should be equipped to their corresponding body parts
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

    /// <summary>
    /// See if the player has all the necessary materials to craft
    /// </summary>
    /// <param name="desired"></param>
    /// <returns></returns>
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
                    hasOres = true;
                    break;
                }
            }
        }

        //Check to see if the play has enough points
        bool hasPoints = desired.oreNeeded.neededPoints <= player.CraftingPoints;

        return hasOres & hasPoints;
    }
}