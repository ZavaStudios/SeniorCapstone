using UnityEngine;
using System.Collections;

public class UnitPlayer : Unit {
	
	void Start () 
	{
		setMaxSpeed();
		base.Start();
	}
	
	void Update () 
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
	}
}
