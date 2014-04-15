using UnityEngine;
using System.Collections;

public class testSkeleton : MonoBehaviour
{
    int cntr = 0;

    protected CharacterController control;

	// Use this for initialization
	void Start () {
        control = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //cntr++;
        //if (cntr < 100)
        //    return;
        //cntr = 0;
        //transform.Translate(Vector3.forward * 1.0f);
        control.SimpleMove(Vector3.forward * 1.0f);
	}
}
