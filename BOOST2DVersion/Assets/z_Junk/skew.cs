using UnityEngine;
using System.Collections;

public class skew : MonoBehaviour {

	public Matrix4x4 m;
	public Matrix4x4 local_to_world;
	public Matrix4x4 world_to_local;
	
	// Use this for initialization
	void Start () {
		m = Matrix4x4.identity;
		m.m01 = 1.0f;
		local_to_world = transform.worldToLocalMatrix;
		world_to_local = transform.localToWorldMatrix;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
