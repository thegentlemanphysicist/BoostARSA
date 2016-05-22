using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CountdownTimerScript : MonoBehaviour {
	public float countdownTime=10f;
	float startTime, timeLeft;
	LevelDisplay levelDisplay;

	public Transform timerTransform;
	Text timerText;
	// Use this for initialization
	void Start () 
	{
		//find the objects I will be timing
		timerText = timerTransform.GetComponent<Text>();
		startTime = Time.time;

		levelDisplay = LevelDisplay.levelDisplay;

	}

	
	// Update is called once per frame
	void Update () 
	{
		if (!levelDisplay.levelCompleted)
		{
			timeLeft =countdownTime + startTime - Time.time;
			if (timeLeft>0f)
			{
				timerText.text = timeLeft.ToString();
			} else
			{
				PlatformerController.platformerController.SendMessage("OnDeath");
			}
		}
	
	}
}
