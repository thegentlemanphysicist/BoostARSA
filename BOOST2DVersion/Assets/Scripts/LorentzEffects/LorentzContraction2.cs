using UnityEngine;
using System.Collections;

//Include this so I can tell when the crate is grounded
//[RequireComponent (typeof(CharacterController))]


////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
/// <summary>
/// Lorentz contraction.
/// NEEDS:  Player, Physical Constants
/// This script deals with collisions of a PickUp tagged object
/// and the lorentz contraction of it.  Note that it is not as 
/// trivial as simply shrinking the x-scalling.
/// </summary>
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////	


//Old copy saved on apartment desktop scripts folder


 
public class LorentzContraction2 : MonoBehaviour {
	//The time of a collision will be given by this
	float timeOfCollision;
	public float timeSinceCollison;//It ranges from 0 to 1
	public bool collisionInitiated;
	public bool collisionIncomple=false;
	public float XVelocityGoalNew=0.0f;
	//float gammaXNew=1;
	//The speed the object is suposed to be traveling
	public float targetXSpeed = 0.0f;
	
	//We need to initiate a wall collision
	public bool wallCollisionInitiated=false;
	
	
	//float xScale;
	public	float xScale0;
	public float yScale0;
	public float zScale0;
 	public Vector3 VNew;
	public Vector3 VxNew2;
	public float gammaX;
	//float gammaXold;
	Vector3 lastposition;

	//For the x position shift during contraction
	float xScaleOld;
	float deltaXshift;
	
	
	//We need a boolean variable that is true when the object is held
	public bool IsBeingHeld, IsBeingPickedUp;
	float VJennyX;
	
	//PhysicsConstants physicsConstants; 
	//GameObject physConstGameObj;
	float SpeedOfLight;
	
	//This will be the velocity we want the sliding block to reach
	public float XVelocityGoal=0;
	public float XSpeedSmooth;


	
	//the fudge constant ensures the box reaches the desired speed quickly
	float fudgeConstant=100.0f;
	float appliedForce;	
	//The coeff of friction of the box to figure out decceleration
	public float coeffOfFric = 0.05f;

	
	//This is to access the pick up command
	PlatformerController platformerController;
//	PickingUpObjects pickingUpObjects; 
	GameObject player;
	
	
	
	
	
//////////////The speed that info travels through the block
	public float SpeedOfSound;
		
	
	
	
//////////////These are the variables for the function CollisionSwitchStatement	
	public enum CollisionStates{noCollision,collisionInitiated,contractionPhase
		,expansionPhase,finishCollision};
	public CollisionStates collisionStates = CollisionStates.noCollision;
	public float initialColScale, minXScale, collisionSpeed, contractiontime0,
			contractiontime1, contractiontime2;



	

	
//float rate; 
	


	public bool IsStationary;
     
   
	
	
	
	
	
	
	
	// Use this for initialization
	void Start () {
		collisionStates = CollisionStates.noCollision;
		collisionInitiated=false;
		timeSinceCollison=1.0f;
			
		//lastposition=transform.position;
		
		xScale0= transform.localScale.x;
		yScale0= transform.localScale.y;
		zScale0= transform.localScale.z;
		
		//Jenny is never holding at the start of a level
		IsBeingHeld=false;
		IsBeingPickedUp= false;
		//Is the x position fixed at start?
		IsStationary=false;
		//Get the speed of light from the platformer controler
		//as well as Jenny's speed.
		platformerController = PlatformerController.platformerController;
		player = platformerController.gameObject;

		//player = GameObject.Find ("Jenny");		
		//platformerController = player.GetComponent("PlatformerController") as PlatformerController;
		//The script needs to access PickingUpObjects script on Jenny	
		//pickingUpObjects = player.GetComponent("PickingUpObjects") as PickingUpObjects;		
		
		
		//physConstGameObj = GameObject.Find ("PhysicalConstants");		
		//physicsConstants = //	physConstGameObj.GetComponent("PhysicsConstants") as PhysicsConstants;
		SpeedOfLight=GameControl.control.SpeedOfLight;//physicsConstants.SpeedOfLight;
		
		
		SpeedOfSound = SpeedOfLight;

	
	
		//the fudge constant ensures the box reaches the desired speed quickly
		fudgeConstant=100.0f;
		
		//this has to do with collissions
		//rate= SpeedOfLight/xScale0;	
	}
	
	
	
	//When the box collides with something this is triggered.	
	void OnCollisionEnter(Collision hit) {
		//This should only affect the block that is being picked up
		if (IsBeingPickedUp
		    && platformerController.pickUpState != PlatformerController.PickUpStates.pickUpComplete
			&& Mathf.Abs( hit.contacts[0].normal.y) > 0.8) {
			platformerController.pickUpState = PlatformerController.PickUpStates.pickUpInterupted;
			IsBeingPickedUp=false;
		}
		
		
		
		//timeOfCollision=Time.time;	
		//WORRY ABOUT THE DIRECTION OF COLLISION LATER
		//if (hit.collider.transform.CompareTag("InlasticWall")) {
		
		if (IsBeingHeld) {
			if (Mathf.Abs( hit.contacts[0].normal.x) > 0.8){
				if (collisionStates == CollisionStates.noCollision) {
					collisionStates = CollisionStates.collisionInitiated;
				}
				//platformerController.movement.targetSpeed =0.0f;	
			}
		} else {
			if (Mathf.Abs( hit.contacts[0].normal.x) > 0.8){
				Debug.Log("bomb hit something");
				if (collisionStates == CollisionStates.noCollision) {
					collisionStates = CollisionStates.collisionInitiated;
				}
				collisionInitiated = true;
			}
		
		//Transform bodytransform = hit.collider.transform;
		//if ( bodytransform.CompareTag("InlasticWall")) {
		//	wallCollisionInitiated=true;
			//XVelocityGoal=0.0f;
		//	return;
		//} 
		}	
		
		
	}
	
	
	/*
    void OnCollisionExit (Collision collisionInfo)
    {
    	IsGrounded = false;
		Debug.Log("Does this get triggered?");
    }*/
	
	
	
	void Update () {

		
		
		
		
		
		
		
		
		
		
		
		
//////////This helps deal with the expansion of a block when it hits a wall.		
		CollisionSwitchStatement();
		
		
		//The collision stuff is only included when it is not being carried.
		if (!IsBeingHeld){
			
		//this deals with friction slowing the box
		//This should only be imposed if the object is touching the ground
		//if (controller.isGrounded) {
			if (XVelocityGoal!=0) {
				XVelocityGoal = RelVelShiftUnderForce ( -Mathf.Sign(XVelocityGoal)*10.0f , XVelocityGoal );
			}	
		
			//Because of contractions we need to triger the effect using collisionInitiated
			if (collisionInitiated) {
				Debug.Log("Collision Initiated");
				timeSinceCollison=0.0f;
				XVelocityGoal=0.0f;//XVelocityGoalNew;
				//gammaXNew=GetGammaX( Mathf.Abs(XVelocityGoalNew) );
				collisionIncomple  = true;
				collisionInitiated = false;
			}
			
			
			VNew = GetXVelocity ();
			
			//Here I need to apply the force to make the object move at the correct speed
			if (VNew.x != XVelocityGoal){
				//apply forward force
				appliedForce = fudgeConstant*(XVelocityGoal-VNew.x)*GetComponent<Rigidbody>().mass/SpeedOfLight;
				GetComponent<Rigidbody>().AddForce(new Vector3(appliedForce,0.0f,0.0f));
			}
		
			//This contracts the moving object.
			//Here I have entered V goal.
			gammaX = GetGammaX ( Mathf.Abs(XVelocityGoal) );
			
			/*	
			if (timeSinceCollison<1.0 & collisionIncomple ) {
				timeSinceCollison+=Time.deltaTime*rate;
				//float timeOfContraction = Time.time;
				xScaleOld = transform.localScale.x;
				float xScaleNew=Mathf.Lerp(xScale0, xScale0/gammaXNew,timeSinceCollison);
				//LeftWallShift ( -xScaleNew+xScaleOld );
				transform.localScale = new Vector3 ( xScaleNew , yScale0, zScale0 );
				deltaXshift = -(transform.localScale.x-xScaleOld)*Mathf.Sign(XVelocityGoalNew)/2; 
				transform.position += new Vector3( deltaXshift, 0.0f , 0.0f );
			} else if (timeSinceCollison>1.0 & collisionIncomple){
				//the box may start sliding when the collision is complete.
				XVelocityGoal=XVelocityGoalNew;
				transform.localScale = new Vector3 ( xScale0/gammaX , yScale0, zScale0 );
				collisionIncomple=false;
			} else {
				transform.localScale = new Vector3 ( xScale0/gammaX , yScale0, zScale0 );
			} 
			*/
		
		
		} else {
			if (collisionStates == CollisionStates.noCollision){
				//This else statement is for the box when it's being held it should
				//contract the same as Jenny
				//The smoothed speed Jenny is moving at
				XSpeedSmooth=platformerController.movement.speed;
				//convert the smooth speed to a gamma factor
				//gammaXold=gammaX;
				gammaX = GetGammaX(XSpeedSmooth);  
				//contract the box to the correct length
				transform.localScale = new Vector3 ( xScale0/gammaX , yScale0, zScale0 );
				//The box is shifted to the correct position in the update loop of the pick up function.
			}
		}
		
			
		
	}

	
	
	
	
	
	
	
	
	
	
////////////This funtion deals with the contraction on collidion
	void CollisionSwitchStatement(){
		switch (collisionStates) {
		case (CollisionStates.noCollision):
			break;
		case (CollisionStates.collisionInitiated):
			//The amount of contraction depends on the Xscale of the the object 
			initialColScale = transform.localScale.x;
			if(IsBeingHeld){
				//Remove control during this process
				platformerController.canControl = false;
				collisionSpeed = platformerController.movement.targetSpeed;
			} else {
				collisionSpeed = XVelocityGoal;
			}	
			//The minimum size of box
			minXScale = transform.localScale.x*SpeedOfSound
						/( SpeedOfSound + collisionSpeed);
			contractiontime0=Time.time;
			//The time it takes to minimize the box
			contractiontime1 = transform.localScale.x/
				(SpeedOfSound + collisionSpeed);
			//The time it takes the box to expand from minimum
			contractiontime2 = (xScale0 - minXScale)/SpeedOfSound;
			collisionStates = CollisionStates.contractionPhase;
			goto case CollisionStates.contractionPhase;
		case (CollisionStates.contractionPhase):
			//Loop over the time until the contraction is done.
			if (transform.localScale.x > minXScale){
				transform.localScale=new Vector3(
					Mathf.Lerp(initialColScale, minXScale,
					(Time.time - contractiontime0 )/contractiontime1 )
					,transform.localScale.y, transform.localScale.z);
				break;
			} else {
			//Reset the zero time for the next loop.
				contractiontime0 = Time.time;
				collisionStates = CollisionStates.expansionPhase;
				goto case CollisionStates.expansionPhase;
			}
			
			
		case (CollisionStates.expansionPhase):
			//Loop over the time until the expansion is done.
			if (transform.localScale.x < xScale0){
				transform.localScale=new Vector3(
					Mathf.Lerp( minXScale, xScale0,
					(Time.time - contractiontime0)/contractiontime2 )
					,transform.localScale.y, transform.localScale.z);
				break;
			} else {
				platformerController.movement.targetSpeed=0.0f;
				collisionStates = CollisionStates.finishCollision;
				goto case CollisionStates.finishCollision;
			}
		
		case (CollisionStates.finishCollision):
			if(IsBeingHeld){
				platformerController.canControl = true;
			}
			collisionStates = CollisionStates.noCollision;
			break;	
		}
			
	}
	
	
	
	
	
	
//////////////////////////////////////////////////////	
//////////////////////////////////////////////////////	
//////////////////////////////////////////////////////	
//////These are functions we will use in the script
//////
//////	
//////////////////////////////////////////////////////	
//////////////////////////////////////////////////////	
//////////////////////////////////////////////////////	
	

	
	
	
	float GetXVelocityJenny () {
		float Vx = platformerController.GetSpeed();
		//float Vx = rigidbody.velocity.x;
		return Vx;// Vx is the float returned by GetXVelocity
	}
	
	
	Vector3 GetXVelocity () {
		Vector3 V = GetComponent<Rigidbody>().velocity;
		return V;// Vx is the float returned by GetXVelocity
	}
	
	
	
	//Gammax is the gamma factor of the object this script is applied to
	float GetGammaX ( float Vcurrent ) {
		float gammaX = 1/ Mathf.Sqrt( 1.0f - Mathf.Pow( Vcurrent / SpeedOfLight, 2 )  );
		return gammaX;
	}
	
	
	//Now we need a function that deals with the decelaration of an object under a constant force
	// use a=F/m
	/////////////////////////////////////////////////////////////
	////////MAKE SURE THIS IS ONLY CALLED IN FIXED UPDATE////////
	/////////////////////////////////////////////////////////////
	float RelVelShiftUnderForce ( float inducedAccel , float initialVel ) {
		float newVelocity = (inducedAccel*Time.deltaTime+GetGammaX(initialVel)*initialVel)
			/Mathf.Sqrt(1+Mathf.Pow( (inducedAccel*Time.deltaTime+GetGammaX(initialVel)*initialVel)/SpeedOfLight  ,2)  );
		return newVelocity;
	}
	

	
	
	

}




	
	
	
	
	
	
	
	

	
	
	
	
	
