using UnityEngine;
using System.Collections;

public class ExplosionOfBomb : MonoBehaviour {
	
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
/// <summary>
/// This deals with the explosiong of objects
/// NEEDS:  Player, Physical Constants
/// This script deals with collisions of a PickUp tagged object
/// and the lorentz contraction of it.  Note that it is not as 
/// trivial as simply shrinking the x-scalling.
/// </summary>
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////	
	
	//float SpeedOfLight;
	
	//The physics constants give us the speed of light in one place
	//PhysicsConstants physicsConstants; 
	//GameObject physConst;
	
	//This is the lorentz contraction script on the bomb
	LorentzContraction lorentzContraction;
	
	//This is to access the pick up command
	PlatformerController platformerController;
//	PickingUpObjects pickingUpObjects; 
	GameObject player;
//	need to deactivate the timer if bomb detonated prematurely
	Transform timer;



	

	float initialRadius;
	public float finalRadius=5.7f;
	


	//A list of the various states in the explosion process
	public enum ExplosionStates
	{
		noDetonation,
		explosionInitiated,
		explosionEnding
	};
	public ExplosionStates explosionState = ExplosionStates.noDetonation;

	BombExplosionSpawner bombExplosionSpawner;


	
	
	
	
	// Use this for initialization
	void Start () {
	    //Get the speed of light from the platformer controler
		//as well as Jenny's speed.
		player = GameObject.Find ("Jenny");		
		platformerController = player.GetComponent("PlatformerController") as PlatformerController;
		//The script needs to access PickingUpObjects script on Jenny	
		//pickingUpObjects = player.GetComponent("PickingUpObjects") as PickingUpObjects;		
		
		timer =   transform.parent.FindChild("Timer") as Transform;


		//physConst = GameObject.Find ("PhysicalConstants");		
		//physicsConstants = physConst.GetComponent("PhysicsConstants") as PhysicsConstants;
		//SpeedOfLight=physicsConstants.SpeedOfLight;
		
		lorentzContraction = transform.GetComponent ("LorentzContraction") 
						 as LorentzContraction;
		
	//	explosionInitiated = false;
	
		
	
		bombExplosionSpawner = transform.parent.GetComponent ("BombExplosionSpawner") as BombExplosionSpawner;

	}
	

	void OnDeath()
	{
		explosionState = ExplosionStates.explosionInitiated;
	}


	
	void OnHitByBomb() 
	{
		explosionState = ExplosionStates.explosionInitiated;

	}




	// Update is called once per frame
	void Update () {

		ExplosionSwitchStatement () ;
	
	
	}
	
	
	
	
	
	
	//step1: Destoy or deactivate bomb renderer and collider
	//step2: Activate an ellipsoid the explosion
	//step3: Cause the explosion to expand
	//step4: Have the explosion hover at max
	//step5: fade away
	//step6: destroy the cube game object!
	void ExplosionSwitchStatement () {
	
		switch (explosionState) {
			
		case(ExplosionStates.noDetonation):
			break;
		
		case(ExplosionStates.explosionInitiated):

			if (timer !=null)
			{//this destroys the timer if the bomb is triggered prematurely
				timer.gameObject.SetActive(false);
			}
			//Disable the bomb rendering
			gameObject.GetComponent<Renderer>().enabled=false;
		    gameObject.GetComponent<Collider2D>().enabled=false;
			transform.GetComponent<Rigidbody2D>().gravityScale=0.0f;
			foreach (Transform r in transform) {
    			if (r.name != "Explosion") {
					r.GetComponent<Renderer>().enabled = false;
    			}
			}
						
			//Activate the explosion
			//transform.FindChild("Explosion").gameObject.SetActive(true);
			//transform.FindChild("ExplosionMessage").gameObject.SetActive(true);
			





			bombExplosionSpawner.bombPosition = transform.position;

			if (!lorentzContraction.IsBeingHeld)
			{
				//Debug.Log ("The x velocity goal is ="+lorentzContraction.XVelocityGoal);
				bombExplosionSpawner.velOfBomb = new Vector2(lorentzContraction.XVelocityGoal
				                                             ,transform.GetComponent<Rigidbody2D>().velocity.y);
			} else 
			{
				bombExplosionSpawner.velOfBomb = new Vector2(platformerController.movement.speed*Mathf.Sign(platformerController.movement.direction.x)
				                                             ,transform.GetComponent<Rigidbody2D>().velocity.y);
				//Debug.Log ("The x velocity goal is ="+platformerController.movement.speed);
			}




			//Let go of the bomb
			if(lorentzContraction.IsBeingHeld){
				//the object is no longer being held
				platformerController.holdingSomething = false;
				lorentzContraction.IsBeingHeld = false;
			}




			bombExplosionSpawner.goBoom = true;
			explosionState= ExplosionStates.explosionEnding;

			goto case ExplosionStates.explosionEnding;
		
		case(ExplosionStates.explosionEnding):
			//Send a message to potential parent that the bomb has been destroyed and we 
			//may need a new one spawned
			if (transform.parent.transform.parent!= null)
			{
				transform.parent.transform.parent.
					SendMessage("BombGoBoom",SendMessageOptions.DontRequireReceiver);
			}
			//Destroy the bomb
			Destroy(gameObject);

			//Destroy the bomb parent
			//Destroy(transform.parent.gameObject);

			break;
		}
		
	}
	
	
	
	
	
	
}
