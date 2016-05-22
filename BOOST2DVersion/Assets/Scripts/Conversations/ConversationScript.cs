using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;


public class ConversationScript : MonoBehaviour {
	public GameObject popUp;//this is for elements of canvas that we want to become active
	public GameObject forwardButton,backButton;


	//This script will read in a text file and animate a conversation between characters.
	string whichTrig;




	//These bool tell us if the text should start, and if it should run twice
	public bool beenTriggered, allowsMultTrig = false;
	bool canBeTrigerred;
	bool dialogueBoxOpen;

	public GUISkin dialogueSkin; 
	public string fileName;
	//these will be called to make the dialogue make sense
	public Texture jennyPic, drFrizPic;
	private Texture currentSpeaker;


	private class TrigName : List<string>{};
	private class Speaker  : List<string> {};
	private class Dialogue : List<string> {};
	public  class Triggers : List<GameObject>{};

	TrigName trigName = new TrigName();//the name of the trigger Jenny is talking to
	Speaker  speaker  = new Speaker(); //the name of the speaker
	Dialogue dialogue = new Dialogue();//the actual line of dialogue


	int lineOfDialogue, firstLineOfDialogue, lastLineOfDialogue;



	//private TextReader tr; 
	//private string path; 



	string credits;

	
	float guiHeight, guiWidth;
	float textSize;
	
	private float screenHeight, screenWidth;
	float smallButtonHeight, smallButtonWidth;


	//bool testBoxOpen=false,testBoxOpen2=false;




	TextAsset txt;
	public string theWholeTextFile;
	public string[] linesFromtxtFile;
	

	public GameObject dialogueBox, speakerBox, dialogueHolder;
	Text dialogueText;
	RawImage speakerPic;


	// Use this for initialization
	void Start () 
	{
		beenTriggered  = false;
		canBeTrigerred = false;
		dialogueBoxOpen= false; 


		screenWidth  = Screen.width;//is 1336 when the window is fullscreen
		screenHeight = Screen.height;
		textSize     = 20*(screenWidth/1336);
		dialogueSkin.box.fontSize = (int)textSize;



		txt = (TextAsset)Resources.Load("TextResources/"+fileName, typeof(TextAsset));
		theWholeTextFile = txt.text;
		//parse the file.
		linesFromtxtFile = theWholeTextFile.Split('\n');
		// read this array into trigName, speaker, and dialogue
		int i =0;
		
		while (i+2 <linesFromtxtFile.Length)
		{
			trigName.Add (linesFromtxtFile[i]);
			i++;
			speaker.Add(linesFromtxtFile[i]);
			i++;
			dialogue.Add(linesFromtxtFile[i]);
			i++;
		}




		/////GET THE DIALOGUE BOX ELEMENTS OF THE CANVAS
		dialogueBox = GameObject.FindGameObjectWithTag("Canvas").transform.FindChild("DialogueBox").gameObject;
		dialogueBox.SetActive(false);
		speakerBox  = dialogueBox.transform.FindChild("SpeakerPic").gameObject;
		dialogueHolder = dialogueBox.transform.FindChild("TextHolder").FindChild("Dialogue").gameObject;
		dialogueText = dialogueHolder.transform.GetComponent<Text>();
		speakerPic	 = speakerBox.transform.GetComponent<RawImage>();
		//speakerPic.texture = drFrizPic;
		//dialogueText.text = "The text is hello world";




		//this pushes the text into box to make toom for the picture
		dialogueSkin.box.padding.left = (int)(0.2f*screenHeight);


	}


	public void OnDialogueTrigger (string trigNameString){
		whichTrig = trigNameString;
		//this returns the index of the first entry with this trigger name

		firstLineOfDialogue = trigName.IndexOf(whichTrig);
		lastLineOfDialogue  = trigName.LastIndexOf(whichTrig);
		lineOfDialogue      = firstLineOfDialogue; 

		canBeTrigerred = true;


	}




	public void OnDialogueExit()
	{
		canBeTrigerred = false;
		dialogueBoxOpen = false;
		dialogueBox.SetActive(false);

	
	}





	void Update()
	{


		//each time I hit space or click on an arrow button the dialogue moves forward.
		if (canBeTrigerred 
		    && Input.GetButtonDown("PickUp") 
		    && (dialogue[lineOfDialogue]!=null) )
		{

			if (dialogueBoxOpen && lineOfDialogue < lastLineOfDialogue )
			{

				lineOfDialogue++;
				PickSpeaker();

				speakerPic.texture = currentSpeaker;
				dialogueText.text = dialogue[lineOfDialogue];


			} else if (dialogueBoxOpen) {
				dialogueBoxOpen=false;
				dialogueBox.SetActive(false);
				lineOfDialogue = firstLineOfDialogue;
			} else if (lineOfDialogue != -1) //if the tag which trig is not found
			{
				beenTriggered   = true;
				GetComponent<AudioSource>().Play();
				PickSpeaker();
				speakerPic.texture = currentSpeaker;
				dialogueText.text = dialogue[lineOfDialogue];
				dialogueBoxOpen = true;
				dialogueBox.SetActive(true);
			} 


			if (lineOfDialogue>firstLineOfDialogue)
			{
				//MAKE THE BACK BUTTON APPEAR
				backButton.SetActive(true);
			} else 
			{
				backButton.SetActive(false);
			}

			if (lineOfDialogue <lastLineOfDialogue) 
			{
				//MAKE THE FORWARD BUTTON ACTIVE
				forwardButton.SetActive(true);
			} else 
			{
				forwardButton.SetActive(false);
			}



		}
	

	
	}


	public void nextLineOfDialogue()
	{
		if (lineOfDialogue < lastLineOfDialogue)
		{
			lineOfDialogue +=1;
			PickSpeaker();
			speakerPic.texture = currentSpeaker;
			dialogueText.text = dialogue[lineOfDialogue];

		} else {
			dialogueBoxOpen=false;
			dialogueBox.SetActive(false);
			lineOfDialogue = firstLineOfDialogue;
		}

		
		//check if I should show button
		if (lineOfDialogue>firstLineOfDialogue)
		{
			//MAKE THE BACK BUTTON APPEAR
			backButton.SetActive(true);
		} else 
		{
			backButton.SetActive(false);
		}
		
		if (lineOfDialogue <lastLineOfDialogue) 
		{
			//MAKE THE FORWARD BUTTON ACTIVE
			forwardButton.SetActive(true);
		} else 
		{
			forwardButton.SetActive(false);
		}




	}

	public void prevLineOfDialogue()
	{
		if (lineOfDialogue > firstLineOfDialogue)
		{
			lineOfDialogue -=1;
			PickSpeaker();
			speakerPic.texture = currentSpeaker;
			dialogueText.text = dialogue[lineOfDialogue];
		}




		//check if I should show button
		if (lineOfDialogue>firstLineOfDialogue)
		{
			//MAKE THE BACK BUTTON APPEAR
			backButton.SetActive(true);
		} else 
		{
			backButton.SetActive(false);
		}
		
		if (lineOfDialogue <lastLineOfDialogue) 
		{
			//MAKE THE FORWARD BUTTON ACTIVE
			forwardButton.SetActive(true);
		} else 
		{
			forwardButton.SetActive(false);
		}



	}







	void PickSpeaker()
	{

		if (speaker[lineOfDialogue] == "J")
		{
			//insert Jenny's pic
			currentSpeaker = jennyPic;

		} else if (speaker[lineOfDialogue] == "F")
		{
			//insert Friz's pic
			currentSpeaker = drFrizPic;
		} else {
			//send error message or add new options for speakers
			currentSpeaker = null;
			dialogueBoxOpen=false;
			dialogueBox.SetActive(false);
			lineOfDialogue = firstLineOfDialogue;
			if (popUp != null)
			{
				popUp.SetActive(true);
			}

		}

	}










}
