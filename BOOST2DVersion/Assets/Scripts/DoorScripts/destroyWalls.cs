using UnityEngine;
using System.Collections;

public class destroyWalls: MonoBehaviour {
	///When a wall is destroyed, have the renderer flash on and off fo a 
	/// short time before the destroy command is sent.
	bool blockDead=false;
	float timeOfDestruction  = 0.0f;
	float totalFlashTime     = 1.0f,oneOverFlashTime;
	float durationOfFlash    = 0.1f;
	float timeOfLastFlash    = 0.0f;
	//MeshRenderer theWallsMesh;




	void OnHitByBomb() 
	{
		//Debug.Log("Block Is Dead");
		blockDead=true;
		timeOfDestruction = Time.time;
		timeOfLastFlash   = Time.time;
		
	}





	// Use this for initialization
	void Start () {
		oneOverFlashTime = 1/totalFlashTime;
		//theWallsMesh= GetComponent[](MeshRenderer);
	}
	
	// Update is called once per frame
	void Update () {

		if (blockDead) {
			this.GetComponent<Renderer>().material.SetFloat("_DissolveVal", 
			           Mathf.Lerp(1.2f, -0.199f,
			           (Time.time-timeOfDestruction)*oneOverFlashTime ) );
			///Let's have the block flash for a second until it disapears
			/*if(Time.time > timeOfLastFlash + durationOfFlash) {
				if (transform.renderer.enabled == true){
					transform.renderer.enabled = false;
				} else {
					transform.renderer.enabled = true;
				}
				//GetComponent(MeshRenderer).enabled = false;
				timeOfLastFlash= Time.time;
			}*/

			if(Time.time > timeOfDestruction + totalFlashTime){
				Destroy(gameObject);
			}


		}




	}
}
