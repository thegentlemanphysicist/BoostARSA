using UnityEngine;
using System.Collections;

public class SendReflectionMessage : MonoBehaviour {

	float angleOfMirror;

	// Use this for initialization
	void Start () {
		angleOfMirror= 0.0f;



	}

	void OnCollisionEnter(){
	
		angleOfMirror = transform.rotation.z;
		SendMessage("OnReflect",angleOfMirror,
		            SendMessageOptions.DontRequireReceiver);
	
	}






}
