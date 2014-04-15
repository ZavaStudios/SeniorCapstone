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
    //private UnitPlayer PlayerCharacter;
    
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
    
    public void SwitchWeapon(ItemWeapon.tWeaponType newWeapon, ItemWeapon.tOreType oreType)
    {
        sword.SetActive(false);
        pickaxe.SetActive(false);
		staff.SetActive(false);
        //crappy_toolbox.SetActive(false);
        wrench.SetActive(false);
        bow.SetActive(false);
		key.SetActive(false);
        
        //PlayerCharacter = GameObject.FindGameObjectWithTag("Player").GetComponent<UnitPlayer>();
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
                wrench.SetActive(true);
                Active = wrench;
            break;
            default:
                Active = null;
            break;
            
        }

 
        if (newWeapon != ItemWeapon.tWeaponType.WeaponStaff)
        {
            ItemTextureSwitcher texSwitcher;
            texSwitcher = Active.GetComponent<ItemTextureSwitcher>();
            texSwitcher.SwitchTexture(oreType);
            
            if (newWeapon == ItemWeapon.tWeaponType.WeaponBow)
            {
                WeaponBowFiringScript s = bow.GetComponent<WeaponBowFiringScript>();
                PlayerStaffTextureSwitcher arrowTexSwitcher = s.arrowModel.GetComponent<PlayerStaffTextureSwitcher>();
                arrowTexSwitcher.SwitchTexture(oreType);
            }

        }
        else
        {
            PlayerStaffTextureSwitcher texSwitcher;
            texSwitcher = Active.GetComponent<PlayerStaffTextureSwitcher>();
            texSwitcher.SwitchTexture(oreType);
        }

    }
    
    public void playAnimation()
    {
        if (!Active) return;

        if (Active == sword)
        {
            Active.animation.PlayQueued("SwordSwing");
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
            Active.animation.PlayQueued("Aim");
        }
        else if (Active == wrench )
        {
            //crappy_toolbox.animation.PlayQueued("ToolBoxSwing");
            Active.animation.PlayQueued("WrenchBonk");
        }
        else if (Active == key )
        {
            Active.animation.PlayQueued("KeyUnlock");
            //print ("no animation found");
        }
    }
}

