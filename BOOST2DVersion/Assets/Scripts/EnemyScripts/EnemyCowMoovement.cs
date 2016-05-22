using UnityEngine;
using System.Collections;

public class EnemyCowMoovement : MonoBehaviour {

	public Transform cowTrans;
	Vector3 cowPosition;
	
	// Use this for initialization
	void Start () {
		//cowTrans = GameObject.Find ("CowBody").GetComponent<Transform>();
		cowPosition=cowTrans.position;
	}
	
	// Update is called once per frame
	void Update () {
		cowTrans.position= 
			new Vector3(cowPosition.x,
				cowPosition.y + 1.0f*Mathf.Sin(2.0f*Time.time) ,
				cowPosition.z);
	}
}
