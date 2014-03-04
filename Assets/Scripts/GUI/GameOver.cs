using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {
	
	public static bool gameOver = false;
	public static bool hackMenuShit = true;
	public Texture2D rogueCraftImage;
	public Texture2D gameOverTexture;
	public Rect position;
	
	// Use this for initialization
	void Start ()
	{
		gameOver = false;
		hackMenuShit = true;
		position = new Rect((Screen.width - gameOverTexture.width) / 2, (Screen.height - 
        gameOverTexture.height) /2, gameOverTexture.width, gameOverTexture.height);
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
		if(gameOver)
		{
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), gameOverTexture);
		}
	}
}
