using UnityEngine;
using System.Collections;

public class DestroyPulse : MonoBehaviour {

	// This destroys the pulse after 20 seconds
	void Start () {
		Destroy(gameObject,20.0f);
	}
	

}
