using UnityEngine;
using System.Collections;

public class SquisherVertAnim : MonoBehaviour {
	float maxHeight, minHeight, diffInHeight
		, distanceDown, distanceUp;

	Transform squisherTrans;
	[Range(0.0f,1.0f)]
	public float fracSquish  = 0.0f;//this is the fraction of time squisher has been in the state
	float pauseAtTop=2.0f, pauseAtBottom=0.0f; 
	float speedRise= 15.0f, speedLower= 30.0f;


	float timeArriveAtTop, timeArriveAtBottom, timeRiseStart, timeDescendStart;
	float timeFall, timeRise, oneOverTimeFall, oneOverTimeRise;
	float yPositionGoal;//this is the goal height of squisher


	public enum StateOfSquisher
	{
		squisherUp,
		squisherFalling,
		squisherDown,
		squisherRising
	};
	public StateOfSquisher stateOfSquisher = StateOfSquisher.squisherUp;


	RaycastHit2D firstObjHit, secondObjHit;





	// Use this for initialization
	void Start () {
		squisherTrans = transform.FindChild("SquisherBody");

		//This calculates the minimum position of the squisher
		firstObjHit =Physics2D.Raycast(new Vector2(squisherTrans.position.x ,squisherTrans.position.y) ,
		                  -Vector2.up ,
		                  40.0f
		                  );



		distanceDown = Mathf.Min(distanceDown, 40.0f);

		minHeight = firstObjHit.transform.position.y +  firstObjHit.transform.localScale.y*0.5f +squisherTrans.localScale.y*0.5f;
		//minHeight    = squisherTrans.position.y - firstObjHit.distance + squisherTrans.localScale.y*0.5f;


		// This calculated the max height of the squisher
		secondObjHit = Physics2D.Raycast(new Vector2(squisherTrans.position.x ,squisherTrans.position.y),
		                  Vector2.up,
		                  40.0f  
		                  );

		distanceUp   = Mathf.Min(distanceUp, 40.0f);
		maxHeight = secondObjHit.transform.position.y -  secondObjHit.transform.localScale.y*0.5f -squisherTrans.localScale.y*0.5f;
		//maxHeight    = squisherTrans.position.y +secondObjHit.distance - squisherTrans.localScale.y*0.5f;

		timeFall = (maxHeight-minHeight)/speedLower;
		timeRise = (maxHeight-minHeight)/speedRise;

		//The object starts at the top
		squisherTrans.position = new Vector3(squisherTrans.position.x, 
		                                     maxHeight,
		                                     squisherTrans.position.z);

		timeArriveAtTop = Time.time;
		timeArriveAtBottom= 0.0f;//just so it's not null



		oneOverTimeFall = 1.0f/timeFall;
		oneOverTimeRise = 1.0f/timeRise;

		//initialise the squsher at the right height
		if (stateOfSquisher == StateOfSquisher.squisherUp) 
		{
			timeArriveAtTop = - fracSquish*pauseAtTop;
		} else if (stateOfSquisher == StateOfSquisher.squisherFalling)
		{
			timeDescendStart = - fracSquish*timeFall;
		} else if (stateOfSquisher == StateOfSquisher.squisherDown)
		{
			timeArriveAtBottom = - fracSquish*pauseAtBottom;
		} else if (stateOfSquisher == StateOfSquisher.squisherRising) {
			timeRiseStart = - fracSquish*timeRise;
		}

	}







	// Update is called once per frame
	void FixedUpdate () {
		//This should be called in fixed update.
		VerticalSquisherControl();

	
	}




	void VerticalSquisherControl(){
		//This controls the rising and falling of the squisher
		switch(stateOfSquisher){
		
		case (StateOfSquisher.squisherUp):
			//pause at the top
			if (Time.time < timeArriveAtTop + pauseAtTop){
				break;
			} else {
				timeDescendStart = Time.time;
				stateOfSquisher = StateOfSquisher.squisherFalling;
				goto case StateOfSquisher.squisherFalling;
			}

		case (StateOfSquisher.squisherFalling):
			//fall down to a given height, or until it hits something, or for a minimum length of time
			if (Time.time < timeDescendStart + timeFall) {
				//The goal position is determined by lerp
				yPositionGoal = Mathf.Lerp(maxHeight, minHeight, 
				                           (Time.time-timeDescendStart)*oneOverTimeFall);
				//Move the block down
				squisherTrans.position = new Vector3(squisherTrans.position.x,
				                                 yPositionGoal,
				                                 0.0f);
				break;

			} else {

			//	squisherTrans.rigidbody.isKinematic=false;
				timeArriveAtBottom= Time.time;
				squisherTrans.position = new Vector3(squisherTrans.position.x,
				                                     minHeight,
				                                     0.0f);

				stateOfSquisher = StateOfSquisher.squisherDown;
				goto case StateOfSquisher.squisherDown;
				//break;

			}




			

		case (StateOfSquisher.squisherDown):
			//pause at bottom
			if (Time.time < timeArriveAtBottom + pauseAtBottom){
				break;
			} else {
				timeRiseStart = Time.time;
				//squisherTrans.rigidbody.isKinematic=true;
				stateOfSquisher = StateOfSquisher.squisherRising;
				goto case StateOfSquisher.squisherRising;
			}

		case (StateOfSquisher.squisherRising):
			//rise slowly
			//squisherTrans.rigidbody.AddForce(100.0f*Vector3.up,ForceMode.Acceleration);
			//timeArriveAtTop = Time.time;
			//break;



			if (Time.time <  timeRiseStart + timeRise) {
				//The goal position is determined by lerp
				yPositionGoal = Mathf.Lerp(minHeight, maxHeight, 
				                           (Time.time-timeRiseStart)*oneOverTimeRise);
				//Move the block down
				squisherTrans.position = new Vector3(squisherTrans.position.x,
				                                 yPositionGoal,
				                                 0.0f);
				break;
				
			} else {
				
				//squisherTrans.rigidbody.isKinematic=false;
				timeArriveAtTop= Time.time;
				squisherTrans.position = new Vector3(squisherTrans.position.x,
				                                     maxHeight,
				                                     0.0f);



				stateOfSquisher = StateOfSquisher.squisherUp;
				goto case StateOfSquisher.squisherUp;
				//break;
				
			}





		}







	}






}
