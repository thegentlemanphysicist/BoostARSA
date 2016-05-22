using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;


public class creditReaderLameEnding : MonoBehaviour {

	public float timeOnScreen; 
	float timeAtStart;
	//private TextReader tr; 
	private string path; 
	
	string credits;




	RectTransform panelTransform;
	Text creditTextBox; 
	float finalHeight, initialHeight;

	TextAsset theClosingCredits;

	void Awake()
	{
		if (CameraTracking.cameraTracking !=null)
		{
			Destroy(CameraTracking.cameraTracking.gameObject);
		}

	}


	// Use this for initialization
	void Start ()
	{


		theClosingCredits = (TextAsset)Resources.Load("TextResources/lameEndCredits",typeof(TextAsset));
	

		credits = theClosingCredits.text;


		creditTextBox = transform.FindChild("Panel").FindChild("EpilogueText").GetComponent<Text>();
		creditTextBox.text = credits;
		panelTransform = transform.FindChild("Panel").GetComponent<RectTransform>();
		finalHeight = panelTransform.localPosition.y;
		initialHeight = 0;
		panelTransform.localPosition= new Vector3(
			0f,	initialHeight, 0f);



		//the level loads at the following time
		timeAtStart  = Time.time;
		timeOnScreen = 25f;







	}


	void Update()
	{
		panelTransform.localPosition= new Vector3(
			0f,	Mathf.Lerp(initialHeight,finalHeight, (Time.time - timeAtStart)/timeOnScreen )  , 0f);
	


		if (Input.GetKeyDown("q")){
			Application.LoadLevel("StartScreen");
		}
	
	}





	public void quitToMainMenu()
	{
		Application.LoadLevel("StartScreen");
	}

	public void goToIntroLev1()
	{
		Application.LoadLevel( "IntroToLevel1" );
	}

	public void goToLevel1()
	{
		Application.LoadLevel( "Level1" );
	}


}
