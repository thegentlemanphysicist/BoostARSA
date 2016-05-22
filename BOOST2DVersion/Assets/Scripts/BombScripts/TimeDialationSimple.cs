using UnityEngine;
using System.Collections;

public class TimeDialationSimple : MonoBehaviour {
	
	//This time dialation script is used to keep track 
	//of the objects internal clock
	
	
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
/// <summary>
/// This script needs: Lorentz contraction, Explosion of Bomb, PieTimer.
///   It will deal with the internal clock of short objects like 
///   bombs, maybe some of the badguys
/// </summary>
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////	

	
	
	bool clockStarted, clockCounting;//boom;
	
	public float bodyTime, fuseLength;
	
	public float gammaX;
	
	float percentTimeLeft;
		
		
	
	float[] mData= new float[2];
	
	LorentzContraction lorentzContraction;
	
	ExplosionOfBomb explosion;
	
	GameObject timer;
	MeshRenderer timerMesh;
	//PieChartMesh mPieChart;
	Transform bombParent;
	

	PieTimer pieTimer;
	//public Material[] countDownColour;

	float audioPitch;


	
	// Use this for initialization
	void Start () {
		lorentzContraction = transform.GetComponent ("LorentzContraction") 
						as LorentzContraction;
		
		explosion = transform.GetComponent("ExplosionOfBomb")
						as ExplosionOfBomb;
		//We need to use a common parent so the clock won't be lorentz contracted
		bombParent = transform.parent;
		timer = bombParent.FindChild("Timer").gameObject;//	
		audioPitch = timer.GetComponent<AudioSource>().pitch;
		
		
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
		if (lorentzContraction.IsBeingHeld && !clockCounting){
			clockStarted = true;
		}
		//This initiates the timer
		if (clockStarted) {
			clockStarted  = false;
			clockCounting = true;
			//timer = gameObject.AddComponent	
			
			
			//pieTimer = timer.AddComponent("PieTimer") as PieTimer;
			pieTimer = timer.GetComponent("PieTimer") as PieTimer;
			timerMesh = timer.GetComponent("MeshRenderer") as MeshRenderer;
			timerMesh.enabled =true;
			timer.GetComponent<AudioSource>().Play();


			if (pieTimer != null){
        	    percentTimeLeft=1.0f;
				pieTimer.Init(mData, 100, 0.0f, 100.0f, null);
				mData[0] = 100.0f;
				mData[1] = 0.0f;
				pieTimer.Draw(mData);
        	}
		}		
		
		
		//This increments the timer
		if (clockCounting) {
			gammaX    = lorentzContraction.gammaX;
			timer.GetComponent<AudioSource>().pitch = audioPitch/gammaX;
			bodyTime += Time.deltaTime/gammaX; 
			
			//This draws and controls the timer.
			percentTimeLeft=(fuseLength-bodyTime)/fuseLength;
			mData[0] = percentTimeLeft*100.0f;
			mData[1] = 100.0f-mData[0];
			//pieTimer.Init(mData, 100, 0.0f, 100.0f, null);
           	pieTimer.Draw(mData);
			
			//Send boom command
			if ( bodyTime > fuseLength ) {
				Destroy(timer);
			//	boom = true;
				//Debug.Log ("Boom");
				explosion.explosionState = ExplosionOfBomb.ExplosionStates.explosionInitiated;
				//explosion.explosionInitiated=true;
				clockCounting = false;
				//Now I should send the destruction command
			}
		}
	}




}
