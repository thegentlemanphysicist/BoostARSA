using UnityEngine;
using System.Collections;

public class DoorTrigger : MonoBehaviour {
	Transform doorTransform;
	///Vector3 doorPositionInitial, doorPositionCurent;
	//float doorHeight, finalDoorHeight;

	bool doorOpening, doorOpen;//, doorClosing;

	float timeOfDoor;
	float doorClosePos, doorOpenPos;
	float doorXPos;
	float oneoverDoorOpeningTime;
	// Use this for initialization
	void Start () {


		doorTransform = transform.parent.FindChild("Door");

		//doorPositionInitial= doorTransform.position;
		//doorHeight = doorTransform.localScale.y;
		//doorOpening = false;
		//doorClosing = true;
		doorOpening = false;
		doorOpen	=false;
		timeOfDoor=Time.time;
		oneoverDoorOpeningTime = 2.0f;
		doorClosePos = doorTransform.localPosition.y;
		doorXPos	 = doorTransform.localPosition.x;
		doorOpenPos  =doorClosePos + doorTransform.localScale.y;


	}

	
	void OnTriggerEnter2D()
	{
		if (!doorOpen)
		{
			timeOfDoor  = Time.time;
			doorOpening = true;

		}
	
	}	
	
	void OnTriggerExit2D()
	{

	}




	// Update is called once per frame
	void Update () {
		if (doorOpening){
			doorTransform.localPosition= new Vector3(doorXPos,
			                                         Mathf.Lerp(doorClosePos, doorOpenPos, (Time.time-timeOfDoor)*oneoverDoorOpeningTime),
			                                0.0f);
			doorOpen = true;
			//doorTransform.rigidbody2D.AddForce(20.0f*Vector3.up);
		}


		
		
		
	}
}
