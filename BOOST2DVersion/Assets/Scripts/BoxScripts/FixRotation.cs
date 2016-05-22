using UnityEngine;
using System.Collections;

public class FixRotation : MonoBehaviour {
	
	
	public Quaternion GlobalRotation;
	// Use this for initialization
	void Start () {
	//Initialize with the original rotation
		GlobalRotation=transform.rotation ;
	}
	
	// Update is called once per frame
	void Update () {
	//transform.rotation = GlobalRotation;
		GlobalRotation=transform.rotation ;
	}
}
