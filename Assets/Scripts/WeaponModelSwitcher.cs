using UnityEngine;
using System.Collections;

public class WeaponModelSwitcher : MonoBehaviour 
{
    public GameObject crappy_sword;
    public GameObject crappy_pickaxe;
    public GameObject Active;
    
    public void start()
    {
        crappy_sword.SetActive(false);
        crappy_pickaxe.SetActive(false);
    }
    
    public void SwitchWeapon(string WeaponType)
    {
        if (WeaponType == "WeaponSword")
        {
            crappy_sword.SetActive(true);
            crappy_pickaxe.SetActive(false);
            Active = crappy_sword;
        }
        else if (WeaponType == "WeaponPickaxe")
        {
		    crappy_sword.SetActive(false);
			crappy_pickaxe.SetActive(true);
            Active = crappy_pickaxe;
        }
		else
		{
            crappy_sword.SetActive(false);
			crappy_pickaxe.SetActive(false);
			Active = null;
		}
        
    }
    
    public void playAnimation()
    {

        if (Active == crappy_sword)
        {
            Active.animation.Play("SwordSwing");
        }
        else if (Active == crappy_pickaxe )
        {
			print("animation!");
            Active.animation.Play("PickaxeSwing");
        }
    }
}

