using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Vectrosity;

public class LevelDisplay : MonoBehaviour {

	public string transitionSceneName;

	public static LevelDisplay levelDisplay;


	/// <summary>
	/// This will display what the character needs to know:
	/// gamma factor
	/// timed dilation
	/// number of boosts left
	/// </summary>//
	private float screenHeight, screenWidth;//,buttonHeight, buttonWidth

	
	public bool levelCompleted, paused, playerDead;
	
	public Texture2D gammaPic;

	public Sprite mutedSprite, unMutedSprite;
	GameObject muteButtonGameObject;
	RectTransform muteButton;
	Image buttonImage;



	//the pause screen game object
	GameObject pauseScreen, levelCompleteScreen, playerDeadScreen;




	//We need to define the speed of light
	private float SpeedOfLight,oneOverC;
	//The physics constants give us the speed of light in one place
	//PhysicsConstants physicsConstants; 
	//GameObject physConst;
	
	
	GameObject player;
	PlatformerController platformerController;
	GameControl control;
	int nextLevelNum;

	public float gammaX, speed;	
	float gammaXRounded, percentSpeedLight;
	
	
	//the dials that show recovery of pulse and boost
	public GameObject shotDial, boostDial;
	Transform     gammaDispValue,   velocityDispValue;
	RectTransform gammaDispValueRT, velocityDispValueRT;
	Text          gammaDispText,    velocityDispText;
	Canvas		  levelCanvas;
	Vector3 bottomCornerOfScreen;




	void Awake () 
	{
		//makes it a singleton
		if (levelDisplay == null)
		{
			//DontDestroyOnLoad(gameObject);
			levelDisplay = this;
		} else if ( levelDisplay != this)
		{
			Destroy(gameObject);
		}
		
	}

	
	
	void Start () {
		//This should make the canvas render the correct camera.
		//camera=CameraTracking.cameraTracking.gameObject;
		levelCanvas = transform.GetComponent<Canvas>();
		levelCanvas.worldCamera = Camera.main;
		//RectTransform.
		//Canvas.worldCamera = Camera.main;

		shotDial = DopplerCircle3.dopplerCircle3.gameObject;//GameObject.Find("ShotRecovery");
		boostDial= GameObject.Find("BoostRecovery");
		gammaDispValue    = GameObject.Find("gammaValue").transform;
		velocityDispValue = GameObject.Find("velocityValue").transform;

		gammaDispValueRT    = (RectTransform) gammaDispValue.GetComponent<RectTransform>();
		velocityDispValueRT = (RectTransform) velocityDispValue.GetComponent<RectTransform>();


		gammaDispText    = gammaDispValueRT.GetComponent<Text> ();
		velocityDispText = velocityDispValueRT.GetComponent<Text>();


		//physConst = GameObject.Find ("PhysicalConstants");		
		//physicsConstants = physConst.GetComponent("PhysicsConstants") as PhysicsConstants;
		control 	  = GameControl.control;
		SpeedOfLight  = control.SpeedOfLight;//  physicsConstants.SpeedOfLight;

		//SpeedOfLight=physicsConstants.SpeedOfLight;
		//division is more expensive than multiplication
		oneOverC = 1.0f/SpeedOfLight;
		
		
		player = GameObject.Find ("Jenny");		
		playerDead = false;
		//platformerController = PlatformerController.platformerController as PlatformerController;
		platformerController = PlatformerController.platformerController;
			
		gammaX = platformerController.movement.gammaX;

		gammaXRounded = 1.0f;
		speed = 0.0f;
		screenHeight = Screen.height;
		screenWidth  = Screen.width;


		
		muteButtonGameObject = GameObject.Find("MuteButton");
		muteButton = (RectTransform) muteButtonGameObject.GetComponent<RectTransform>();
		buttonImage= muteButton.GetComponent<UnityEngine.UI.Image>();
		//THIS MAKES SURE THE GAME STAYS MUTED BETWEEN LEVELS
		if (PlayerPrefs.GetInt("Muted")==1)
		{
			//pause the audio listener
			AudioListener.pause = true;
			buttonImage.overrideSprite = mutedSprite;
		} else 
		{
			AudioListener.pause = false;
			buttonImage.overrideSprite = unMutedSprite;
		}


			





		
		
		levelCompleted = false;
		//This makes sure the level isn't paused if I leave while paused
		paused = false;
		Time.timeScale = 1.0f;



		
		
		pauseScreen 		= transform.FindChild("PauseScreen").gameObject;
		levelCompleteScreen = transform.FindChild("LevelComplete").gameObject;
		playerDeadScreen	= transform.FindChild("PlayerDead").gameObject;






	}
	

	void Update(){

		///Tell the gamma label and velocity label the correct values
		gammaXRounded     = Mathf.Round(gammaX*100f)*0.01f;
		percentSpeedLight = Mathf.Round(speed*oneOverC*1000f)*0.1f;
		//gammaDispValue.guiText = ToString(gammaXRounded);
		gammaDispText.text=  gammaXRounded.ToString();    
		velocityDispText.text  =  percentSpeedLight.ToString();
		
		if (Input.GetKeyDown("m")){
			//mute the game
			muteGame();
					
		}




		if(Input.GetKeyDown("p") || Input.GetKeyDown("escape")){
			if (!paused) {
				Time.timeScale = 0.0f;
				platformerController.canControl=false;
				//Debug.Log ("Turned off shot state 1");
				JennyShoot.jennyShoot.shotState =  JennyShoot.ShotState.turnOffShoot;
				pauseScreen.SetActive(true);
				paused = true;
			} else {
				Time.timeScale = 1.0f;
				platformerController.canControl=true;
				JennyShoot.jennyShoot.shotState =  JennyShoot.ShotState.turnOnShoot;
				pauseScreen.SetActive(false);
				paused = false;
			}

		} 


		if (paused)
		{

			//NOW HAVE ALL THESE COMMANDS REPEATED WITH KEYS FROM THE KEYBOARD
			//PAUSE IS ALREADY TAKEN CARE OF 
			if (Input.GetKeyDown("q")){
				//unpause the game
				Time.timeScale = 1.0f;
				platformerController.canControl=true;
				pauseScreen.SetActive(false);
				paused = false;
				VectorLine.Destroy(ref JennyShoot.jennyShoot.gunLineVec);
				Application.LoadLevel("StartScreen");
			}
			if (Input.GetKeyDown("r")){

				//unpause the game
				Time.timeScale = 1.0f;
				platformerController.canControl=true;
				pauseScreen.SetActive(false);
				paused = false;

				VectorLine.Destroy(ref JennyShoot.jennyShoot.gunLineVec);
				Application.LoadLevel(Application.loadedLevel);
			}
		}


		if (levelCompleted)
		{

			//NOW HAVE ALL THESE COMMANDS REPEATED WITH KEYS FROM THE KEYBOARD
			//PAUSE IS ALREADY TAKEN CARE OF 
			if (Input.GetKeyDown("q")){
				VectorLine.Destroy(ref JennyShoot.jennyShoot.gunLineVec);
				Application.LoadLevel("StartScreen");
			}
			if (Input.GetKeyDown("r")){
				VectorLine.Destroy(ref JennyShoot.jennyShoot.gunLineVec);
				Application.LoadLevel(Application.loadedLevel);
			}

			if (Input.GetKeyDown("c")){
			//Load next level.
				VectorLine.Destroy(ref JennyShoot.jennyShoot.gunLineVec);
				if (transitionSceneName ==null)
				{
					Application.LoadLevel(Application.loadedLevel +1);
				} else 
				{
					SceneFadeInOut.sceneFadeInOut.nextSceneName= transitionSceneName;
					SceneFadeInOut.sceneFadeInOut.sceneEnding  = true;
				}
		
			}
		
		}


		if (playerDead && !levelCompleted){
								
			if (Input.GetKeyDown("q")){
				VectorLine.Destroy(ref JennyShoot.jennyShoot.gunLineVec);
				Application.LoadLevel("StartScreen");
			}
			if (Input.GetKeyDown("r")){
				VectorLine.Destroy(ref JennyShoot.jennyShoot.gunLineVec);
				Application.LoadLevel(Application.loadedLevel);
			}
			
		}







	}




	



	void PlayerDied()
	{
		if (!playerDead  && !levelCompleted)
		{
			playerDead = true;
			playerDeadScreen.SetActive(true);
			//deactivate shootability
			JennyShoot.jennyShoot.shotState =  JennyShoot.ShotState.turnOffShoot;
		}
	}

	public void quitToMainMenu()
	{
		VectorLine.Destroy(ref JennyShoot.jennyShoot.gunLineVec);
		Application.LoadLevel("StartScreen");
	}
	
	public void restartLevel()
	{
		VectorLine.Destroy(ref JennyShoot.jennyShoot.gunLineVec);
		Application.LoadLevel(Application.loadedLevel);
	}
	
	public void unPause()
	{
		Time.timeScale = 1.0f;
		platformerController.canControl=true;
		JennyShoot.jennyShoot.shotState =  JennyShoot.ShotState.turnOnShoot;
		pauseScreen.SetActive(false);
		paused = false;
	}
	
	public void goToNextLevel()
	{
		VectorLine.Destroy(ref JennyShoot.jennyShoot.gunLineVec);
		if (transitionSceneName ==null)
		{
			Application.LoadLevel(Application.loadedLevel +1);
		} else 
		{
			SceneFadeInOut.sceneFadeInOut.nextSceneName= transitionSceneName;
			SceneFadeInOut.sceneFadeInOut.sceneEnding  = true;
		}

	}







	public void muteGame()
	{
		//http://docs.unity3d.com/ScriptReference/AudioListener-pause.html


		//pause the audio listener
		AudioListener.pause = !AudioListener.pause;
		if (AudioListener.pause == true )
		{
			//save mute state to game state
			PlayerPrefs.SetInt("Muted", 1);
			//put audio mute image
			buttonImage.overrideSprite = mutedSprite;
		} else
		{
			//save mute state to game state
			PlayerPrefs.SetInt("Muted", 0);
			//put audio unmute image
			buttonImage.overrideSprite = unMutedSprite;

		}

	}



	void LevelComplete () {
		if(!levelCompleted)
		{
			//Display Message "Level Complete"
			levelCompleted = true;
			levelCompleteScreen.SetActive(true);

			control.SendMessage("LoadGameData");

			//log game state in controler then save the state
			if ( Application.loadedLevel != null && Application.loadedLevel <= control.numberOfLevels)
			{

				//Debug.Log("The loaded level number is="+Application.loadedLevel);
				//Debug.Log("The coffee bean number is ="+control.coffeeBeans[Application.loadedLevel - 1] );
				if (CoffeeBeanManager.coffeeBeanManager==null)// control.coffeeBeans[Application.loadedLevel - 1]==null)
				{
					Debug.Log("The coffee bean manager aint here");
				} else 
				{
					if (control.coffeeBeans.Length<Application.loadedLevel )
					{
						control.coffeeBeans[Application.loadedLevel - 1] 
							= Mathf.Max(CoffeeBeanManager.coffeeBeanManager.coffeeBeansCaptured,
					            control.coffeeBeans[Application.loadedLevel - 1]);
					} else
					{
						Debug.Log("The coffee bean matrix not big enough");
					}
				}
			}
		

			control.SendMessage("SaveGameData");

			//turn off shotdial
			JennyShoot.jennyShoot.shotState =  JennyShoot.ShotState.turnOffShoot;


		}
		
	}
	
	

	

	
	
}
