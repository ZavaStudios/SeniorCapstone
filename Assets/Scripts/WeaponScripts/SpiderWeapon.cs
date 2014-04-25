using UnityEngine;
using System.Collections;

public class SpiderWeapon : ZombieWeapon
{

	// Use this for initialization
	new void Start () 
	{
		base.Start();
		attackRange = 2;

        //Add the audio source for when this unit attacks
        attackSound = gameObject.AddComponent<AudioSource>();
        attackSound.clip = (AudioClip)Resources.Load("Sounds/Spider");
	}
	
	// Update is called once per frame
	new void Update ()
	{
		base.Update();
	}
}
