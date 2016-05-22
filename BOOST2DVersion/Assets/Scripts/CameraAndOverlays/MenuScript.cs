using UnityEngine;
using System.Collections;
//using JsonConvert;


public class MenuScript : MonoBehaviour {
	


	GameObject mainMenuPanel, levelSelectPanel, eraseProgessPanel, controlsPanel;
	int sceneNumber;

	//public Transform level2button, level3button,level4button, level5button, level6button;
	//public Texture lockTexture, icon1, icon2, icon3;
	public Transform levelButtons;

	private delegate void MenuDelegate();
	private MenuDelegate menuFunction;
	
	
	private float screenHeight, screenWidth, buttonHeight, buttonWidth;
	
	// Use this for initialization
	void Start () {
		mainMenuPanel     = transform.FindChild("OpeningMenu").gameObject;
		levelSelectPanel  = transform.FindChild("PickALevel").gameObject;
		eraseProgessPanel = transform.FindChild("EraseProgress").gameObject;
		controlsPanel	  = transform.FindChild("Controls").gameObject;
		//this will set the start screen as main menu
		MainMenuScreen();



		screenHeight = Screen.height;
		screenWidth  = Screen.width;
		buttonHeight = screenHeight*0.15f;
		buttonWidth  = screenWidth*0.4f;
		
		menuFunction = mainMenu;
		
		
		//levelButtons = new Transform[] {  level2button, level3button,level4button, level5button, level6button};

		LockLevels(levelButtons);
		


		
		
		
	}
	
	
	void OnGUI(){
		menuFunction();	
	}
	
	
	/*void anyKey()
	{
		if(Input.anyKey){
			menuFunction = mainMenu;
		
		}
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect( screenWidth*0.45f,screenHeight*0.45f, screenWidth*0.1f,
			screenHeight*0.1f), "Press any key to continue");
		
	}*/


	
	void mainMenu(){
		//if (GUI.Button (new Rect((screenWidth-buttonWidth)*0.5f, screenHeight*0.1f, buttonWidth, buttonHeight ), "Start Game")){
			//Application.LoadLevel
		//}
		/*if (GUI.Button (new Rect((screenWidth-buttonWidth)*0.5f, screenHeight*0.25f, buttonWidth, buttonHeight ), "PLAY")){
			//Load the lowest level not completed
			//if the opening scene hasn't been run
			if (PlayerPrefs.GetInt("OpeningSceneCompleted")==0){
				Application.LoadLevel("OpeningScene");
			}else {
				//I need to be careful how I define a level as complete
				Application.LoadLevel( PlayerPrefs.GetInt("LevelsCompleted",0) + 1 );
			}
		}
		if (GUI.Button (new Rect((screenWidth-buttonWidth)*0.5f, screenHeight*0.40f, buttonWidth, buttonHeight ), "Level Select")){
			menuFunction =loadLevelMenu;
		}
		if (GUI.Button (new Rect((screenWidth-buttonWidth)*0.5f, screenHeight*0.55f, buttonWidth, buttonHeight ), "Quit Game")){
			Application.Quit();
		}
		
		if (GUI.Button (new Rect((screenWidth-buttonWidth)*0.5f, screenHeight*0.70f, buttonWidth, buttonHeight ), "Erase Progress")){
			menuFunction = eraseProgressMenu;
		}*/
		
	}
	
	void eraseProgressMenu() {
		/*if (GUI.Button (new Rect((screenWidth-buttonWidth)*0.5f, screenHeight*0.1f, buttonWidth, buttonHeight ), "Back To Menu")){
			menuFunction = mainMenu;
		}
		
		if (GUI.Button (new Rect((screenWidth-buttonWidth)*0.5f, screenHeight*0.25f, buttonWidth, buttonHeight ), "Erase Progress")){
			PlayerPrefs.DeleteAll();
			menuFunction = mainMenu;
		}*/
	}
	
	void loadLevelMenu(){
		/*if(GUI.Button (new Rect((screenWidth-buttonWidth)*0.5f, screenHeight*0.1f, buttonHeight, buttonHeight ), icon1)){
			//if the opening scene hasn't been run
			if (PlayerPrefs.GetInt("OpeningSceneCompleted")==0){
				Application.LoadLevel("OpeningScene");
			}else {
				//I need to be careful how I define a level as complete
 				Application.LoadLevel( "Level1" );
			}
				//just to check the next statement works
			//PlayerPrefs.SetInt("Level2Open",1);
			//load scene Level ??
		};

		//level two icon
		if (PlayerPrefs.GetInt( "Level2Open" , 0 ) == 1 ){
			if(GUI.Button (new Rect((screenWidth-buttonWidth)*0.5f+buttonHeight, screenHeight*0.1f, buttonHeight, buttonHeight ), icon2)){
				Application.LoadLevel("Level2");
			};
		} else {

			GUI.Box(new Rect((screenWidth-buttonWidth)*0.5f+buttonHeight, screenHeight*0.1f, buttonHeight, buttonHeight ), lockTexture);
		}

		//level 3 icon
		if (PlayerPrefs.GetInt( "Level3Open" , 0 ) == 1 ){
			if(GUI.Button (new Rect((screenWidth-buttonWidth)*0.5f+2.0f*buttonHeight, screenHeight*0.1f, buttonHeight, buttonHeight ), icon3)){
				Application.LoadLevel("Level3");
			};
		} else {
			
			GUI.Box(new Rect((screenWidth-buttonWidth)*0.5f+2.0f*buttonHeight, screenHeight*0.1f, buttonHeight, buttonHeight ), lockTexture);
		}






		//return to main
		if (GUI.Button (new Rect((screenWidth-buttonWidth)*0.5f, screenHeight*0.75f, buttonWidth, buttonHeight ), "Back To Menu")){
			menuFunction = mainMenu;
		}*/
	}
	

	public void PlayGame()
	{
		if (PlayerPrefs.GetInt("OpeningSceneCompleted")==0){
			Application.LoadLevel("OpeningScene");
		}else if (PlayerPrefs.GetInt("OpeningSceneCompleted")==1)
		{
			Application.LoadLevel( "IntroToLevel1" );
		} else 
		{
			sceneNumber = PlayerPrefs.GetInt("LevelsCompleted",0);
			if (sceneNumber +1 == 4)//CHANGE THIS WITH BUILD SETTINGS!
			{
				//load the time intro level
				Application.LoadLevel( "IntroToTime" );
			} else {
				//I need to be careful how I define a level as complete
				Application.LoadLevel( PlayerPrefs.GetInt("LevelsCompleted",0) + 1 );
			}

		}
	}



	public void MainMenuScreen()
	{
		mainMenuPanel.SetActive(true);
		eraseProgessPanel.SetActive(false);
		levelSelectPanel.SetActive(false);
		controlsPanel.SetActive(false);

	}



	public void LevelSelectScreen()
	{
		mainMenuPanel.SetActive(false);
		eraseProgessPanel.SetActive(false);
		levelSelectPanel.SetActive(true);
		controlsPanel.SetActive(false);
		// this makes sure the correct levels are locked 	
		LockLevels(levelButtons);
	}

	public void EraseProgressScreen()
	{
		mainMenuPanel.SetActive(false);
		eraseProgessPanel.SetActive(true);
		levelSelectPanel.SetActive(false);
		controlsPanel.SetActive(false);
	}

	public void EraseProgress()
	{
		PlayerPrefs.DeleteAll();
		menuFunction = mainMenu;
		//go back to main menu
		MainMenuScreen();
	}

	public void ControlsScreen()
	{
		mainMenuPanel.SetActive(false);
		eraseProgessPanel.SetActive(false);
		levelSelectPanel.SetActive(false);
		controlsPanel.SetActive(true);
	}

	public void QuitGame()
	{

	}



	/*public void GoToLevel(int levelNumber)
	{
		if (levelNumber ==1)
		{
			//if the opening scene hasn't been run
			if (PlayerPrefs.GetInt("OpeningSceneCompleted")==1){
				//I need to be careful how I define a level as complete
				Application.LoadLevel( "IntroToLevel1" );

			}else {
				
				Application.LoadLevel("OpeningScene");
			}
		} else 
		{
			Application.LoadLevel( "Level"+levelNumber );
		}
	
	}*/


	public void GoToLevel(string levelName)
	{
		if (levelName =="Level1")
		{
			//if the opening scene hasn't been run
			if (PlayerPrefs.GetInt("OpeningSceneCompleted")==1){
				//I need to be careful how I define a level as complete
				Application.LoadLevel( "IntroToLevel1" );
				
			}else {
				
				Application.LoadLevel("OpeningScene");
			}
		} else 
		{
			Application.LoadLevel( levelName );
		}
		
	}





	public void LockLevels(Transform bTransform)
	{
		int levelOpen;
		string lastCharacterOfName;
		int buttonNumberMinus1;
		//for each child of pickalevel with tag levelbutton
		foreach (Transform t in bTransform)
		{
			//get the level name of the transform
			lastCharacterOfName = t.name;
			//Debug.Log ("The string is=" +  lastCharacterOfName.Substring(lastCharacterOfName.Length - 1));
			lastCharacterOfName = lastCharacterOfName.Substring(lastCharacterOfName.Length - 1);
			buttonNumberMinus1 = int.Parse(lastCharacterOfName)-1;
			// the -1 is because we want the previous level completed for a level to be unlocked

			if (buttonNumberMinus1>0)//can't be the button number for level 1
			{

				//check if that level has been completed
				
				levelOpen = PlayerPrefs.GetInt("Level" + buttonNumberMinus1 +"Completed");
				//Debug.Log ("Level" + buttonNumberMinus1 +"Completed = " +	levelOpen );
				if (levelOpen ==1)
				{
					t.gameObject.SetActive(true);
				//set active the button
				} else
				{
					t.gameObject.SetActive(false);
					//set active the lock
				}
			}
		}

	}


	public void UnlockAllLevels()
	{
		foreach (Transform t in levelButtons)
		{
			t.gameObject.SetActive(true);
		}
	}




}
