using UnityEngine;
using System.Collections;

public class destroyBox: MonoBehaviour {
	///When a wall is destroyed, have the renderer flash on and off fo a 
	/// short time before the destroy command is sent.
	bool blockDead=false;
	float timeOfDestruction  = 0.0f;
	float totalFlashTime     = 0.7f, oneOverFlashTime;
	float durationOfFlash    = 0.1f;
	float timeOfLastFlash    = 0.0f;
	//MeshRenderer theWallsMesh;
	LorentzContraction lC;



	void OnHitByBomb() 
	{
		blockDead=true;
		timeOfDestruction = Time.time;
		timeOfLastFlash   = Time.time;
	}




	void OnDeath(){
		blockDead=true;
		timeOfDestruction = Time.time;
		timeOfLastFlash   = Time.time;
	}


	// Use this for initialization
	void Start () {
		//theWallsMesh= GetComponent[](MeshRenderer);
		oneOverFlashTime = 1/totalFlashTime;
	}
	
	// Update is called once per frame
	void Update () {

		if (blockDead) {
			///Let's have the block flash for a second until it disapears
			this.GetComponent<Renderer>().material.SetFloat("_DissolveVal", Mathf.Lerp(1.2f, -0.199f,
			                              (Time.time-timeOfDestruction)*oneOverFlashTime ) );
			/*
			if(Time.time > timeOfLastFlash + durationOfFlash) {
				if (transform.renderer.enabled == true){
					transform.renderer.enabled = false;
				} else {
					transform.renderer.enabled = true;
				}
				//GetComponent(MeshRenderer).enabled = false;
				timeOfLastFlash= Time.time;
			}*/

			if(Time.time > timeOfDestruction + totalFlashTime){
				//if is being held, send message to jenny that it is getting droped
				lC = transform.GetComponent ("LorentzContraction") as LorentzContraction;
				if (lC.IsBeingHeld || lC.IsBeingPickedUp)
				{
					PlatformerController.platformerController.pickUpState =PlatformerController.PickUpStates.objectDestroyed;
				}

				Destroy(gameObject);
			}


		}




	}
}
