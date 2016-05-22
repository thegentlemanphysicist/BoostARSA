using UnityEngine;
using System.Collections;

public class TimeDialationOfCarried : MonoBehaviour {
	
	//This time dialation script is used to keep track 
	//of the objects internal clock
	bool clockStarted, clockCounting;//, boom
	
	public float bodyTime, fuseLength;
	
	float gammaX;
	
	// Use this for initialization
	void Start () {
		clockStarted	= false;
		clockCounting	= false;
		//boom 			= false;
		bodyTime =0.0f;
		
		//This is the fuse length of the bomb.
		fuseLength = 8.0f;
		
		gammaX = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
		//This initiates the timer
		if (clockStarted) {
			bodyTime = 0.0f;
			clockStarted  = false;
			clockCounting = true;
		}
		//Get gammaX from other scripts
		
		
		
		//This increments the timer
		if (clockCounting) {
			bodyTime += Time.deltaTime*gammaX; 
		}
		//Send boom command
		/*if ( bodyTime > fuseLength ) {
			boom = true;
		}*/
		
		
		
	}
}
