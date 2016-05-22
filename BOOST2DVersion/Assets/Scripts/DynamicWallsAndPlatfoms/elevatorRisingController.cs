using UnityEngine;
using System.Collections;

public class elevatorRisingController : MonoBehaviour {
	//the elevator platform
	public Transform topMarker, bottomMarker, elevatorPlatform;
	float minHeight, maxHeight, platformSpeed, platformSeparation;
	float spawnTime, restTime, timeExtrude;
	Transform elevatorPlatformClone;




	// Use this for initialization
	void Start () 
	{
		minHeight=bottomMarker.position.y;
		maxHeight=topMarker.position.y;
		platformSpeed = GameControl.control.elevatorRiseSpeed;
		platformSeparation =60f;


		timeExtrude = GameControl.control.elevatorPauseBottom;// the time it takes the platform to form

		spawnTime = Time.time-timeExtrude;// this is a 1 time correction so the new platforms spawn correctly
		restTime  = platformSeparation/platformSpeed; //+ timeExtrude;

		int i =0;
		while ( i*platformSeparation < maxHeight-minHeight)
		{
			//instantiate platform
			elevatorPlatformClone = (Transform)Instantiate(elevatorPlatform
			            , new Vector3(transform.position.x, minHeight +i*platformSeparation, 0f)
			            , Quaternion.identity); 
			elevatorPlatformClone.parent = transform;
			//send message defining state, minheight, maxheight
			elevatorPlatformClone.GetComponent<elevatorRisingOnly>().
				InitialiseElevator(elevatorRisingOnly.StateOfElevator.elevatorRising,minHeight,maxHeight);
			
			i+=1;
		}


		//initialise 1 that is already waiting
		//elevatorPlatformClone = (Transform)Instantiate(elevatorPlatform
		//                                               , new Vector3(transform.position.x, minHeight , 5f)
		//                                              , Quaternion.identity); 
		//elevatorPlatformClone.parent = transform;
		//send message defining state, minheight, maxheight
		//elevatorPlatformClone.GetComponent<elevatorRisingOnly>().
		//	InitialiseElevator(elevatorRisingOnly.StateOfElevator.elevatorWaiting ,minHeight,maxHeight);



	}
	
	// Update is called once per frame
	void Update () 
	{

		if (Time.time> restTime + spawnTime)
		{
			elevatorPlatformClone = (Transform)Instantiate(elevatorPlatform
			                                               , new Vector3(transform.position.x, minHeight , 0f)
			                                               , Quaternion.identity); 
			elevatorPlatformClone.parent = transform;
			//send message defining state, minheight, maxheight
			elevatorPlatformClone.GetComponent<elevatorRisingOnly>().
				InitialiseElevator(elevatorRisingOnly.StateOfElevator.elevatorExtrude,minHeight,maxHeight);
			spawnTime = Time.time;
		}


	}
}
