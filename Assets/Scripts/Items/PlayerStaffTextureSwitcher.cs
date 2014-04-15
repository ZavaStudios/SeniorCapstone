using UnityEngine;
using System.Collections;

public class PlayerStaffTextureSwitcher : ItemTextureSwitcher
{
    public override void SwitchTexture(ItemBase.tOreType type)
    {
        switch (type)
        {
            case ItemBase.tOreType.Bone:
                transform.FindChild("Model").renderer.material = materials[0];
                break;
            case ItemBase.tOreType.Copper:
                transform.FindChild("Model").renderer.material = materials[1];
                break;
            case ItemBase.tOreType.Dragon:
                transform.FindChild("Model").renderer.material = materials[2];
                break;
            case ItemBase.tOreType.Ethereal:
                transform.FindChild("Model").renderer.material = materials[3];
                break;
            case ItemBase.tOreType.Iron:
                transform.FindChild("Model").renderer.material = materials[4];
                break;
            case ItemBase.tOreType.Mithril:
                transform.FindChild("Model").renderer.material = materials[5];
                break;
            case ItemBase.tOreType.Steel:
            default:
                transform.FindChild("Model").renderer.material = materials[6];
                break;
        }
    }
}
