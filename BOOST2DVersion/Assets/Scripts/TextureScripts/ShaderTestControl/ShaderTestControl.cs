using UnityEngine;
using System.Collections;

public class ShaderTestControl : MonoBehaviour {
	//Material theTestCrateMaterial;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.GetComponent<Renderer>().material.SetFloat("_Cutoff", Mathf.Sin(Time.time));
	}
}
