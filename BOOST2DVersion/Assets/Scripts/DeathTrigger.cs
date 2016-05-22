using UnityEngine;
using System.Collections;

public class DeathTrigger : MonoBehaviour {


	Collider2D colliderThatHit;
	// Use this to tell the object hitting it that it's supposed to die

	/*void OnCollisionEnter2D (Collision2D  colliderThatHit){
		Debug.Log ("The collider that hit is " + colliderThatHit.transform.name);
		colliderThatHit.gameObject.SendMessage("OnDeath",SendMessageOptions.DontRequireReceiver);
		if (colliderThatHit.transform.parent != null){
			colliderThatHit.transform.parent.SendMessage("OnDeath",SendMessageOptions.DontRequireReceiver);
		}
	
	}*/

	void OnTriggerEnter2D (Collider2D  colliderThatHit){
		//Debug.Log ("The collider that hit is " + colliderThatHit.transform.name);
		colliderThatHit.gameObject.SendMessage("OnDeath",SendMessageOptions.DontRequireReceiver);
		if (colliderThatHit.transform.parent != null){
			colliderThatHit.transform.parent.SendMessage("OnDeath",SendMessageOptions.DontRequireReceiver);
		}
		
	}

	void OnDrawGizmos () {
		Gizmos.DrawIcon (transform.position, "SkullIcon.tif");
	}
	
	
	
}
