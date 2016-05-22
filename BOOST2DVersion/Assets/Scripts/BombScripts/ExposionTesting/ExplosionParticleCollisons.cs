using UnityEngine;
using System.Collections;

public class ExplosionParticleCollisons : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	
	void OnCollisionStay(Collision collision) {
		Debug.Log("I was hit by" + collision.transform.name);
	}
	
	void OnParticleCollision(GameObject other) {
			Debug.Log("The explosion hit" + other.name);
	//	if (particleGameobjec.name == "Explosion") {
	//		Debug.Log("The explosion hit me");
	//	}
	}
	
	
	
	// Update is called once per frame
	void Update () {
	
	}
}
