using UnityEngine;
using System.Collections;

public class WeaponModelSwitcher : MonoBehaviour 
{
    public GameObject sword;
    public GameObject pickaxe;
	public GameObject staff;
    //public GameObject crappy_toolbox;
    public GameObject wrench;
    public GameObject bow;
	public GameObject key;
    private GameObject Active;
    
    public void start()
    {
        sword.SetActive(false);
        pickaxe.SetActive(false);
		staff.SetActive(false);
        //crappy_toolbox.SetActive(false);
        wrench.SetActive(false);
        bow.SetActive(false);
		key.SetActive(false);
    }
    
    public void SwitchWeapon(ItemWeapon.tWeaponType newWeapon)
    {
        sword.SetActive(false);
        pickaxe.SetActive(false);
		staff.SetActive(false);
        //crappy_toolbox.SetActive(false);
        wrench.SetActive(false);
        bow.SetActive(false);
		key.SetActive(false);
        
        switch(newWeapon)
        {
            case ItemWeapon.tWeaponType.WeaponBow:
                bow.SetActive(true);
                Active = bow;
            break;
            case ItemWeapon.tWeaponType.WeaponKey:
                key.SetActive(true);
			    Active = key;
            break;
            case ItemWeapon.tWeaponType.WeaponPickaxe:
                pickaxe.SetActive(true);
                Active = pickaxe;
            break;
            case ItemWeapon.tWeaponType.WeaponStaff:
			    staff.SetActive(true);
			    Active = staff;
            break;
            case ItemWeapon.tWeaponType.WeaponSword:
                sword.SetActive(true);
                Active = sword;
            break;
            case ItemWeapon.tWeaponType.WeaponToolbox:
                //crappy_toolbox.SetActive(true);
                wrench.SetActive(true);
                Active = wrench;
            break;
            default:
                Active = null;
            break;
            
        }

    }
    
    public void playAnimation()
    {
        if (!Active) return;

        if (Active == sword)
        {
            Active.animation["SwordSwing"].speed = 1.0f; //animation speed was set to negative by sword collision detection.
        }
        else if (Active == pickaxe )
        {
            Active.animation.PlayQueued("PickaxeSwing");
        }
		else if (Active == staff )
        {
            Active.animation.PlayQueued("StaffBlast");
        }
        else if (Active == bow )
        {
            //Active.animation.PlayQueued("BowstringDraw");
        }
        else if (Active == wrench )
        {
            //crappy_toolbox.animation.PlayQueued("ToolBoxSwing");
            wrench.animation.PlayQueued("WrenchBonk");
        }

        else
        {
            //print ("no animation found");
        }
    }
}

