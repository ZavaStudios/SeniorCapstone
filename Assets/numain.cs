using UnityEngine;
using System.Collections;

public class numain : MonoBehaviour {

    public Transform player;

	// Use this for initialization
	void Start ()
    {
        var a = new GameObject("Empty prefab yay!");
        var b = new GameObject("Foo");
        b.transform.parent = a.transform;
        DoorScript.player = player;
	}
	
	// Update is called once per frame
	void Update ()
    {

	}
}
