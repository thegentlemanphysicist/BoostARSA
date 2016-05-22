using UnityEngine;
using System.Collections;

public class destroyWall: MonoBehaviour {
	///When a wall is destroyed, have the renderer flash on and off fo a 
	/// short time before the destroy command is sent.
	bool blockDead=false;
	float timeOfDestruction  = 0.0f;
	float totalFlashTime     = 1.2f;
	float durationOfFlash    = 0.1f;
	float timeOfLastFlash    = 0.0f;
	//MeshRenderer theWallsMesh;


	void OnParticleCollision( GameObject particleGameobjec) {

		if (particleGameobjec.name == "ExplosionMessage") {
			Debug.Log("Block Is Dead");
			blockDead=true;
			timeOfDestruction = Time.time;
			timeOfLastFlash   = Time.time;
			//Destroy(gameObject);
		}
	}


	// Use this for initialization
	void Start () {
		//theWallsMesh= GetComponent[](MeshRenderer);
	}
	
	// Update is called once per frame
	void Update () {

		if (blockDead) {
			///Let's have the block flash for a second until it disapears
			if(Time.time > timeOfLastFlash + durationOfFlash) {
				if (transform.GetComponent<Renderer>().enabled == true){
					transform.GetComponent<Renderer>().enabled = false;
				} else {
					transform.GetComponent<Renderer>().enabled = true;
				}
				//GetComponent(MeshRenderer).enabled = false;
				timeOfLastFlash= Time.time;
			}

			if(Time.time > timeOfDestruction + totalFlashTime){
				Destroy(gameObject);
			}


		}




	}
}
