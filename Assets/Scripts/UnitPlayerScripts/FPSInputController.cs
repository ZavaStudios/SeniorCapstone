using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Look")]
[RequireComponent(typeof(CharacterMotor))]

public class FPSInputController : MonoBehaviour
{
	private CharacterMotor motor;
	
	// Use this for initialization
	void Start ()
	{
		motor = gameObject.GetComponent<CharacterMotor>();
	}
	
	// Update is called once per frame
	void Update ()
	{


		
		float horizontal = InputContextManager.getMOVE_LR();
		float forwards = InputContextManager.getMOVE_UD();
		// Get the input vector from kayboard or analog stick
		Vector3 directionVector = new Vector3 (horizontal, 0, forwards);
		
		if (directionVector != Vector3.zero) {
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			var directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			
			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min (1, directionLength);
			
			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength = directionLength * directionLength;
			
			// Multiply the normalized direction vector by the modified length
			directionVector = directionVector * directionLength;
		}
		
		// Apply the direction to the CharacterMotorasa
		motor.inputMoveDirection = transform.rotation * directionVector;
//		motor.inputJump = OuyaExampleCommon.GetButtonDown(OuyaSDK.KeyEnum.BUTTON_O, OuyaExampleCommon.Player);
		//TODO replace KeyCode.Space to the prooper Input manager control
		motor.inputJump = Input.GetKeyDown (KeyCode.Space);
	}
}
