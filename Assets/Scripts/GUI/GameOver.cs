using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {
	
	public static bool gameOver = false;
	public Texture2D gameOverTexture;
	public Rect position;
	
	// Use this for initialization
	void Start ()
	{
		gameOver = false;
		position = new Rect((Screen.width - gameOverTexture.width) / 2, (Screen.height - 
        gameOverTexture.height) /2, gameOverTexture.width, gameOverTexture.height);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	void OnGUI()
	{
		if(gameOver)
		{
			GUI.DrawTexture(position, gameOverTexture);
		}
	}
}
