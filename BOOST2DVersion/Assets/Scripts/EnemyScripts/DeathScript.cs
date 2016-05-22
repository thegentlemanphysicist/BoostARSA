using UnityEngine;
using System.Collections;

public class DeathScript : MonoBehaviour {
	/// <summary>
	/// This should control when an enemy kills something and when it can kill
	/// </summary>
	
	
	//This is how stron a given  bad guy is.
	public float maxPulseWavelength;
	public string nameOfShooter;
	
	
	void OnCollisionEnter2D(Collision2D collisionThatHit){
		collisionThatHit.gameObject.SendMessage("OnDeath",SendMessageOptions.DontRequireReceiver);
		collisionThatHit.transform.parent.SendMessage("OnDeath",SendMessageOptions.DontRequireReceiver);
		//Debug.Log("I just got hit by" + collisionThatHit.gameObject.name);	
	}
	
	
	void OnHitByPulse(float pulseWavelength){
		Debug.Log("Got shot");
		Debug.Log("The pulse wavelength is ="+ pulseWavelength);
		Debug.Log("The wavelength is ="+ maxPulseWavelength);
		if (pulseWavelength < maxPulseWavelength  ){
			Destroy(gameObject.transform.parent.gameObject);	
		}
		
	}
	
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
