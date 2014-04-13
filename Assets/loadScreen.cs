using UnityEngine;
using System.Collections;

public class loadScreen : MonoBehaviour {

    public Texture2D LoadBG;
    private bool canProceed = false;

	// Use this for initialization
    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), LoadBG);
        canProceed = true;
    }

    void Update()
    {
        if (canProceed)
            Application.LoadLevel("mainGame");
    }
}
