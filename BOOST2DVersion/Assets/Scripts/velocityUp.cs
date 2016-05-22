using UnityEngine;
using System.Collections;

public class velocityUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 50.0f, 0.0f);
	}
}
