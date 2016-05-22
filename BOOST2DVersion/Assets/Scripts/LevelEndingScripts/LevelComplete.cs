using UnityEngine;
using System.Collections;

public class LevelComplete : MonoBehaviour {
	int levelNumber;
	PlatformerController platformerController;
	GameObject levelCanvas;
	// Use this for initialization
	void Start () {
		levelCanvas = LevelDisplay.levelDisplay.gameObject;//GameObject.Find("Level Canvas");		 
		platformerController = PlatformerController.platformerController;
			//GameObject.FindWithTag("Player").GetComponent ("PlatformerController") as PlatformerController;
	}
	
	void OnTriggerEnter2D (Collider2D  hit){
		//Debug.Log ("The tag is " + collisionHit.gameObject.tag );
		if (hit.gameObject.CompareTag( "Player") ){
			levelCanvas.SendMessage("LevelComplete",SendMessageOptions.DontRequireReceiver);
			platformerController.canControl = false;
			// the level numbers start at zero, but that's the openeing scene
			levelNumber = Application.loadedLevel ;
			//save the level data in the level file
			if (PlayerPrefs.GetInt(  "Level" + levelNumber +"Completed",0)==0){
				PlayerPrefs.SetInt(  "Level" + levelNumber +"Completed",1);
				PlayerPrefs.SetInt("LevelsCompleted", PlayerPrefs.GetInt("LevelsCompleted",0) + 1);
			}

			PlayerPrefs.Save();
			//Debug.Log(Application.loadedLevelName+"complete");
				//application -> next level
		
		}


	}







}
