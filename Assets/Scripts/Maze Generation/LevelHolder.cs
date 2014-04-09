using UnityEngine;
using System.Collections;

public class LevelHolder : MonoBehaviour
{
	public static int Level = 1;

	void Start()
	{
		DontDestroyOnLoad(this);
	}
}
