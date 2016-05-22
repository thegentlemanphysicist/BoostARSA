using UnityEngine;
using System.Collections;

public class LaserGateCollMessage : MonoBehaviour {

	Collider hit;

	void OnTriggerEnter2D (Collider2D hit){
		if (!hit.isTrigger){		
			hit.gameObject.SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
			hit.transform.parent.SendMessage("OnDeath",SendMessageOptions.DontRequireReceiver);
		}
	}
	/*void OnCollisionEnter(Collision collisionThatHit){
		collisionThatHit.gameObject.SendMessage("OnDeath",SendMessageOptions.DontRequireReceiver);
		collisionThatHit.transform.parent.SendMessage("OnDeath",SendMessageOptions.DontRequireReceiver);
		//Debug.Log("I just got hit by" + collisionThatHit.gameObject.name);	
	}*/
}
