using UnityEngine;
using System.Collections;

public class PlaceContactPoints : MonoBehaviour {
	//The purpose of this script is to place tiny spheres at the contact points of collisions
	//I hope to learn the difference between onCollisionEnter,Stay, and Exit
	GameObject contactSphere;
	public bool doRaySweep;	
	
	
	void OnCollisionEnter(Collision collisionInfo) {
        print("Now	 in contact with " + collisionInfo.transform.name);
		//if (collisionInfo.contacts[0].normal.y>0.5f) {
			foreach (ContactPoint contact in collisionInfo.contacts) {
    	        CreateYellowSphereAt(contact.point);
			}
		//}
    }
    
	
	
	// Use this for initialization
	void Start () {
		//CreateYellowSphereAt(new Vector3(0.0f,0.0f,0.0f));
	//renderer.material.color = Color.yellow;
	}
	
	// Update is called once per frame
	RaycastHit hit;
	void Update () {
		if(doRaySweep){
			if (GetComponent<Rigidbody>().SweepTest (new Vector3(0.0f,-1.0f,0.0f) , out hit, 25.0f)){
				//foreach (Transform hitTransf in hit ) {
				hit.transform.GetComponent<Renderer>().material.color = Color.green;
				
//				hitTransf.renderer.material.color = Color.green;
				//}
			}	
			doRaySweep=false;
		}
		
	
	}
	
	void CreateYellowSphereAt ( Vector3 coordOfSphere) {
		contactSphere =GameObject.CreatePrimitive(PrimitiveType.Sphere);
	 	Destroy(contactSphere.GetComponent<Collider>());
		contactSphere.transform.position = coordOfSphere;
		contactSphere.transform.localScale= new Vector3(2.0f,2.0f,2.0f);
		contactSphere.GetComponent<Renderer>().material.color = Color.yellow;
	}
	
}
