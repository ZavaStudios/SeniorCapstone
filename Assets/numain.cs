using UnityEngine;
using System.Collections;

public class numain : MonoBehaviour {

    public Transform player;

	// Use this for initialization
	void Start () {

        DoorScript.player = player;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
