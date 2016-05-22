using UnityEngine;
using System.Collections;

public class elevatorScript : MonoBehaviour {
	public Transform topMarker, bottomMarker, elevatorTransform;
	float minHeight, maxHeight, platformSpeed;

	[Range(0.0f,1.0f)]
	public float fracSquish  = 0.0f;//this is the fraction of time elevator has been in the state
	float pauseAtTop=1.0f, pauseAtBottom=1.0f; 
	float speedRise, speedLower;
	
	
	float timeArriveAtTop, timeArriveAtBottom, timeRiseStart, timeDescendStart;
	float timeFall, timeRise, oneOverTimeFall, oneOverTimeRise;
	float yPositionGoal;//this is the goal height of elevator
	





	public enum StateOfElevator
	{
		elevatorUp,
		elevatorFalling,
		elevatorDown,
		elevatorRising
	};
	public StateOfElevator stateOfElevator = StateOfElevator.elevatorDown;



	// Use this for initialization
	void Start () 
	{

		minHeight=bottomMarker.position.y;
		maxHeight=topMarker.position.y;
		platformSpeed = GameControl.control.SpeedOfLight*0.5f;
		speedRise  =platformSpeed;
		speedLower =platformSpeed;

		elevatorTransform.position 
			= new Vector3( elevatorTransform.position.x, minHeight, 0f );


	
		


		timeFall = (maxHeight-minHeight)/speedLower;
		timeRise = (maxHeight-minHeight)/speedRise;
		
		//The object starts at the top
		elevatorTransform.position = new Vector3(elevatorTransform.position.x, 
		                                     maxHeight,
		                                         elevatorTransform.position.z);
		
		timeArriveAtTop = Time.time;
		timeArriveAtBottom= 0.0f;//just so it's not null
		
		
		
		oneOverTimeFall = 1.0f/timeFall;
		oneOverTimeRise = 1.0f/timeRise;
		
		//initialise the squsher at the right height
		if (stateOfElevator == StateOfElevator.elevatorUp) 
		{
			timeArriveAtTop = - fracSquish*pauseAtTop;
		} else if (stateOfElevator == StateOfElevator.elevatorFalling)
		{
			timeDescendStart = - fracSquish*timeFall;
		} else if (stateOfElevator == StateOfElevator.elevatorDown)
		{
			timeArriveAtBottom = - fracSquish*pauseAtBottom;
		} else if (stateOfElevator == StateOfElevator.elevatorRising) 
		{
			timeRiseStart = - fracSquish*timeRise;
		}





	
	}






	// Update is called once per frame
	void FixedUpdate () {
		ElevatorControl();
	}








	void ElevatorControl()
	{
		//This controls the rising and falling of the elevator
		switch(stateOfElevator){
			
		case (StateOfElevator.elevatorUp):
			//pause at the top
			if (Time.time < timeArriveAtTop + pauseAtTop){
				break;
			} else {
				timeDescendStart = Time.time;
				stateOfElevator = StateOfElevator.elevatorFalling;
				goto case StateOfElevator.elevatorFalling;
			}
			
		case (StateOfElevator.elevatorFalling):
			//fall down to a given height, or until it hits something, or for a minimum length of time
			if (Time.time < timeDescendStart + timeFall) {
				//The goal position is determined by lerp
				yPositionGoal = Mathf.Lerp(maxHeight, minHeight, 
				                           (Time.time-timeDescendStart)*oneOverTimeFall);
				//Move the block down
				elevatorTransform.position = new Vector3(elevatorTransform.position.x,
				                                     yPositionGoal,
				                                     0.0f);
				break;
				
			} else {
				
				//	elevatorTrans.rigidbody.isKinematic=false;
				timeArriveAtBottom= Time.time;
				elevatorTransform.position = new Vector3(elevatorTransform.position.x,
				                                     minHeight,
				                                     0.0f);
				
				stateOfElevator = StateOfElevator.elevatorDown;
				goto case StateOfElevator.elevatorDown;
				
			}
			
			
			
			
			
			
		case (StateOfElevator.elevatorDown):
			//pause at bottom
			if (Time.time < timeArriveAtBottom + pauseAtBottom){
				break;
			} else {
				timeRiseStart = Time.time;
				stateOfElevator = StateOfElevator.elevatorRising;
				goto case StateOfElevator.elevatorRising;
			}
			
		case (StateOfElevator.elevatorRising):

			
			
			if (Time.time <  timeRiseStart + timeRise) {
				//The goal position is determined by lerp
				yPositionGoal = Mathf.Lerp(minHeight, maxHeight, 
				                           (Time.time-timeRiseStart)*oneOverTimeRise);
				//Move the block down
				elevatorTransform.position = new Vector3(elevatorTransform.position.x,
				                                     yPositionGoal,
				                                     0.0f);
				break;
				
			} else {
				

				timeArriveAtTop= Time.time;
				elevatorTransform.position = new Vector3(elevatorTransform.position.x,
				                                     maxHeight,
				                                     0.0f);
				
				
				
				stateOfElevator = StateOfElevator.elevatorUp;
				goto case StateOfElevator.elevatorUp;
				//break;
				
			}
			
			
			
			
			
		}
		
		






	}

}


