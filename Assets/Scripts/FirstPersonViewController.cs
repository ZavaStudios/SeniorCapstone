using UnityEngine;
using System.Collections;

public class FirstPersonViewController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		float vx = Input.GetAxis ("Horizontal");
		float vy = Input.GetAxis ("Vertical");
		
		transform.Translate(new Vector3(vx, 0, vy));
	}
}
