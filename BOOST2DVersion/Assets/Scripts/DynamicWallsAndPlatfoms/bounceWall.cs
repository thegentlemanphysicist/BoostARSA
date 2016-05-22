using UnityEngine;
using System.Collections;


[RequireComponent(typeof(AudioSource))]
public class bounceWall : MonoBehaviour {

	//shift the transform back half a transform width
	public Transform bounceWallRenderer;
	float xWallPosision, timeOfBounce;
	float bounceShift = 1.0f;
	float restoreTime = 0.2f;
	bool restoreWall = false;
	// Use this for initialization
	void Start () {
		xWallPosision = bounceWallRenderer.position.x;
	}

	void BounceStarted(float collisionNormal)
	{
		//Play bounce sound
		GetComponent<AudioSource>().Play();

	
		//Debug.Log("collisionNormal="+collisionNormal);
		bounceShift = -Mathf.Sign(collisionNormal)*Mathf.Abs(bounceShift);
		bounceWallRenderer.position = new Vector3(bounceWallRenderer.position.x-bounceShift,bounceWallRenderer.position.y,0f);//Animate the bounce on the wall
		//have the transform shifted left or right 
		timeOfBounce = Time.time;
		restoreWall  = true;

	}


	// Update is called once per frame
	void Update () {
		if (restoreWall)
		{
			if (Time.time < timeOfBounce + restoreTime)
			{
				bounceWallRenderer.position = new Vector3(
					Mathf.Lerp(
						xWallPosision-bounceShift,
						xWallPosision, 
						(Time.time-timeOfBounce)/restoreTime
						),
					bounceWallRenderer.position.y,0f);
			} else 
			{
				bounceWallRenderer.position =  new Vector3(
					xWallPosision,bounceWallRenderer.position.y,0f);
				restoreWall = false;
			}
		}
	}
}
