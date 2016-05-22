using UnityEngine;
using System.Collections;

public class SendKillMessage : MonoBehaviour {


	void OnCollisionEnter2D(Collision2D collisionThatHit){
		collisionThatHit.gameObject.SendMessage("OnDeath",SendMessageOptions.DontRequireReceiver);
		collisionThatHit.transform.parent.SendMessage("OnDeath",SendMessageOptions.DontRequireReceiver);
		//Debug.Log("I just got hit by" + collisionThatHit.gameObject.name);	
	}
	






}
