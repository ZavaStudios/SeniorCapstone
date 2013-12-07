using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour 
{
    void OnGUI () 
    {
        // Make a health bar
        Unit player = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();
        GUI.Box(new Rect(10,10,100,30), player.Health + "/" + player.MaxHealth);
    }

}
