using UnityEngine;
using System.Collections;

public class WeaponModelSwitcher : MonoBehaviour 
{
    public GameObject crappy_sword;
    public GameObject crappy_pickaxe;
	public GameObject crappy_staff;
    private GameObject Active;
    
    public void start()
    {
        crappy_sword.SetActive(false);
        crappy_pickaxe.SetActive(false);
		crappy_staff.SetActive(false);
    }
    
    public void SwitchWeapon(string newWeapon)
    {

        crappy_sword.SetActive(false);
        crappy_pickaxe.SetActive(false);
		crappy_staff.SetActive(false);
        
            if (newWeapon == ItemWeapon.tWeaponType.WeaponSword.ToString())
            {
                crappy_sword.SetActive(true);
                Active = crappy_sword;
            }
        
            else if (newWeapon == ItemWeapon.tWeaponType.WeaponPickaxe.ToString())
            {
			    crappy_pickaxe.SetActive(true);
                Active = crappy_pickaxe;
            }
            
            else if (newWeapon == ItemWeapon.tWeaponType.WeaponStaff.ToString())
		    {
			    crappy_staff.SetActive(true);
			    Active = crappy_staff;
		    }   
            
            else
            {
                print("ERROR MOTHAFUCKAAAA WHAT YOU THINKINNNN.");
                Active = null;
            }
    }
    
    public void playAnimation()
    {
        if (Active == crappy_sword)
        {
            Active.animation["SwordSwing"].speed = 1.0f; //animation speed was set to negative by sword collision detection.
            Active.animation.Play("SwordSwing");

        }
        else if (Active == crappy_pickaxe )
        {
            Active.animation.Play("PickaxeSwing");
        }
		else if (Active == crappy_staff )
        {
            Active.animation.Play("StaffBlast");
        }
    }
}

