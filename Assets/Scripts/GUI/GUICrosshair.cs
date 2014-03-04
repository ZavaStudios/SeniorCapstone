using UnityEngine;
using System.Collections;

public class GUICrosshair : MonoBehaviour {
	
	
	public Texture2D crosshairTexture;
	public Rect position;
	// Use this for initialization
	void Start () 
	{
		position = new Rect((Screen.width - crosshairTexture.width) / 2, (Screen.height - 
        crosshairTexture.height) /2, crosshairTexture.width, crosshairTexture.height);

	}
	
	void  OnGUI()
	{
		GUI.DrawTexture(position, crosshairTexture);
	}
}
