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
    
    public void SwitchWeapon(string WeaponType)
    {
        if (WeaponType == "WeaponSword")
        {
			crappy_staff.SetActive(false);
            crappy_sword.SetActive(true);
            crappy_pickaxe.SetActive(false);
            Active = crappy_sword;
        }
        else if (WeaponType == "WeaponPickaxe")
        {
			crappy_staff.SetActive(false);
		    crappy_sword.SetActive(false);
			crappy_pickaxe.SetActive(true);
            Active = crappy_pickaxe;
        }
		else if (WeaponType == "WeaponStaff")
		{
			crappy_staff.SetActive(true);
            crappy_sword.SetActive(false);
			crappy_pickaxe.SetActive(false);
			Active = crappy_staff;
		}        
    }
    
    public void playAnimation()
    {
		print ((Active == crappy_staff) ? "its the staff" : "something else");
        if (Active == crappy_sword)
        {
            Active.animation["SwordSwing"].speed = 1.0f;
            Active.animation.Play("SwordSwing");

        }
        else if (Active == crappy_pickaxe )
        {
			print("pickaxe animation!");
            Active.animation["PickaxeSwing"].speed = 1.0f;
            Active.animation.Play("PickaxeSwing");
        }
		else if (Active == crappy_staff )
        {
			print("staff animation!");
            Active.animation["StaffBlast"].speed = 1.0f;
            Active.animation.Play("StaffBlast");
        }
    }
}

