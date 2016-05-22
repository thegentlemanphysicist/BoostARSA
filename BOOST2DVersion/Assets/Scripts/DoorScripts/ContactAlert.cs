using UnityEngine;
using System.Collections;

public class ContactAlert : MonoBehaviour {


	//	public Transform target;
	//	ColourDoorTrigger colourDoorTrigger;

	// Use this for initialization
	void Start () {
	//		target=transform.parent.FindChild("Target");


		//doorTrigger = transform.parent.FindChild("Target");
//		colourDoorTrigger = target.GetComponent("ColourDoorTrigger")	as ColourDoorTrigger;
	}

			//	LorentzContractionForJenny lorentzContractionForJenny;
//	lorentzContractionForJenny =gameObject.GetComponent("LorentzContractionForJenny") 
//		as LorentzContractionForJenny;	

	void OnTriggerEnter (){
		//colourDoorTrigger.beamCollidedWithObj = true;
	}

	void OnTriggerExit (){
		//colourDoorTrigger.beamCollidedWithObj = true;
	}

}
