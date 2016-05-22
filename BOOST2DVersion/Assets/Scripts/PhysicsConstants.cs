using UnityEngine;
using System.Collections;

public class PhysicsConstants : MonoBehaviour {
	//This class is present in every scene and gives us the physical constants in the game
	//most importantly, the speed of light.
	
	
	public float SpeedOfLight     =30.0f;
	public float SpeedOfLightY    = 100.0f;
	//this weekens the dopple effect so it's more visually interesting in the range we use it.
	public float dopplerSoftening = 0.1f;
	public float baseWavelength	  = 475f;	
	/*
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}*/
}
