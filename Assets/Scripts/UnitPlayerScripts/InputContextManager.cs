//#define UNITY_ANDROID //this is only used to get monodevelop/studio to debug the ifdef UNITY_ANDROID section. When compiling for the device this will be defined automatically. AKA only uncomment this line for coding purposes.

using UnityEngine;
using System.Collections;

public class InputContextManager : MonoBehaviour {

    static public bool INVERT_CONTROLS = false;

    private static bool IN_MENU
    {
        get { return Hud.menuCode != Hud.tMenuStates.MENU_NONE; }
    }

	// Use this for initialization
	void Start () 
    {
	

	}
	
	// Update is called once per frame
	void Update () 
    {
       
	}


#if !UNITY_EDITOR && !UNITY_STANDALONE_WIN
    
    private const float buttonRepeatTimer = 0.5f;
    static private float nextMenuJoyStickLeft = 0.0f;
    static private float nextMenuJoyStickRight = 0.0f;
    static private float nextMenuJoyStickUp = 0.0f;
    static private float nextMenuJoyStickDown = 0.0f;
   
    //simulates buttonDown effect using left joystick.
    static private bool joyButtonDownLEFT()
    {
        if (OuyaExampleCommon.GetAxis(OuyaSDK.KeyEnum.AXIS_LSTICK_X, OuyaExampleCommon.Player) > 0.25 && Time.time > nextMenuJoyStickLeft)
        {
            nextMenuJoyStickLeft = Time.time + buttonRepeatTimer;
            return true;
        }
        return false;
    }

    //simulates buttonDown effect using left joystick.
    static private bool joyButtonDownRIGHT()
    {
        if (OuyaExampleCommon.GetAxis(OuyaSDK.KeyEnum.AXIS_LSTICK_X, OuyaExampleCommon.Player) < -0.25 && Time.time > nextMenuJoyStickRight)
        {
            nextMenuJoyStickRight = Time.time + buttonRepeatTimer;
            return true;
        }
        return false;
    }

    //simulates buttonDown effect using left joystick.
    static private bool joyButtonDownDOWN()
    {
        if (OuyaExampleCommon.GetAxis(OuyaSDK.KeyEnum.AXIS_LSTICK_Y, OuyaExampleCommon.Player) < -0.25 && Time.time > nextMenuJoyStickDown)
        {
            nextMenuJoyStickDown = Time.time + buttonRepeatTimer;
            return true;
        }
        return false;
    }

    //simulates buttonDown effect using left joystick.
    static private bool joyButtonDownUP()
    {
        if (OuyaExampleCommon.GetAxis(OuyaSDK.KeyEnum.AXIS_LSTICK_Y, OuyaExampleCommon.Player) < -0.25 && Time.time > nextMenuJoyStickUp)
        {
            nextMenuJoyStickUp = Time.time + buttonRepeatTimer;
            return true;
        }
        return false;
    }

    //private functions to capture menu context switching
    static public bool isITEM_MENU_PUSHED()
    { 
        //return OuyaExampleCommon.GetButtonDown(OuyaSDK.KeyEnum.BUTTON_SYSTEM, OuyaExampleCommon.Player); 
        return false;
    }

    static public bool isMAIN_MENU_PUSHED()
    { 
        return OuyaExampleCommon.GetButtonDown(OuyaSDK.KeyEnum.BUTTON_A, OuyaExampleCommon.Player); 
    }

    
    //menu controls (DPAD on OUYA)
    static public bool isMENU_LEFT()
    { 
        return IN_MENU && (OuyaExampleCommon.GetButtonDown(OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT, OuyaExampleCommon.Player) ||  joyButtonDownLEFT()); 
    }

    static public bool isMENU_RIGHT()
    { 
        return IN_MENU && (OuyaExampleCommon.GetButtonDown(OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT, OuyaExampleCommon.Player) ||  joyButtonDownRIGHT()); 
    }

    static public bool isMENU_UP()
    { 
        return IN_MENU && (OuyaExampleCommon.GetButtonDown(OuyaSDK.KeyEnum.BUTTON_DPAD_UP, OuyaExampleCommon.Player) ||  joyButtonDownUP()); 
    }

    static public bool isMENU_DOWN()
    { 
        return IN_MENU && (OuyaExampleCommon.GetButtonDown(OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN, OuyaExampleCommon.Player) ||  joyButtonDownDOWN()); 
    }

    static public bool isMENU_SELECT()
    {
        return IN_MENU && OuyaExampleCommon.GetButtonDown(OuyaSDK.KeyEnum.BUTTON_O, OuyaExampleCommon.Player); 
    }

    static public bool isMENU_SWITCH_RIGHT()
    {
        return IN_MENU && OuyaExampleCommon.GetButtonDown(OuyaSDK.KeyEnum.BUTTON_RB, OuyaExampleCommon.Player);
    }

    static public bool isMENU_SWITCH_LEFT()
    {
        return IN_MENU && OuyaExampleCommon.GetButtonDown(OuyaSDK.KeyEnum.BUTTON_LB, OuyaExampleCommon.Player);
    }
    
    //weapon controls
    static public bool isATTACK()
    { 
        return !IN_MENU && OuyaExampleCommon.GetButtonDown(OuyaSDK.KeyEnum.BUTTON_RT, OuyaExampleCommon.Player); 
    }

    static public bool isATTACK_RELEASED()
    { 
        return !IN_MENU && OuyaExampleCommon.GetButtonUp(OuyaSDK.KeyEnum.BUTTON_RT, OuyaExampleCommon.Player); 
    }

    static public bool isSPECIAL_ATTACK()
    { 
        return !IN_MENU && OuyaExampleCommon.GetButtonDown(OuyaSDK.KeyEnum.BUTTON_LT, OuyaExampleCommon.Player); 
    }

    static public bool isSPECIAL_ATTACK_RELEASED()
    { 
        return !IN_MENU && OuyaExampleCommon.GetButtonUp(OuyaSDK.KeyEnum.BUTTON_LT, OuyaExampleCommon.Player); 
    }


    static public bool isSWITCH_WEAPON()
    { 
        return !IN_MENU && OuyaExampleCommon.GetButtonDown(OuyaSDK.KeyEnum.BUTTON_Y, OuyaExampleCommon.Player); 
    }


    //movement controls
    static public float getMOVE_UD()
    {
        return IN_MENU ? 0.0f : -OuyaExampleCommon.GetAxis(OuyaSDK.KeyEnum.AXIS_LSTICK_Y, OuyaExampleCommon.Player);
    }

    static public float getMOVE_LR()
    { 
        return IN_MENU ? 0.0f : OuyaExampleCommon.GetAxis(OuyaSDK.KeyEnum.AXIS_LSTICK_X, OuyaExampleCommon.Player); 
    }

    static public float getLOOK_UD()
    {
        if (IN_MENU)
            return 0.0f;
        if (INVERT_CONTROLS)
            return OuyaExampleCommon.GetAxis(OuyaSDK.KeyEnum.AXIS_RSTICK_Y, OuyaExampleCommon.Player);
        else
            return -OuyaExampleCommon.GetAxis(OuyaSDK.KeyEnum.AXIS_RSTICK_Y, OuyaExampleCommon.Player);
    }

    static public float getLOOK_LR() 
    {
        return IN_MENU ? 0.0f : OuyaExampleCommon.GetAxis(OuyaSDK.KeyEnum.AXIS_RSTICK_X, OuyaExampleCommon.Player); 
    }

    static public bool isJUMP()
    {
        return !IN_MENU && OuyaExampleCommon.GetButtonDown(OuyaSDK.KeyEnum.BUTTON_O, OuyaExampleCommon.Player); 
    }

    static public bool isSPRINT()
    {
        return !IN_MENU && ((OuyaExampleCommon.GetButton(OuyaSDK.KeyEnum.BUTTON_RB, OuyaExampleCommon.Player) || OuyaExampleCommon.GetButton(OuyaSDK.KeyEnum.BUTTON_LB, OuyaExampleCommon.Player)); 
    }

	static public bool isACTIVATE()
	{
		return !IN_MENU && OuyaExampleCommon.GetButton(OuyaSDK.KeyEnum.BUTTON_U);
	}

#else

    //private functions to capture menu context switching
    static public bool isITEM_MENU_PUSHED()
    { 
        return Input.GetKeyUp(KeyCode.I); 
    }

    static public bool isMAIN_MENU_PUSHED()
    {
        return Input.GetKeyUp(KeyCode.Escape);
    }

    
    //menu controls (DPAD on OUYA)
    static public bool isMENU_LEFT()
    {
        return IN_MENU && (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A)); 
    }

    static public bool isMENU_RIGHT()
    {
        return IN_MENU && (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D));  
    }

    static public bool isMENU_UP()
    {
        return IN_MENU && (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W)); 
    }

    static public bool isMENU_DOWN()
    {
        return IN_MENU && (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S)); 
    }

    static public bool isMENU_SELECT()
    {
        return IN_MENU && (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter));
    }

    static public bool isMENU_SWITCH_RIGHT()
    {
        return IN_MENU && (Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.Period));
    }

    static public bool isMENU_SWITCH_LEFT()
    {
        return IN_MENU && (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.Comma));
    }
    
    //weapon controls
    static public bool isATTACK()
    {
        return !IN_MENU && Input.GetKeyDown(KeyCode.Mouse0);
    }

    static public bool isATTACK_RELEASED()
    {
        return !IN_MENU && Input.GetKeyUp(KeyCode.Mouse0);
    }

    static public bool isSPECIAL_ATTACK()
    {
        return !IN_MENU && Input.GetKeyDown(KeyCode.Mouse1);
    }

    static public bool isSPECIAL_ATTACK_RELEASED()
    {
        return !IN_MENU && Input.GetKeyUp(KeyCode.Mouse1);
    }

    static public bool isSWITCH_WEAPON()
    {
        return !IN_MENU && Input.GetKeyDown(KeyCode.Q);
    }

    //movement controls
    static public float getMOVE_UD()
    {
        return IN_MENU ? 0.0f : Input.GetAxis("Vertical");
    }

    static public float getMOVE_LR()
    {
        return IN_MENU ? 0.0f : Input.GetAxis("Horizontal");
    }

    static public float getLOOK_UD()
    {
        if (IN_MENU)
            return 0.0f;
        if (INVERT_CONTROLS)
            return -Input.GetAxis("Mouse Y");
        else
            return Input.GetAxis("Mouse Y");
    }

    static public float getLOOK_LR() 
    {
        return IN_MENU ? 0.0f : Input.GetAxis("Mouse X"); 
    }

    static public bool isJUMP()
    {
        return !IN_MENU && Input.GetKeyDown(KeyCode.Space);
    }

    static public bool isSPRINT()
    {
        return !IN_MENU && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)); 
    }

	static public bool isACTIVATE()
	{
        return !IN_MENU && Input.GetKey(KeyCode.E);
	}

#endif



}
