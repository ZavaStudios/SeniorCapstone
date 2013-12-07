using UnityEngine;
using System.Collections;

public class UnitPlayer : Unit {
	
	//Not sure how time is measured, but 30 seems to be good. 
	private float delay = 0f;
	
	protected override void Start () 
	{
		setMaxSpeed();
        
        //gameObject.AddComponent(typeof(WeaponSword));
        
		base.Start();
     
        //equipWeapon("WeaponSword");
        
        //weapon = gameObject.GetComponent<WeaponBase>();
	}
	
	protected override void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Mouse0)) //or some other button on OUYA
		{
			print ("mouse clicked....");
			if (weapon != null)
				weapon.attack = true;
			else
				print ("You cannot attack without a weapon!");
		}
		
		if(Input.GetKeyDown (KeyCode.LeftShift) || Input.GetKeyDown (KeyCode.RightShift))
		{
			moveSpeed = 25.0f;
			setMaxSpeed ();
		}
		else if(Input.GetKeyUp (KeyCode.LeftShift) || Input.GetKeyUp (KeyCode.RightShift))
		{
			moveSpeed = 10.0f;
			setMaxSpeed ();
		}
	
	}
	
	
	//Gets reference to the character motor class, then sets the move speed
	void setMaxSpeed()
	{
		CharacterMotor m = gameObject.GetComponent<CharacterMotor>();
		m.movement.maxForwardSpeed = moveSpeed;
		m.movement.maxSidewaysSpeed = moveSpeed;
		m.movement.maxBackwardsSpeed = moveSpeed;
	}
	
	protected override void killUnit ()
	{
		print ("How did you die...???");
		
		//Draws the gameover GUI to the screen. 
		GameOver.gameOver = true;
		
		//Set the length of time to display the game over tag.
		if(delay == 0)
		{
			delay = Time.time + 5;
		}
		
		//Restart the application when the game is over.
		if(Time.time >= delay)
		{
			Application.LoadLevel(0);
		}
	}
}
