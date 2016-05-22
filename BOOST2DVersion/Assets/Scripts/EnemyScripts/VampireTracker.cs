using UnityEngine;
using System.Collections;

public class VampireTracker : MonoBehaviour {
	Transform jennyBody;
	Rigidbody2D vampRigidbody;
	float speedOfLight, speedOfVamp;
	//float timeDelay;
	//Vector2 directionOfBat;
	//bool facingLeft=false, facingLeftOld=false;

	//this makes sure the vampire goes to the most recent possition of jenny it sees
	float timeOldMessage = 0f;
	bool facingLeftOld = false;

	// Use this for initialization
	void Start () {
		jennyBody = PlatformerPushBodies.platformerPushBodies.transform;
		speedOfLight = GameControl.control.SpeedOfLight;
		speedOfVamp = 0.5f*speedOfLight;
		//repeating function 
		InvokeRepeating("CallCoroutine", 0f,0.2f);
	
		vampRigidbody = transform.GetComponent<Rigidbody2D>();
	}


	void CallCoroutine()
	{
		StartCoroutine( DelayedTracker());
	}



	IEnumerator DelayedTracker()
	{
		//these variables need to be private
		Vector2 directionOfBat;
		bool facingLeft=false;

		//these must be local
		float timeMessageSent, timeDelay;
		Vector2 positionOfJenny;


		timeMessageSent = Time.time;
		//how long until the game object knows where jenny is
		timeDelay = Mathf.Abs( transform.position.x-jennyBody.position.x)/speedOfLight;
		//what direction will the vampire move in?
		directionOfBat = (jennyBody.position-transform.position).normalized;



		//wait for the signal to get to the vampire
		yield return new WaitForSeconds(timeDelay);

		//Tell the vampire where it should go.
		//if it is not an old message
		if (timeMessageSent>timeOldMessage)
		{
			vampRigidbody.velocity = speedOfVamp*directionOfBat;

			//make sure the vamp is facing in the right direction
			if (directionOfBat.x < 0f  )
			{
				facingLeft = true;
			}

			if (facingLeft !=facingLeftOld)
			{
				transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
				facingLeftOld = facingLeft;
			}
			timeOldMessage = timeMessageSent;
		}



		yield return null;

	}




}
