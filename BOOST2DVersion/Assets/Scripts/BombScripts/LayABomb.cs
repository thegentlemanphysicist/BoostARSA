using UnityEngine;
using System.Collections;

public class LayABomb : MonoBehaviour {
	// This script spawns a bomb which will drop down for our hero to use.
	public GameObject theBomb;
	GameObject bombClone;


	// Use this for initialization
	void Start () {
	//spawn the first bomb.

		//photonPulseClone = Instantiate(horizontalPhotonPulseUV,locationOfShot,
		  //                             horizontalPhotonPulseUV.transform.rotation) as GameObject;
		bombClone = Instantiate(theBomb, transform.position ,Quaternion.identity ) as GameObject;
		bombClone.transform.parent = transform;
	}

	void BombGoBoom(){
		//spawn a new bomb to replace his fallen comrade
		bombClone = Instantiate(theBomb, transform.localPosition,Quaternion.identity ) as GameObject;
		bombClone.transform.parent = transform;
	}


}
