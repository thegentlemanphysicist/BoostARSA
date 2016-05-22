using UnityEngine;
using System.Collections;

public class CircleCastExposion : MonoBehaviour {


	public bool runTest;
	// Use this for initialization
	void Start () {
		runTest = true;
	}
	void TestCircleCast()
	{
		RaycastHit2D[] hitByCircle;
		hitByCircle = Physics2D.CircleCastAll(
			new Vector2(transform.position.x,transform.position.y),
		    10.0f,
			Vector2.zero
			); 
	

		for (int i=0 ; i<hitByCircle.Length;i++)
		{
			Debug.Log("I hit "+ hitByCircle[i].transform.name);

		}

	}
	// Update is called once per frame
	void Update () 
	{
		if (runTest)
		{
			TestCircleCast();
			runTest=false;
		}


	}
}
