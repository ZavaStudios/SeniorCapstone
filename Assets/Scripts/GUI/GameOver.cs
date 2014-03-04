using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {
	
	public static bool gameOver = false;
	public static bool hackMenuShit = true;
	public Texture2D rogueCraftImage;
	public Texture2D gameOverTexture;
	public Texture2D crosshairTexture;
	public Rect center;
	
	// Use this for initialization
	void Start ()
	{
		center = new Rect((Screen.width - crosshairTexture.width) / 2,
		                  (Screen.height - crosshairTexture.height) /2,
		                  crosshairTexture.width,
		                  crosshairTexture.height);

		gameOver = false;
		hackMenuShit = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.anyKeyDown)
			hackMenuShit = false;
	}
	
	void OnGUI()
	{
		if (hackMenuShit)
		{
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), rogueCraftImage);
		}
		else
			GUI.DrawTexture(center, crosshairTexture);
		if(gameOver)
		{
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), gameOverTexture);
		}
	}
}
