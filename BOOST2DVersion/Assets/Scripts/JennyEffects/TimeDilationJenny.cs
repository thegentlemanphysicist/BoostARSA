using UnityEngine;
using System.Collections;

public class TimeDilationJenny : MonoBehaviour {
	/// <summary>
	/// This maintains Jenny's clock.  There should be a way of saving 
	/// the time it took to complete the level in the state of the game
	/// so it can be used in scoring
	/// Will need to access LorentzContractionForJenny
	/// </summary>
	
	
	
	LorentzContractionForJenny lorentzContractionForJenny;
	
	
	
	public float jennyTime;
	
	/// // Use this for initialization
	void Start () {
		lorentzContractionForJenny = LorentzContractionForJenny.lCJenny;
			//GetComponent ("LorentzContractionForJenny") as LorentzContractionForJenny;
	
		jennyTime=0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		jennyTime+= Time.deltaTime/lorentzContractionForJenny.gammaX;
	}
}
