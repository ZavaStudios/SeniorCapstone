using UnityEngine;
using System.Collections;

public class ItemTextureSwitcher : MonoBehaviour
{
    public static Material[] materials;

    //applies the appropriate material to a weapon model
    public virtual void SwitchTexture(ItemBase.tOreType type)
    {
        switch (type)
        {
            case ItemBase.tOreType.Bone:
                transform.renderer.material = materials[0];
                break;
            case ItemBase.tOreType.Copper:
                transform.renderer.material = materials[1];
                break;
            case ItemBase.tOreType.Dragon:
                transform.renderer.material = materials[2];
                break;
            case ItemBase.tOreType.Ethereal:
                transform.renderer.material = materials[3];
                break;
            case ItemBase.tOreType.Iron:
                transform.renderer.material = materials[4];
                break;
            case ItemBase.tOreType.Mithril:
                transform.renderer.material = materials[5];
                break;
            case ItemBase.tOreType.Steel:
            default:
                transform.renderer.material = materials[6];
                break;
        }
    }
}
