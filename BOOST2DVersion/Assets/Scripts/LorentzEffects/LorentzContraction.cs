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


 
public class LorentzContraction : MonoBehaviour {


	public float slowingFriction;
	//The time of a collision will be given by this
	float timeOfCollision;
	//public float timeSinceCollison;//It ranges from 0 to 1
	//public bool collisionInitiated;
	public bool collisionIncomple=false;
	float collisionXNormal=0.0f;
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
 	public float VxActual;
	public Vector3 VxNew2;
	public float gammaX, gammaXOld;
	

	//For the x position shift during contraction
	float xScaleOld;
	
	
	
	//We need a boolean variable that is true when the object is held
	public bool IsBeingHeld, IsBeingPickedUp, canBePickedUp;
	
	
	//PhysicsConstants physicsConstants; 
	//GameObject physConstGameObj;
	float SpeedOfLight;
	
	//This will be the velocity we want the sliding block to reach
	public float XVelocityGoal=0.0f;
	float   XVelocityGoalOld=0.0f;
	public bool xMotionStatic = false;


	
	
	//the fudge constant ensures the box reaches the desired speed quickly
	float fudgeConstant=100.0f;
	float appliedForce;	
	//The coeff of friction of the box to figure out decceleration
	public float coeffOfFric = 0.05f;

	
	//This is to access the pick up command
	PlatformerController platformerController;
//	PickingUpObjects pickingUpObjects; 
	GameObject player;
	
	
	
	//the tag of the object collided with.
	public string tagOfhit;


//////////////The speed that info travels through the block
	public float SpeedOfSound;
		
	


	
//////////////These are the variables for the function CollisionSwitchStatement	
	public enum CollisionStates{noCollision,collisionInitiated,contractionPhase
		,expansionPhase,finishCollision};
	public CollisionStates collisionStates = CollisionStates.noCollision;
	public float initialColScale, minXScale, collisionSpeed, contractiontime0,
			contractiontime1, contractiontime2;



	

	

	


	public bool IsStationary;
     
   
	
	
	
	
	
	
	
	// Use this for initialization
	void Start () {

		slowingFriction = 25f;


		collisionStates = CollisionStates.noCollision;
		//collisionInitiated=false;
		//timeSinceCollison=1.0f;
			
		//lastposition=transform.position;
		
		xScale0= transform.localScale.x;
		yScale0= transform.localScale.y;
		zScale0= transform.localScale.z;
		
		//Jenny is never holding at the start of a level
		IsBeingHeld=false;
		IsBeingPickedUp= false;
		//Is the x position fixed at start?
		IsStationary=false;

			
		platformerController = PlatformerController.platformerController;
		player = platformerController.gameObject;

		SpeedOfLight= GameControl.control.SpeedOfLight;
		
		
		SpeedOfSound = SpeedOfLight;
	
		//the fudge constant ensures the box reaches the desired speed quickly
		fudgeConstant=100.0f;
		
		
		
		gammaX    = 1.0f;
		gammaXOld = 1.0f;
		//set the gravity to gamecontrol gravity
		transform.GetComponent<Rigidbody2D>().gravityScale = GameControl.control.gravity;
	}




	void OnCollisionStay2D(Collision2D hit3)
	{
		if (IsBeingPickedUp &&
		    platformerController.pickUpState != PlatformerController.PickUpStates.pickUpComplete )
		{

			foreach (ContactPoint2D contact in hit3.contacts) 
			{
				if ( hit3.contacts[0].normal.y < -0.8f  )
				{
					platformerController.pickUpState = PlatformerController.PickUpStates.pickUpInterupted;
					IsBeingPickedUp=false;
				} else if ( !hit3.transform.CompareTag("Player")&&
					hit3.contacts[0].normal.x*Mathf.Sign(platformerController.movement.direction.x) >0.1f )
				{
					platformerController.pickUpState = PlatformerController.PickUpStates.pickUpInterupted;
					IsBeingPickedUp=false;
				}
			}
		}


		tagOfhit = hit3.transform.tag;
		collisionXNormal=hit3.contacts[0].normal.x;
		//this was commented out for some reason
		if (IsBeingHeld) {
			// has to be the platformer controler direction that controls this
			if (Mathf.Abs(collisionXNormal) > 0.8f 
			    && Mathf.Sign(collisionXNormal*platformerController.movement.direction.x)==-1){

				if (platformerController.movement.targetSpeed  > 1f
				    && collisionStates == CollisionStates.noCollision) {
					collisionStates = CollisionStates.collisionInitiated;
				} 

			}

			if (tagOfhit == "BouncyWall"  && 
			    Time.time < PlatformerPushBodies.platformerPushBodies.timeOfBounce+ 0.2f) {
				PlatformerPushBodies.platformerPushBodies.timeOfBounce = Time.time;
				//tell wall there's a bounce
				hit3.transform.SendMessage("BounceStarted",collisionXNormal , SendMessageOptions.DontRequireReceiver);
			}

		} else {


			if (Mathf.Abs(collisionXNormal) > 0.2f && Mathf.Sign(collisionXNormal*XVelocityGoal)==-1  ){
				if (hit3.transform.tag != "BouncyWall" )//remove bouncy wall, let it be controlled in the collision statement
				{
					if (collisionStates == CollisionStates.noCollision) {
						collisionStates = CollisionStates.collisionInitiated;
					}
				} else {//if it hits a bouncy wall  
					//Debug.Log("I hit a bouncy wall");
					hit3.transform.SendMessage("BounceStarted",collisionXNormal , SendMessageOptions.DontRequireReceiver);
					XVelocityGoal = -XVelocityGoal;
				}
			}
			
		}	
	

	
	
	
	
	
	}






	
	
	
	//try this with fixed update
	void FixedUpdate () {
		
	//What does this need to do?
		//1)control the sliding speed of the block
		//2)Lorentz contract the cube for 3 cases 
		//	"carried", "notcarried", "notcarried & stationary"
		//3)Deal with collisions (contraction then expantion phase)
		
		
		//Split the two cases right away.
		if (IsBeingHeld) {
			//1)THE SPEED OF THE BLOCK IS CONTROLED BY JENNY WHEN CARRIED.
			
			//2)THE LORENTZ CONTRACTION OF A CARRIED OBJECT IS PASSED THROUGH THE CONTROLER
					
			//3)Deal With The Collision			
			CollisionSwitchStatementHeld();


			
		} else {//not held
		
			//1)CONTROL THE SPEED OF THE BLOCK	
	
			//Friction slows the box when it's released
			if (Mathf.Abs (XVelocityGoal) >= 0.1f) 
			{
				if (xMotionStatic) 
				{
			
					xMotionStatic = false;

				}

				XVelocityGoal = RelVelShiftUnderForce ( -Mathf.Sign(XVelocityGoal)*slowingFriction , XVelocityGoal );
			} else if (!IsBeingPickedUp) 
			{
				if (!xMotionStatic && (Mathf.Abs(xScale0 - transform.localScale.x) < 0.02f) )
				{//this should only be called once to change it
				
					xMotionStatic = true;
				}
			}

			//The actual speed in the x direction
			VxActual = GetComponent<Rigidbody2D>().velocity.x;			
			//Here I need to apply the force to make the object move at the correct speed
			if (VxActual != XVelocityGoal){
				//apply forward force
				appliedForce = fudgeConstant*GetComponent<Rigidbody2D>().mass*(XVelocityGoal-VxActual)*GetComponent<Rigidbody2D>().mass/SpeedOfLight;
				GetComponent<Rigidbody2D>().AddForce(new Vector2(appliedForce,0.0f));
			}
			
			//2)CONTRACT THE BLOCK CORRECTLY
			//This loop controls how the transform shrinks and grows.	
			CollisionSwitchStatementNotHeld();
		

			
		}
		
		
		
			
		
	}




	
	
	
	
	
	
////////////This funtion deals with the contraction on collidion
	void CollisionSwitchStatementHeld(){
		switch (collisionStates) {
		case (CollisionStates.noCollision):
			
			//gammaX is assigned in platformer control ONLY CONTRACT IF GAMMA CHANGES
			if (gammaX!=gammaXOld){
				//contract the box to the correct length
				transform.localScale = new Vector3 ( xScale0/gammaX , yScale0, zScale0 );
				gammaXOld = gammaX;
			}
			//The box is shifted to the correct position in the update loop of the pick up function.
		
			
			break;
		case (CollisionStates.collisionInitiated):
			//The amount of contraction depends on the Xscale of the the object 
			initialColScale = transform.localScale.x;
			
			//Remove control during this process
			platformerController.canControl = false;
			collisionSpeed = platformerController.movement.targetSpeed;
		
			//The minimum size of box
			//minXScale = transform.localScale.x*SpeedOfSound
			//			/( SpeedOfSound + collisionSpeed);
			//TO REMOVER THE CONTRACTION PHASE, SET MINXSCALE EQUAL TO THE TRANSFORM SCALE
			minXScale = transform.localScale.x;



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
			if ( tagOfhit != "BouncyWall")
			{
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
			} else 
			{

				//this will expand back out to the incomming velocity
				if (transform.localScale.x < initialColScale){
					transform.localScale=new Vector3(
						Mathf.Lerp(minXScale, initialColScale,
					           (Time.time - contractiontime0 )/contractiontime1 )
						,transform.localScale.y, transform.localScale.z);

					break;
				} else {
					//platformerController.movement.targetSpeed=collisionSpeed;// this will be the initial incoming velocity

					platformerController.movement.direction = new Vector3(
						-platformerController.movement.direction.x, 0f, 0f);


					collisionStates = CollisionStates.finishCollision;
					goto case CollisionStates.finishCollision;
				}


				//put in the case for the bouncy wall
				//break;
			}



		case (CollisionStates.finishCollision):
			if(IsBeingHeld){
				platformerController.canControl = true;
			}
			collisionStates = CollisionStates.noCollision;
			break;	
		}
			
	}
	
	
	void CollisionSwitchStatementNotHeld(){
		switch (collisionStates) {
		case (CollisionStates.noCollision):
			if(XVelocityGoal!=XVelocityGoalOld){
				gammaX= GetGammaX(XVelocityGoal);
				transform.localScale = new Vector3 ( xScale0/gammaX , yScale0, zScale0 );
				XVelocityGoalOld=XVelocityGoal;
				
			}
			break;
		case (CollisionStates.collisionInitiated):
			//Debug.Log("The collision was initiated");
			//timeSinceCollison = 0.0f;
			//Set the goal speed equal to 0 when a collision is initiated
			XVelocityGoal     = 0.0f;//HAVING THIS ZERO IS WHAT'S KEEPING THE BOX FROM HAVING A CONTRACTION PHASE
			collisionIncomple  = true;
						
			//The amount of contraction depends on the Xscale of the the object 
			initialColScale = transform.localScale.x;
			
			collisionSpeed = XVelocityGoal;

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

			//Debug.Log("Got here!!! Min x Scale is="+minXScale);
			//Debug.Log("The actual scale is="+transform.localScale.x);

			//Loop over the time until the contraction is done.
			if (transform.localScale.x > minXScale){
				Debug.Log ("The contraction phase was called");
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
			//put an if statement in here for bouncy or not
			if ( tagOfhit != "BouncyWall")
			{

				if (transform.localScale.x < xScale0){
					transform.localScale=new Vector3(
						Mathf.Lerp( minXScale, xScale0,
						(Time.time - contractiontime0)/contractiontime2 )
						,transform.localScale.y, transform.localScale.z);
					break;
				} else {
					//platformerController.movement.targetSpeed=0.0f;
					collisionStates = CollisionStates.finishCollision;
					goto case CollisionStates.finishCollision;
				}
			} else 
			{
				if (transform.localScale.x < initialColScale){
					transform.localScale=new Vector3(
						Mathf.Lerp( minXScale, initialColScale,
					           (Time.time - contractiontime0)/contractiontime1 )
						,transform.localScale.y, transform.localScale.z);
					break;
				} else {
					//platformerController.movement.targetSpeed=0.0f;
					collisionStates = CollisionStates.finishCollision;
					goto case CollisionStates.finishCollision;
				}
			}


		
		case (CollisionStates.finishCollision):
			
			collisionStates = CollisionStates.noCollision;
			break;	
		}
			
	}







	
	
	
	float GetXVelocityJenny () {
		float Vx = platformerController.GetSpeed();
		//float Vx = rigidbody.velocity.x;
		return Vx;// Vx is the float returned by GetXVelocity
	}
	
	
	float GetXVelocity () {
		float Vx = GetComponent<Rigidbody2D>().velocity.x;
		return Vx;// Vx is the float returned by GetXVelocity
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












	
	/*
			if (timeSinceCollison<1.0 & collisionIncomple ) {
				
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
			}// */	
	
	
	
	
	
	

	
	
	
	
	
