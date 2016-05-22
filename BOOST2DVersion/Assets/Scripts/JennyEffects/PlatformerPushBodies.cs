// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class PlatformerPushBodies : MonoBehaviour {
	// Script added to a player for it to be able to push rigidbodies around.

	// How hard the player can push
	//	float pushPower= 0.5f;

	// Which layers the player can push
	// This is useful to make unpushable rigidbodies
	//LayerMask pushLayers = -1;


	public static PlatformerPushBodies platformerPushBodies;
	float SpeedOfLight;
	// pointer to the player so we can get values from it quickly
	private PlatformerController controller;
	LorentzContractionForJenny lorentzContractionForJenny;
	public Vector3 TheNormal;
	public AudioClip impactSound;
	AudioSource impactAudio;


	public float timeOfBounce;
	float delayBounce = 0.2f;

	void Awake () 
	{
		//makes it a singleton
		if (platformerPushBodies == null)
		{
			platformerPushBodies = this;
		} else if (platformerPushBodies != this)
		{
			Destroy(gameObject);
		}
		
	}


	void  Start (){
		controller = PlatformerController.platformerController;
		lorentzContractionForJenny = LorentzContractionForJenny.lCJenny;


		SpeedOfLight = GameControl.control.SpeedOfLight;

		impactAudio = gameObject.AddComponent<AudioSource>();
		impactAudio.clip = impactSound;
		impactAudio.volume = 0.2f;
		impactAudio.playOnAwake = false;
		timeOfBounce = 0f;
	}




	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag != "BouncyWall" && collision.gameObject.tag != "Trigger" ){



			//don't play a sound unless there's actual speed behind the collision
			if (   -collision.contacts[0].normal.x*controller.movement.direction.x > 0.2f  &&
			    Mathf.Abs(collision.relativeVelocity.x) > 0.3*SpeedOfLight) 
			{
				//play collision sound
				impactAudio.Play();
				//give the camera a kick
				CameraTracking.cameraTracking.WallCollisionKick();
			}
	
		}
		if ( Mathf.Abs(collision.contacts[0].normal.y) >0.3f && !collision.transform.CompareTag("PickUp") &&
		    Mathf.Abs(collision.relativeVelocity.y) > 0.3*SpeedOfLight)
		{
			CameraTracking.cameraTracking.HitFloorJerk(collision.relativeVelocity.y
				//transform.rigidbody2D.velocity.y
			                                           ,collision.contacts[0].normal.y);
				
		}

	}
	
	
	
	
	void OnCollisionStay2D(Collision2D collision) {
		
		//Debug.Log("I was hit by" + collision.transform.name);
		
		//Transform bodytransform = hit.collider.transform;
	
		TheNormal = collision.contacts[0].normal;
		if (collision.transform.tag !="BouncyWall")
		{
			if (   -collision.contacts[0].normal.x*controller.movement.direction.x > 0.2f    ) 
			{

				controller.movement.targetSpeed=0.0f;
				controller.movement.gammaX	   =1.0f;

				return;
			}


			/*if (   collision.contacts[0].normal.y <0.9f && collision.transform.rigidbody2D != null){
				Debug.Log ("Push Box Up");
				collision.transform.rigidbody2D.AddForce( 
				         new Vector2(0.0f, 10.0f*collision.transform.rigidbody2D.gravityScale*collision.transform.rigidbody2D.mass));
				//, ForceMode2D.Impulse);
			}*/

		} else {

			//make sure jenny's facing the collider
			if (   -collision.contacts[0].normal.x*controller.movement.direction.x > 0.8f   
			    && Time.time > timeOfBounce+delayBounce) 
			{
				//This is to keep multiple bounces
				timeOfBounce = Time.time;


				collision.transform.SendMessage("BounceStarted", collision.contacts[0].normal.x,SendMessageOptions.DontRequireReceiver);
				controller.movement.direction = new Vector3( -controller.movement.direction.x,0f,0f);
				//This redraws the DopplerCircle after a change in direction
				lorentzContractionForJenny.gammaXOld2 = 0f;
			}
		}


		
	}






//	void OnCollisionExit(Collision collision){
		
//	}
	
	
	/*
	/////
	/////This will need to be commented out when I'm done with this
	/////
	void  OnControllerColliderHit ( ControllerColliderHit hit  ){
	//If Jenny hits an innelastic wall she stops
			
		Transform bodytransform = hit.collider.transform;
		if ( bodytransform.CompareTag("InlasticWall")) {
			controller.movement.targetSpeed=0.0f;
			return;
		} 
		
		
			
		
		
		Rigidbody body = hit.collider.attachedRigidbody;
				
		// no rigidbody
		if (body == null || body.isKinematic)
			return;

		// Only push rigidbodies in the right layers
		int bodyLayerMask= 1 << body.gameObject.layer;
		if ((bodyLayerMask & pushLayers.value) == 0)
			return;
		
		// We dont want to push objects below us
		if (hit.moveDirection.y < -0.3f) {
			return;
		}

			
		// Calculate push direction from move direction, we only push objects to the sides
		// never up and down
		//Vector3 pushDir= new Vector3 (hit.moveDirection.x, 0, hit.moveDirection.z);
	
		
		////////////////////////////////////////
		///Here I use conservation of momentum to calculate the final velocity 
		///of innelastic collisions		
		////////////////////////////////////////	
		if (   body.CompareTag("PickUp")   ) {		
			controller.movement.targetSpeed=0.0f;
			return;
		}
	}
		*/	


	
	
	
	
	
	
	
}