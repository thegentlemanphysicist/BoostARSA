using UnityEngine;
using System.Collections;

public class GoToLevel : MonoBehaviour {
	public string levelToGoTo;

	public void LoadNextLevel()
	{
		if (levelToGoTo != null){
			if (levelToGoTo =="Level1")
			{
				//if the opening scene hasn't been run
				if (PlayerPrefs.GetInt("OpeningSceneCompleted")==1){
					SceneFadeInOut.sceneFadeInOut.nextSceneName="IntroToLevel1";
					SceneFadeInOut.sceneFadeInOut.sceneEnding  = true;

					//I need to be careful how I define a level as complete
					//Application.LoadLevel( "IntroToLevel1" );
					
				}else {
					SceneFadeInOut.sceneFadeInOut.nextSceneName="OpeningScene";
					SceneFadeInOut.sceneFadeInOut.sceneEnding  = true;
					//Application.LoadLevel("OpeningScene");
				
				}
			//} else if (levelToGoTo =="TimeLevel1")
			//{
			//		SceneFadeInOut.sceneFadeInOut.nextSceneName="IntroToTime";
			//		SceneFadeInOut.sceneFadeInOut.sceneEnding  = true;
							
			
			} else 
			{
				SceneFadeInOut.sceneFadeInOut.nextSceneName=levelToGoTo;
				SceneFadeInOut.sceneFadeInOut.sceneEnding  = true;
			}






			//SceneFadeInOut.sceneFadeInOut.nextSceneName=levelToGoTo;
			//SceneFadeInOut.sceneFadeInOut.sceneEnding  = true;
		}
	}

	void Update()
	{
		if (Input.GetKeyDown("c")){
			SceneFadeInOut.sceneFadeInOut.nextSceneName = levelToGoTo;
			SceneFadeInOut.sceneFadeInOut.sceneEnding   = true;
		}
	}

}
