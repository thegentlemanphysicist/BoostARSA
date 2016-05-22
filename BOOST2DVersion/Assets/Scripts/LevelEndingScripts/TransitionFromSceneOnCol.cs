using UnityEngine;
using System.Collections;
using Vectrosity;

public class TransitionFromSceneOnCol : MonoBehaviour {
	//When Jenny enters this region a new scene will be loaded.
	//public bool SceneIsCompleted;

	public string NextLevel;
	SceneFadeInOut sceneFadeInOut;


	PlatformerController platformerController;
	public GameObject levelCamera;
	// Use this for initialization
	void Start () {
		levelCamera = CameraTracking.cameraTracking.gameObject; 
		platformerController = PlatformerController.platformerController;
		//The scene fader is needed to call the end scene function;
		sceneFadeInOut =SceneFadeInOut.sceneFadeInOut;
	}



	void OnTriggerEnter2D (Collider2D  hit){
		//Debug.Log ("The tag is " + collisionHit.gameObject.tag );
		if (hit.gameObject.CompareTag( "Player") ){
			platformerController.canControl = false;
			
			//save the level data in the level file
	//		if (SceneIsCompleted){
			PlayerPrefs.SetInt(Application.loadedLevelName+"Completed",1);
			PlayerPrefs.Save();

			//Debug.Log(Application.loadedLevelName+"Completed");
	//		}

			//destroy the vectrosity line
			VectorLine.Destroy(ref JennyShoot.jennyShoot.gunLineVec);
			if (NextLevel != null){

				sceneFadeInOut.nextSceneName=NextLevel;
				sceneFadeInOut.sceneEnding  = true;
			}


		}
	}


}
