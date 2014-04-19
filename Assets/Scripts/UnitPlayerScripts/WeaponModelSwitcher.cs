using UnityEngine;
using System.Collections;



/// <summary>
/// This script is responsible for keeping reference to the player weapon models,
/// enabling and disabling/texture swapping, and playing animations
/// </summary>
public class WeaponModelSwitcher : MonoBehaviour 
{
    public GameObject sword;
    public GameObject pickaxe;
	public GameObject staff;
    public GameObject wrench;
    public GameObject bow;
	public GameObject key;
    private GameObject Active;
    
    public void start()
    {
        sword.SetActive(false);
        pickaxe.SetActive(false);
		staff.SetActive(false);
        wrench.SetActive(false);
        bow.SetActive(false);
		key.SetActive(false);
    }
    
    //given a newWeapon type and an oreType, swaps the weapon model to the appropriate
    //model and texture
    public void SwitchWeapon(ItemWeapon.tWeaponType newWeapon, ItemWeapon.tOreType oreType)
    {
        sword.SetActive(false);
        pickaxe.SetActive(false);
		staff.SetActive(false);
        wrench.SetActive(false);
        bow.SetActive(false);
		key.SetActive(false);
      
        //switch model to the model used for new weapon type.
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

        //apply the texture.
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
    
    //keeps track of which weapon is currently equippped
    //and plays the appropriate attack animation.
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
            Active.animation.PlayQueued("WrenchBonk");
        }
        else if (Active == key )
        {
            Active.animation.PlayQueued("KeyUnlock");
        }
    }
}

