using UnityEngine;
using System.Collections;

public class DoorTimeTrigger : MonoBehaviour {







	public float timeDoorOpen =5.0f;

	public Color theDoorsColor;
	//The expanding circle will be spawned each time the switch is thrown
	public Transform expandingRing;
	public Transform expandingRingRed;
	Transform expandingRingClone;


	public Transform doorTransform, trigBase, trigPlate, colourOnDoorTrans, timerNumberTrans;
	///Vector3 doorPositionInitial, doorPositionCurent;
	//float doorHeight, finalDoorHeight;

	bool doorOpening, doorOpen, doorClosing, doorClosed;//, doorClosing;
	bool openMessageSent, closeMessageSent;
	bool sendingOpenMessage, sendingCloseMessage;
	float timeOfDoor, timeOfDoorClose;
	float doorClosePos, doorOpenPos, doorPosition;
	float doorXPos;
	float oneoverDoorOpeningTime;
	float speedOfDoor;

	//The physics constants give us the speed of light in one place
	float SpeedOfLight;



	float timeOpenMessageSent, timeCloseMessageSent, timeMessageTravel;

	float  trigPlateDownPos;
	float distPlateAndDoor;


	//The timer display messages
	TextMesh countDownTimer;

	float timerStartTimer;
	int timeDisplayed;
	bool countDownStarted;

	// Use this for initialization
	void Start () {

		//trigBase = transform.FindChild("TriggerBase").GetComponent<Transform>();

		//trigPlate = trigBase.FindChild("TriggerPlate").GetComponent<Transform>();

		trigPlate.position = new Vector3(trigBase.position.x,
		                                 trigBase.position.y + 2.5f,
		                                 0.0f);
		trigPlateDownPos = trigBase.position.y + 1.5f;
		openMessageSent  = false;
		closeMessageSent = false;


		//doorParent = transform.FindChild("DoorParent");
		//doorTransform 	  = doorParent.FindChild("Door").GetComponent<Transform>();
		//colourOnDoorTrans = doorParent.FindChild("Cylinder").FindChild("ColourOverDoor").GetComponent<Transform>();



		//doorTransform = transform.FindChild("Door").GetComponent<Transform>();

		//colourOnDoorTrans = transform.FindChild("Cylinder").FindChild("ColourOverDoor").GetComponent<Transform>();
		//this is the distance from the base to the glowy light on the platform
		distPlateAndDoor  = Vector2.Distance( new Vector2(trigBase.position.x, trigBase.position.y),
		                                     new Vector2(colourOnDoorTrans.position.x, colourOnDoorTrans.position.y));
		//the door light must start red 
		colourOnDoorTrans.GetComponent<Renderer>().material.color = Color.red;




		doorOpening = false;
		doorClosing = false;
		doorOpening = false;
		doorOpen   = false; 
		doorClosed = true;
		openMessageSent  = false;
		closeMessageSent = true;
		sendingOpenMessage  = false;
		sendingCloseMessage = false;


		timeOfDoor=Time.time;
		oneoverDoorOpeningTime = 2.0f;
		doorClosePos = doorTransform.localPosition.y;
		doorXPos	 = doorTransform.localPosition.x;
		doorOpenPos  = doorClosePos + doorTransform.localScale.y;
		doorPosition = doorClosePos;

		speedOfDoor = 30.0f;


		//GET PHYSICS CONSTANTS
		//physConst = GameObject.Find ("PhysicalConstants");		
		//physicsConstants = physConst.GetComponent("PhysicsConstants") as PhysicsConstants;
		SpeedOfLight=GameControl.control.SpeedOfLight;


		//timeMessageTravel = Mathf.Abs(trigBase.position.x-doorTransform.position.x)/SpeedOfLight;
		timeMessageTravel = distPlateAndDoor/SpeedOfLight;



		countDownTimer = timerNumberTrans.GetComponent<TextMesh>();
		//doorParent.FindChild("Cylinder").FindChild("TimeNumber").GetComponent<TextMesh>();
		countDownTimer.text = "0";
		countDownStarted = false;




		//assign a color to the door
		if (theDoorsColor!=null)
		{
			trigBase.GetComponent<Renderer>().material.color = theDoorsColor;
			doorTransform.GetComponent<Renderer>().material.color = theDoorsColor;
		}




	}








	// Update is called once per frame
	void Update () {


		///THIS DETERMINES IF THE TRIGGER PLATE IS PUSHED DOWN
		if (trigPlate.position.y < trigPlateDownPos)//TRIGERED
		{
			if (!openMessageSent )
			{
				openMessageSent  = true; 		//no more open messages can be sent
				closeMessageSent = false;		//a close message can be sent now
				//MAYBE HAVE THE SENDING MESSAGE BE A SINGLE FUNCTION WITH A COLOUR AS A INPUT
				sendingOpenMessage = true;		//the sending open message statement is now on
				timeOpenMessageSent = Time.time;//the timing of sent message is ready

				expandingRingClone = Instantiate(expandingRing,trigBase.position,
				                                 Quaternion.identity) as Transform;
				expandingRingClone.SendMessage("Spawn", SendMessageOptions.DontRequireReceiver);


				trigPlate.GetComponent<Renderer>().material.color = Color.green;


				//play click click
				trigPlate.GetComponent<AudioSource>().Play();
			}
		} else 										//NOT TRIGGERED 
		{
			if (!closeMessageSent )
			{
				//send message
				closeMessageSent = true; 			//no more close messages can be sent
				openMessageSent  = false;			//an open message can be sent now

				sendingCloseMessage = true;			//the sending close message statement is now on
				timeCloseMessageSent = Time.time;	//the timing of sent message is ready

				expandingRingClone = Instantiate(expandingRingRed,trigBase.position,
				                                 Quaternion.identity) as Transform;
				expandingRingClone.SendMessage("Spawn", SendMessageOptions.DontRequireReceiver);


				trigPlate.GetComponent<Renderer>().material.color = Color.red;

				//play click click
				trigPlate.GetComponent<AudioSource>().Play();
			}

		}





		//THIS SENDS THE MESSAGES FROM THE TRIGGER TO THE DOOR
		if (sendingOpenMessage) 
		{
			//move the message transform from the trigger to the door
			if (Time.time > timeOpenMessageSent + timeMessageTravel)
			{
				doorOpening        = true;
				doorClosing		   = false;
				timeOfDoor		   = Time.time;
				sendingOpenMessage = false;
				countDownStarted   = false;
				doorPosition = doorTransform.localPosition.y;
				oneoverDoorOpeningTime = speedOfDoor/(doorOpenPos -doorClosePos + 0.1f);
				//the door light must be green here
				colourOnDoorTrans.GetComponent<Renderer>().material.color = Color.green;
			
			}
			//when it gets there set opening to true
			//set openmessageset to false

		}
		if (sendingCloseMessage) 
		{

			if (Time.time > timeCloseMessageSent + timeMessageTravel)
			{
				//move the message transform from the trigger to the door
				//doorOpening		 = false;
				//doorClosing 	 = true;
				countDownStarted = true; 
				timerStartTimer  = Time.time; 
				//timeOfDoor	    = Time.time;
				sendingCloseMessage = false;

				colourOnDoorTrans.GetComponent<Renderer>().material.color = Color.red;
			}
		}



		if (doorOpening)
		{

			if( (Time.time-timeOfDoor)*oneoverDoorOpeningTime < 1f)
			{
				doorTransform.localPosition= new Vector3(
					doorXPos,
					Mathf.Lerp(doorPosition, doorOpenPos, (Time.time-timeOfDoor)*oneoverDoorOpeningTime),
				    0.0f);
			
					timeDisplayed = (int)timeDoorOpen;
					countDownTimer.text = timeDisplayed.ToString();
			
			} else {
				doorOpening = false;
				doorOpen = true;
				timerStartTimer = Time.time;



			}


		}



		if (countDownStarted && doorOpen)
		{
			//start countdown timer
			if (Time.time > timerStartTimer +1f)
			{
				timerStartTimer = Time.time;
				timeDisplayed -= 1;
				countDownTimer.text = timeDisplayed.ToString();
				//play sound here 


				if ( timeDisplayed == 0)
				{
					timeOfDoorClose	 = Time.time;
					doorPosition	 = doorTransform.localPosition.y;
					doorClosing 	 = true;
					doorOpen		 = false;
					countDownStarted = false;

				}
				
			}
		}




		if (doorClosing)
		{





			doorTransform.localPosition= new Vector3(
				doorXPos,
				Mathf.Lerp(doorPosition, doorClosePos, (Time.time-timeOfDoorClose)*oneoverDoorOpeningTime),
				0.0f);




			/*
			if (doorTransform.position.y < doorClosePos)
			{
				doorTransform.rigidbody2D.velocity = -10.0f*Vector2.up;
				//set velocity down
			} else
			{
				//set velocity 0, position
				doorTransform.position = new Vector3 (
					doorTransform.position.x,
					doorClosePos,
					0.0f);

				doorClosed = true;
			}*/
			
		}



		/*
		if (doorOpening){
			doorTransform.localPosition= new Vector3(doorXPos,
			                                         Mathf.Lerp(doorClosePos, doorOpenPos, (Time.time-timeOfDoor)*oneoverDoorOpeningTime),
			                                0.0f);
			doorOpen = true;
			//doorTransform.rigidbody2D.AddForce(20.0f*Vector3.up);
		}*/


	
	
	
	
	}







	/*void DoorOpenSwitchStatement ()
	{
		
					
		switch (doorState)
		{
		case (DoorStates.doorClosed):
			//do nothing but wait

			break;


		case (DoorStates.sendingOpenMessage):
			//calculate the time needed to communicate with the door using taxicab metric
			//if time.Time < timeTrigger hit open the door

			//send a small sprite, greed to the door that means it's open


			break;


		case DoorStates.doorOpening :
			///because I can send a message while animated I can't 
			/// keep this in the same switch statement unless I use goto a lot in the update loop
			// math lerp until door is open
			//may have to interupt a previous process 
			break;

		case DoorStates.doorOpen:
			//do nothing

			break;

		case DoorStates.sendCloseMessage:
			break;

		case DoorStates.doorClosing:
			break;

				

		}

	}*/
	
		
	
}
