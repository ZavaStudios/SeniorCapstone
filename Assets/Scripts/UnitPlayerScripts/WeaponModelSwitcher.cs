using UnityEngine;
using System.Collections;

public class WeaponModelSwitcher : MonoBehaviour 
{
    public GameObject crappy_sword;
    public GameObject crappy_pickaxe;
	public GameObject crappy_staff;
    public GameObject crappy_toolbox;
    public GameObject crappy_wrench;
    public GameObject crappy_bow;
	public GameObject crappy_key;
    private GameObject Active;
    
    public void start()
    {
        crappy_sword.SetActive(false);
        crappy_pickaxe.SetActive(false);
		crappy_staff.SetActive(false);
        crappy_toolbox.SetActive(false);
        crappy_wrench.SetActive(false);
        crappy_bow.SetActive(false);
		crappy_key.SetActive(false);
    }
    
    public void SwitchWeapon(ItemWeapon.tWeaponType newWeapon)
    {
        crappy_sword.SetActive(false);
        crappy_pickaxe.SetActive(false);
		crappy_staff.SetActive(false);
        crappy_toolbox.SetActive(false);
        crappy_wrench.SetActive(false);
        crappy_bow.SetActive(false);
		crappy_key.SetActive(false);
        
        switch(newWeapon)
        {
            case ItemWeapon.tWeaponType.WeaponBow:
                crappy_bow.SetActive(true);
                Active = crappy_bow;
            break;
            case ItemWeapon.tWeaponType.WeaponKey:
                crappy_key.SetActive(true);
			    Active = crappy_key;
            break;
            case ItemWeapon.tWeaponType.WeaponPickaxe:
                crappy_pickaxe.SetActive(true);
                Active = crappy_pickaxe;
            break;
            case ItemWeapon.tWeaponType.WeaponStaff:
			    crappy_staff.SetActive(true);
			    Active = crappy_staff;
            break;
            case ItemWeapon.tWeaponType.WeaponSword:
                crappy_sword.SetActive(true);
                Active = crappy_sword;
            break;
            case ItemWeapon.tWeaponType.WeaponToolbox:
                crappy_toolbox.SetActive(true);
                crappy_wrench.SetActive(true);
                Active = crappy_toolbox;
            break;
            default:
                Active = null;
            break;
            
        }

    }
    
    public void playAnimation()
    {
        if (Active == crappy_sword)
        {
            //Active.animation["SwordSwing"].speed = 1.0f; //animation speed was set to negative by sword collision detection.
            Active.animation.PlayQueued("SwordSwing");

        }
        else if (Active == crappy_pickaxe )
        {
            Active.animation.PlayQueued("PickaxeSwing");
        }
		else if (Active == crappy_staff )
        {
            Active.animation.PlayQueued("StaffBlast");
        }
        else if (Active == crappy_bow )
        {
            Active.animation.PlayQueued("CrappyBowDraw");
        }
        else if (Active == crappy_toolbox )
        {
            crappy_toolbox.animation.PlayQueued("ToolBoxSwing");
            crappy_wrench.animation.PlayQueued("WrenchBonk");
        }

        else
        {
            //print ("no animation found");
        }
    }
}

