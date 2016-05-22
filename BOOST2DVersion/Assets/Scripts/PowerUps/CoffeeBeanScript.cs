using UnityEngine;
using System.Collections;

public class CoffeeBeanScript : MonoBehaviour {



	Vector3 beanPosition;
	float beanFrequency=4f;
	// Use this for initialization
	void Start () {
		//cowTrans = GameObject.Find ("CowBody").GetComponent<Transform>();
		beanPosition=transform.position;
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.CompareTag("Player"))
		{

			transform.parent.SendMessage("GotABean");
			Destroy(gameObject);
		}
	}


	// Update is called once per frame
	void Update () {
		transform.position= 
			new Vector3(beanPosition.x,
			            beanPosition.y + 1.0f*Mathf.Sin(beanFrequency*Time.time) ,
			            beanPosition.z);
	}
}
