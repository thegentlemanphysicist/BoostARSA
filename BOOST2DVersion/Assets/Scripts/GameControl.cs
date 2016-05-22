using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class GameControl : MonoBehaviour {
	//since game control has a pubic static instance we can access it without using get or find
	public static GameControl control;



	public float SpeedOfLight     =30.0f;
	public float SpeedOfLightY    = 100.0f;
	//this weekens the dopple effect so it's more visually interesting in the range we use it.
	public float dopplerSoftening = 0.1f;
	public float baseWavelength	  = 475f;	
	public float deltaGamma 	  = 1f;
	public float gravity 		  = 12f;//may change this 

	public int numberOfLevels     = 6;

	//jumping
	public float jumpSpeed		  = 60f;
	public int   maxNumberOfJumps = 2;


	//bomb control
	public float explosionFallSmoothing = 0.2f;//keeps the explosion from falling into the ground


	public float elevatorRiseSpeed = 15f;
	public float elevatorPauseBottom = 1f;
	public float elevatorPauseTop	 = 1f;


	///PlayerData
	//These are assigned on level complete
	int levelsCompleted;
	public int[] coffeeBeans, coffeeBeansPerLevel;
	int numberOfDeaths;





	void Awake () 
	{
		//makes it a singleton
		if (control == null)
		{
			DontDestroyOnLoad(gameObject);
			control = this;
		} else if (control != this)
		{
			Destroy(gameObject);
		}


		InitialiseBlankData();
		LoadGameData();



	}

	

	public void InitialiseBlankData()
	{
		//initialize the game data
		coffeeBeansPerLevel = new int[] {3,3,3,3,3};//this has to be changed when the number of levels changed
		coffeeBeans = new int[numberOfLevels];//coffee beans gained per level


	}



	public void ClearGame()
	{
		InitialiseBlankData();
		//now save the blank file
		SaveGameData();
	}



	public void SaveGameData ()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open( Application.persistentDataPath +"/PlayerInfo.dat" , FileMode.OpenOrCreate);
		//FileStream file = File.Open( "Assets/Resources/PlayerInfo.dat" , FileMode.OpenOrCreate);
		PlayerData data = new PlayerData();
		data.levelsCompleted =levelsCompleted;
		data.numberOfDeaths  =numberOfDeaths;

		if (data.coffeeBeans!=null)
		{
			Array.Copy(coffeeBeans,data.coffeeBeans,numberOfLevels);
		} else
		{
			data.coffeeBeans = new int[numberOfLevels];
			Array.Copy(coffeeBeans,data.coffeeBeans,numberOfLevels);
		}


		bf.Serialize(file, data);
		file.Close();
		//Debug.Log("The beans caught in level 1 is="+data.coffeeBeans[0]);

	}



	public void LoadGameData ()
	{


		if (File.Exists(Application.persistentDataPath +"/PlayerInfo.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath +"/PlayerInfo.dat",
			                            FileMode.Open);
			//FileStream file = File.Open("Assets/Resources/PlayerInfo.dat",
			//                            FileMode.Open);
			PlayerData data = (PlayerData)bf.Deserialize(file);
			file.Close();

			if (numberOfLevels == data.numberOfLevels)
			{//if the number of levels has changed we need to write over it

				//assign the saved values to the local variables;
				levelsCompleted = data.levelsCompleted; 
				numberOfDeaths  = data.numberOfDeaths;
				//if coffeeBeans exist load it, else create it
				if (data.coffeeBeans!=null)
				{
					Array.Copy(data.coffeeBeans,coffeeBeans,numberOfLevels);
				} else {
					data.coffeeBeans = new int[numberOfLevels];
				}


			} else 
			{//If the number of levels changes the game data has to get changed
				data.numberOfLevels = numberOfLevels;
				data.coffeeBeans = new int[numberOfLevels];
			}


		}

	}

	public void OnDeath()
	{
		LoadGameData();
		numberOfDeaths += 1;
		SaveGameData();	
	}





}



[Serializable]
class PlayerData
{
	public int numberOfLevels;//number of levels in the game

	public int[] coffeeBeans;//number of coffee beans found per level


	public int levelsCompleted;
	public int numberOfDeaths;

	public bool OpeningLevel1Complete, OpeningLevel2Complete, OpeningLevel3Complete;
	public bool TimeLevel1Complete, TimeLevel2Complete;
	//need to make an array for each level containing number of boosts, number of coffee beans 


}

