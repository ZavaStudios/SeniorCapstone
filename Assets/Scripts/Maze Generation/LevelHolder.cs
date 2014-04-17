using UnityEngine;
using System.Collections;

/// <summary>
/// Placeholder class intended to be attached to an empty prefab that doesn't
/// get destroyed on reloading a scene. This doesn't really do much - holds a
/// value which tracks which level we're on, and establishes that this object
/// is, in fact, not going to be destroyed on a scene load.
/// 
/// Short version: this class should hold any state taht needs to be persistent
/// across level changes.
/// </summary>
public class LevelHolder : MonoBehaviour
{
    // Which level we're on. Tracking this so that other scripts can determine
    // the level value, and make difficulty changes accordingly.
	public static int Level = 1;

	void Start()
	{
		DontDestroyOnLoad(this);
	}
}
