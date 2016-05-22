using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class OpeningSceneOverlay : MonoBehaviour {

	//GameObject player;
	PlatformerController platformerController;

	private float SpeedOfLight;
	//PhysicsConstants physicsConstants; 
	//GameObject physConst;


	GameObject choosingScreen;

	// Use this for initialization
	void Start () {
		//physConst = GameObject.Find ("PhysicalConstants");		
		//physicsConstants = physConst.GetComponent("PhysicsConstants") as PhysicsConstants;
		//SpeedOfLight=physicsConstants.SpeedOfLight;
		SpeedOfLight  = GameControl.control.SpeedOfLight;

		//player = GameObject.Find ("Jenny");		
		platformerController =  PlatformerController.platformerController;
		//player.GetComponent("PlatformerController") as PlatformerController;	
		platformerController.canControl = false;
		//on space bar, close the overlay
		//have a left or right button that sends you to one or the other 
		choosingScreen = transform.FindChild("Decide").gameObject;
		choosingScreen.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("PickUp"))
		{
			transform.FindChild("PressSpace").gameObject.SetActive(false);
		}



		if (choosingScreen.activeSelf)
		{
			if (Input.GetKeyDown("a"))
		    {
				GoHome();
			}
			if (Input.GetKeyDown("d"))
			{
				GoLevel1();
			}

		}


	}

	public void GoLevel1()
	{
		platformerController.movement.direction = new Vector3(1f,0f,0f);
		platformerController.movement.gammaX =1.5f;
		platformerController.movement.targetSpeed= GetVxTargetFromGammaX (1.5f );

	}



	public void GoHome()
	{
		platformerController.movement.direction = new Vector3(-1f,0f,0f);
		platformerController.movement.gammaX =1.5f;
		platformerController.movement.targetSpeed= GetVxTargetFromGammaX (1.5f );//  platformerController.GetVxTargetFromGammaX ( movement.gammaX );
	}
	//This function will convert the new gamma factor to an xvelocity
	float  GetVxTargetFromGammaX (float GAMMAX ){
		return SpeedOfLight * Mathf.Sqrt(  1.0f -  Mathf.Pow(GAMMAX, -2)   );
	}



}
