using UnityEngine;
using System.Collections;

public class TimerPosition : MonoBehaviour {
	/// <summary>
	/// Here we keep the timer in a fixed position relative to the bomb.
	/// </summary>
	
	
	Vector3 timerPosition;
	Transform bombParent, bombTransform;
	Vector3 offset = new Vector3(0.0f,10.0f,-4.0f); 
	
	
	
	
	// Use this for initialization
	void Start () {
		bombParent = transform.parent;
		bombTransform = bombParent.FindChild("Bomb").transform;
		transform.position=bombTransform.position+offset;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position=bombTransform.position+offset;
	}
}
