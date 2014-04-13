using UnityEngine;
using System.Collections;

public class mainMenuGUI : MonoBehaviour
{
    public Texture2D MainBg;
    public Texture2D StartUnselected;
    public Texture2D StartSelected;
    public Texture2D ExitUnselected;
    public Texture2D ExitSelected;
    private int selected = 0;

    // WARNING:  some hacks below, in order to use the input manager, which is fairly hacky as well.
    // Basically, since the InputManager gets its context from the Hud class's menuCode, I'm just going
    // to tell the Hud we're in the main menu state for now. Since nothing in this scene directly uses
    // the hud, this shouldn't matter, and I'll reset it when I leave to make sure everything still
    // works correctly.

    void Start()
    {
        Hud.menuCode = Hud.tMenuStates.MENU_MAIN;
    }

	// Update is called once per frame
	void Update ()
    {
        if (InputContextManager.isMENU_DOWN())
            selected = (selected + 1) & 1;
        if (InputContextManager.isMENU_UP())
            selected = (selected - 1) & 1;
        if (InputContextManager.isMENU_SELECT())
        {
            Debug.Log("selected: " + selected);
            if (selected == 0)
            {
                Application.LoadLevel("loadScreen");
            }
            else
                Application.Quit();
        }
	}

    void OnGUI()
    {
        // TODO: better placement
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), MainBg);
        GUI.DrawTexture(new Rect(0, 0, 100, 100), selected == 0 ? StartSelected : StartUnselected);
        GUI.DrawTexture(new Rect(0, 0, 100, 150), selected == 0 ? ExitUnselected : ExitSelected);
    }
}
