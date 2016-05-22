using UnityEngine;
using System.Collections;

public class elevatorRisingOnly : MonoBehaviour {
	float minHeight, maxHeight, platformSpeed;

	float pauseAtTop, pauseAfterSpawn;
	
	float timeArriveAtTop, timeSpawn, timeRiseStart, timeKilled, timeWaitingToRespawn;
	float timeFall, timeRise, oneOverTimeFall, oneOverTimeRise;
	float yPositionGoal;//this is the goal height of elevator
	//float delayBeforeRespawn, platformSeparation;





	public enum StateOfElevator
	{
		notInitialized,
		elevatorRetract,
		elevatorRising,
		elevatorExtrude//,
		//elevatorWaiting
	};
	public StateOfElevator stateOfElevator = StateOfElevator.notInitialized;



	// Use this for initialization
	void Start () 
	{
		platformSpeed = GameControl.control.elevatorRiseSpeed;
		//platformSeparation = 30f;

		timeSpawn = Time.time;
		pauseAfterSpawn = GameControl.control.elevatorPauseBottom;
		pauseAtTop		= GameControl.control.elevatorPauseTop;


		//timeKilled= Time.time;
		//timeWaitingToRespawn = platformSeparation/platformSpeed -pauseAfterSpawn;


		//delayBeforeRespawn = (platformSeparation - (maxHeight-minHeight)%platformSeparation)/
		//	platformSpeed - timeWaitingToRespawn -pauseAtTop; 

				//- pauseAfterSpawn - (platformSeparation/platformSpeed -pauseAfterSpawn);


		//This time killed is so the platforme spawned waiting starts moving at the correct time
		//timeKilled = Time.time + platformSeparation/platformSpeed -pauseAfterSpawn -delayBeforeRespawn;



	}






	// Update is called once per frame
	void FixedUpdate () {
		ElevatorControl();
	}








	void ElevatorControl()
	{
		//This controls the rising and falling of the elevator
		switch(stateOfElevator){

		case (StateOfElevator.notInitialized):
			break;


		case (StateOfElevator.elevatorRetract):
			if (Time.time < timeArriveAtTop + pauseAtTop)
			{
				break;
			} else 
			{
				Destroy(gameObject);
				break;

				//timeKilled = Time.time;
				//timeWaitingToRespawn = delayBeforeRespawn;
				//transform.position = new Vector3(transform.position.x, minHeight, +5f) ;
				//stateOfElevator = StateOfElevator.elevatorWaiting;
				//goto case StateOfElevator.elevatorWaiting;
			}
		
			
		case (StateOfElevator.elevatorExtrude):
			if (Time.time < timeSpawn + pauseAfterSpawn)
			{
				break;
			} else
			{
				stateOfElevator = StateOfElevator.elevatorRising;
				goto case StateOfElevator.elevatorRising;
			}

		case (StateOfElevator.elevatorRising):

			if ( transform.position.y < maxHeight )
			{
				transform.position += new Vector3(0f, Time.fixedDeltaTime*platformSpeed, 0f);
				break;
			} else 
			{
				timeArriveAtTop = Time.time;
				stateOfElevator= StateOfElevator.elevatorRetract;
				goto case StateOfElevator.elevatorRetract;
			}



		/*case(StateOfElevator.elevatorWaiting):
			if (Time.time < timeKilled + timeWaitingToRespawn)
			{
				break;
			} else
			{
				timeSpawn = Time.time;
				transform.position = new Vector3(transform.position.x, minHeight, 0f) ;
				stateOfElevator = StateOfElevator.elevatorExtrude;
				goto case StateOfElevator.elevatorExtrude;
			}
		*/	
		

			
		}
		
		






	}






	public void InitialiseElevator(StateOfElevator initialState, float lowHeight, float highHeight)
	{
		minHeight = lowHeight;
		maxHeight = highHeight;
		stateOfElevator = initialState;
	}







}


