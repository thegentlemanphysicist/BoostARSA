// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;
//using Vectrosity;


///
///THESE COMPONENTS WERE MOVED UP FROM THE BOTTOM OF THE CODE, I NOW GET MANY ERRORS 
///
// Require a character controller to be attached to the same game object
//[RequireComponent (typeof(CharacterController))]
//[AddComponentMenu ("2D Platformer/Platformer Controller")]



public class PlatformerController : MonoBehaviour {


	public static PlatformerController platformerController;

	// This controls whether or not boosts are continuous or discrete.
	public bool continuousBoost = true;


	// Does this script currently respond to Input?
	public bool canControl= true;
	
	// The character will spawn at spawnPoint's position when needed.  This could be changed via a script at runtime to implement, e.g. waypoints/savepoints.
	public Transform spawnPoint;
	


	//audio stuff
	public AudioClip boostSound, breakingSound, jumpSound;
	AudioSource boostAudio, slowingAudio, jumpAudio;






	

	float SpeedOfLight;//this is held in GAMECONTROL	
	public float SpeedOfLightY;





	//for now the velocity of the jump will be the speed of light, may play with it later
	float SpeedOfJump;
	int MaxNumbOfJumps;//Define the maximum number of jumps 
	//This will get the distance from Jenny to the ground so I can initiate jumping correctly
	public float distToGround;
	//THESE SHOULD BE IN THE JUMP CLASS



	//These control how Jenny reaches the desired speed
	float appliedForce = 0.0f; 
	float fudgeConstant =100.0f; 
	public float XVelocityGoal =0.0f;
	//THESE SHOULD BE IN THE MOVEMENT CLASS

		
	
	Transform jennyTransform;
	
	
	

	///////////PICK UP RELATED VARIABLES//////////// 
	bool 		 automatedPickUpComplete;
	float 		 initialXposition;//initial height and position of the picked up object
	public float riseTime,riseSpeed,slideOverSpeed, tempLiftSpeed;//riseTime is how long it takes to pick up the object
	float 		 liftStartTime, maxLiftDuration=4f;//this will interupt the pick up state if it gets stuck
	float 		 goalLiftPosY, goalLiftPosX;//This is the goal height difference between Jenny and picked up object
	public float goalHeight;//this should get assigned in the pickUpSwitchStatement
	//float 		 initialHeightOfCarried;//the y coord when pick up starts.
	public bool  holdingSomething;//is Jenny holding something
	Vector3 	 pickUpRayStart; // This vector will tell us from where to check for a pickuppablebox
	//DeltaXShift is the initial separation of Jenny and item held.
	//it will be used to correct the spacing after lorentz contraction
	//float DeltaXShift;
	//A list of the various states in the pick up process
	public enum PickUpStates
	{
		notHolding,
		checkBoxPresence,
		automateLift,
		attachJoint,
		pickUpInterupted,
		pickUpComplete,
		puttingDown,
		objectDestroyed
	};
	public PickUpStates pickUpState = PickUpStates.notHolding;

	Transform itemCurrentlyHeld;
	LorentzContraction lorentzContraction;//The lorentz contraction script of held item

	//Holding joint is how Jenny carries objects

	SliderJoint2D holdingJoint;
	public RaycastHit2D hit, hit2;
	int layerMaskOfHit;
	public Vector3 directionOfCharacter;
	float speedOfChar;//used to make sure Jenny is stopped before picking something up.
	float massOfObjInitial, massOfObjCarried=0.5f;// the object's mass changes so Jenny can't bump it around when it's sliding






	//////LORENTZ CONTRACTION AND TIME DILATION FOR JENNY/////// 
	LorentzContractionForJenny lorentzContractionForJenny;
	float gammaX, gammaX0ld;
	public float jennyTime = 0f; //Jenny's clock
	float DeltaGamma;//The size of the change in gamma upon keystroke is
	float timeBetweenBoosts, oneOverTimeBetweenBoosts, timeOfBoost, ratioOfTimes;
	bool  boostComplete    = false;
	//LET'S ADD A COUPLE OF OPTIONS TO FOR THE CONTINUOUS BOOSTING
	float smallDeltaGamma = 0.05f, smallTimeBetweenBoosts = 0.05f, small1OverTimeBetweenBoosts;


	//Jenny Shoot
	JennyShoot jennyShoot;


	//////ACCESSES THE LEVEL DISPLAY TO SEND PLAYER DEAD MESSAGE AS WELL AS GET THE GAMMA AND % SPEED/////// 
	LevelDisplay levelDisplay;
	public GameObject boostDial;///the dials that show recovery of pulse and boost

	
	
	/// USED TO CHECK IF JENNY'S GROUNDED IN ISGROUNDED
	public RaycastHit2D[] ground;
	

	

	
	
	
	
	
	
		
		
	//A start function
	
	void  Start (){

		SpeedOfLight  = GameControl.control.SpeedOfLight;
		SpeedOfLightY = GameControl.control.SpeedOfLightY;
		SpeedOfJump   = GameControl.control.jumpSpeed;
		jennyTransform.GetComponent<Rigidbody2D>().gravityScale = GameControl.control.gravity;
		MaxNumbOfJumps= GameControl.control.maxNumberOfJumps;
		movement.maxFallSpeed= SpeedOfLightY;
		// get the distance to ground
	  	distToGround = jennyTransform.GetComponent<Collider2D>().bounds.extents.y;	
		DeltaGamma = GameControl.control.deltaGamma;
		
		
		///////////
		////These are used for pick up
		//////////

		holdingSomething = false;
		//DeltaXShift=1.0f;
		gammaX=1.0f;
		gammaX0ld=1.0f;
		
		riseTime = 0.5f;
		lorentzContractionForJenny =gameObject.GetComponent("LorentzContractionForJenny") 
			as LorentzContractionForJenny;	

		jennyShoot = gameObject.GetComponent("JennyShoot") as JennyShoot;




		levelDisplay=GameObject.FindGameObjectWithTag ("Canvas").GetComponent("LevelDisplay") as LevelDisplay;




		
		jennyTime = 0.0f;
		
		//These are needed for the delay period between boosts
		timeBetweenBoosts = 0.5f;
		oneOverTimeBetweenBoosts= 1.0f/timeBetweenBoosts;
		small1OverTimeBetweenBoosts = 1.0f/smallTimeBetweenBoosts;
		timeOfBoost=0.0f;
		ratioOfTimes=0.0f;

		//boostDial= GameObject.Find("GreenCircle");
		boostDial= GameObject.Find("BoostRecovery");



	


	}
	
	

	
	
	
	
	[System.Serializable]
	public class PlatformerControllerMovement {
	
		
	
		
	
	
		public float inAirControlAcceleration= 1.0f;
	

		public float maxFallSpeed;
	
		// How fast does the character change speeds?  Higher is faster.
		public float speedSmoothing= 5.0f;
		//I will need to sort out what to do with this later
		// This controls how fast the graphics of the character "turn around" when the player turns around using the controls.
		public float rotationSmoothing= 10.0f;
	
		// The current move direction in x-y.  This will always been (1,0,0) or (-1,0,0)
		// The next line, @System.NonSerialized , tells Unity to not serialize the variable 
		//	or show it in the inspector view.  Very handy for organization!
		//@System.NonSerialized  THIS IS NOT VALID IN .CS
		public Vector3 direction= new Vector3 (1,0,0);
	
		// The current vertical speed
		
		public float verticalSpeed= 0.0f;
	
		// The current movement speed.  This gets smoothed by speedSmoothing.
		//I ASSUME THIS IS THE VARIABLE FOR SPEED IN THE X-DIRECTION
		//Comment out the NonSerialized so I can track the speed
		//
		public float speed= 0.0f;
	
		// Is the user pressing the left or right movement keys?
		//THIS WILL HAVE A DIFFERENT APPLICATION
		
		public bool isMoving= false;
	
		// The last collision flags returned from controller.Move
		
		public CollisionFlags collisionFlags; 
	
		// This will keep track of how long we have we been in the air (not grounded)
		
		public float hangTime= 0.0f;
	
		//THIS WILL LET ME DEFINE THE GAMMA FACTOR AT ANY GIVEN TIME, THE SIGN WILL NOT MATTER
		//
		public float gammaX= 1.0f;

		//Declare the target speed here
		public float targetSpeed=0.0f;
	}

	public	PlatformerControllerMovement movement;
	








	
	// We will contain all the jumping related variables in one helper class for clarity.
	[System.Serializable]
	public class PlatformerControllerJumping {
		// Can the character jump?
		public bool enabled= true;
	
		// How high do we jump when pressing jump and letting go immediately
		public float height= 1.0f;
		// We add extraHeight units (meters) on top when holding the button down longer while jumping
		public float extraHeight= 4.1f;
		
		// This prevents inordinarily too quick jumping
		// The next line,  , tells Unity to not serialize the variable or show it in the inspector view.  Very handy for organization!
		
		public float repeatTime= 0.05f;
	
		
		public float timeout= 0.15f;
	
		// Are we jumping? (Initiated with jump button and not grounded yet)
		
		public bool jumping= false;
		
		
		public bool reachedApex= false;
	  
		// Last time the jump button was clicked down
		
		public float lastButtonTime= -10.0f;
		
		// Last time we performed a jump
		
		public float lastTime= -1.0f;
	
		// the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
		
		public float lastStartHeight= 0.0f;
		
		//The number of jumps I have done
		//
		public int NumberOfJumps=0;
	}
	
	public PlatformerControllerJumping jump;
	









	void  Awake (){
		//makes it a singleton
		//and prevents it from being reloded each scen
		if (platformerController == null)
		{
			//DontDestroyOnLoad(gameObject);
			platformerController = this;
		} else if (platformerController != this)
		{
			Destroy(gameObject);
		}

		jennyTransform = GameObject.Find ("JennyAvatar").GetComponent<Transform>();

		boostAudio = jennyTransform.gameObject.AddComponent<AudioSource>();
		boostAudio.clip = boostSound;
		boostAudio.playOnAwake = false;
		boostAudio.volume = 0.2f;

		slowingAudio = jennyTransform.gameObject.AddComponent<AudioSource>();
		slowingAudio.clip = breakingSound;
		slowingAudio.playOnAwake = false;
		slowingAudio.volume = 0.2f;

		jumpAudio = jennyTransform.gameObject.AddComponent<AudioSource>();
		jumpAudio.clip = jumpSound;
		jumpAudio.playOnAwake = false;
		jumpAudio.volume = 0.2f;




		if (movement == null){
			Debug.Log("movement not assigned properly");
		} else {
			movement.direction = jennyTransform.TransformDirection (Vector3.right);
			Spawn ();
		}
	}
	








	void  Spawn (){


		// reset the character's speed

		movement.verticalSpeed = 0.0f;
		movement.speed = 0.0f;
		
		// reset the character's position to the spawnPoint
		jennyTransform.position = spawnPoint.position;
		if (jennyShoot != null)
		{
			jennyShoot.shotState = JennyShoot.ShotState.turnOnShoot;
		}
	}










	void  OnDeath (){
		if(holdingSomething){
			//Gravity removed so it doesn't mess with the physics of the jumping				
			itemCurrentlyHeld.GetComponent<Rigidbody2D>().gravityScale = GameControl.control.gravity;
			//Destroy the joint
			Destroy (holdingJoint);
			pickUpState=PickUpStates.notHolding;
			holdingSomething = false;
			canControl = true;
			//I need picked up object to have the same velocity as the Jenny when she let go.
			lorentzContraction.XVelocityGoal=lorentzContractionForJenny.VxNew
				*movement.direction.x;
			lorentzContraction.IsBeingHeld = false;
			//I'm no longer holding something
			holdingSomething=false;
		}




		GameControl.control.SendMessage("OnDeath");
	//Pause the game with message you died!
		//the GUI should allow restart level

		//send message to the guiscript on the camera that the player is dead
			// it will bring up a death screen

		//levelDisplay.playerDead=true;
		levelDisplay.SendMessage("PlayerDied");
		//Deactivate the player
		jennyShoot.enabled = false;
		//VectorLine.Destroy(ref jennyShoot.gunLineVec);
		jennyTransform.gameObject.SetActive(false);


		
	}








	void OnHitByBomb() 
	{
		OnDeath();
	}
	








	
	
	
	
	
	void  FixedUpdate (){
		// Make sure we are absolutely always in the 2D plane.
		//jennyTransform.position.Set(jennyTransform.position.x,jennyTransform.position.y,0.0f);

		//THIS WAS MOVED FROM UpdateSmoothedMovementDirection SINCE THAT IS CALLED IN UPDATE
		if (jennyTransform.GetComponent<Rigidbody2D>().velocity.x != XVelocityGoal) {
			if (XVelocityGoal == 0f && Mathf.Abs (jennyTransform.GetComponent<Rigidbody2D>().velocity.x) < 1.5f)
			{
				//when jenny's speed is close to zero, just set it to sero, no fapping about with forces
				jennyTransform.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, jennyTransform.GetComponent<Rigidbody2D>().velocity.y);

			} else 
			{
			
				appliedForce = fudgeConstant*(30f*Mathf.Sign(XVelocityGoal-jennyTransform.GetComponent<Rigidbody2D>().velocity.x))*
					jennyTransform.GetComponent<Rigidbody2D>().mass/SpeedOfLight;
				jennyTransform.GetComponent<Rigidbody2D>().AddForce(new Vector2(appliedForce,0.0f) );
			}
		}
		
	}
	




	
	
	void  Update (){
		
		//Jenny's internal clock depends on the gamma factor.
		jennyTime+= Time.deltaTime/movement.gammaX;
		
		
		
		
		if (Input.GetButtonDown ("Jump") && canControl) {
			jump.lastButtonTime = Time.time;
		}
		
		
		//The applied force used here should be done in fixed update.
		UpdateSmoothedMovementDirection();
		
		
		
		// Apply jumping logic
		if(canControl){
			ApplyJumping ();
		}

		
		
		
		// Set rotation to the move direction	
		if (movement.direction.sqrMagnitude > 0.01f){
			
			Quaternion intermediateQuarenion= Quaternion.LookRotation (movement.direction);
			//The extra rotation of 90 degrees is so I wont have to worry about the Lorentz 
			//contraction script acting up.
			Vector3 yEulerReduction = new Vector3(0.0f, 90.0f,0.0f);
			intermediateQuarenion.eulerAngles-=yEulerReduction;
			
			
			jennyTransform.rotation = Quaternion.Slerp (jennyTransform.rotation, 
			                                            intermediateQuarenion, Time.deltaTime * movement.rotationSmoothing);
			
		}
		
		
		
		
		// Update special effects like rocket pack particle effects
		//UpdateEffects ();
		
		
		
		
		
		////////////This will initiate the pickup or putdown function:
		if (Input.GetButtonDown ("PickUp") && canControl) {
			//the speed is taken from the platformer controller and used to make sure that we 
			//only pick up when Jenny is stationary.
			speedOfChar = Mathf.Sqrt (Mathf.Pow (GetSpeed (), 2.0f)
			                          + Mathf.Pow (GetVerticalSpeed (), 2.0f));
			
			if (!holdingSomething && (speedOfChar < 1.5f)) {
				pickUpState = PickUpStates.checkBoxPresence;
			} else if (holdingSomething) {
				pickUpState= PickUpStates.puttingDown;
			}		
		}
		
		////////////THIS DEALS WITH THE PICK UP AND CARRYING OF OBJECTS
		/// RETURNS NULL WHEN NOTHING BEING CARRIED
		PickUpSwitchStatement ();
		
		
		
	}






















	
	
///////
///////
///////This is called in the update loop.  
///////It controls the movement left and right.
///////
///////
	void  UpdateSmoothedMovementDirection (){	
	// Smooth the speed based on the current target direction
		float curSmooth= movement.speedSmoothing * Time.deltaTime;
		
		
	//FOR SOME REASON THE Z DIRECTION KEEPS BECOMING NON ZERO THIS WILL FORCE IT TO THAT	
		movement.direction=new Vector3(movement.direction.x, movement.direction.y, 0.0f);
		
	//THE POINT OF THIS IS TO SLOW THE SPRITE TO A STOP WHEN THE DOWN KEY IS PRESSED.	
		float brake= Input.GetAxisRaw("VerticalDown");
		if (Mathf.Abs (brake) > 0.01f) {
			//play slowing audio
			if (movement.targetSpeed > 5f)
			{
				slowingAudio.Play();
			}

			movement.targetSpeed=0.0f;
			movement.gammaX     =1.0f;
	
		}
		
		//This keeps Jenny from falling faster than the speed of light in the Y direction.
		jennyTransform.GetComponent<Rigidbody2D>().velocity = new Vector2(jennyTransform.GetComponent<Rigidbody2D>().velocity.x,
		                                                  Mathf.Max(jennyTransform.GetComponent<Rigidbody2D>().velocity.y, -movement.maxFallSpeed)
		                                                  );



		//I want to try out a different control system with continuous acceleration
		if (continuousBoost)
		{

			//Animate the boost disk
			ratioOfTimes = ( jennyTime - timeOfBoost)*small1OverTimeBetweenBoosts;
			
			if (ratioOfTimes<1.0f) {
				boostDial.GetComponent<Renderer>().material.SetFloat("_Cutoff", Mathf.Lerp(1,0, ratioOfTimes ) );
			} else if (!boostComplete) {
				boostDial.GetComponent<Renderer>().material.SetFloat("_Cutoff", 0.0f );
				boostComplete=true;
			}




			//IF THE LEFT RIGHT ARROW KEY IS HELD, THE HORIZONTAL SPEED CHANGES
			if ( canControl && Input.GetButton ("Horizontal") && boostComplete ) 
			{	
				
				timeOfBoost = jennyTime;
				boostComplete = false;
				
				float h= Input.GetAxisRaw ("Horizontal");
				//	if (!canControl){
				//		h = 0.0f;
				//	}
				
				//This will give +1 or -1 depending on the direction of the boost 
				float boostdirection= -1.0f+2.0f*Mathf.Sign(h);
				
				if ( movement.direction.x == 0.0f ){
					movement.direction.x = h;
				}		
				
				
				
				/////HERE WE GENERATE THE NEW GAMMA FACTOR
				//A variable that is +ve when boost and dir align and negative otherwise
				float RelativeSignOfBoost=  -1.0f + 2.0f*Mathf.Sign( movement.direction.x * boostdirection );
				//The direction of the character only changes if gamma is less than 1.+ deltagamma, and the old direction
				// and boostdirection are different.	
				//movement.speed= GetGammaX (movement.speed );
				movement.gammaX= GetGammaX (movement.speed );
				if( movement.gammaX < 1.0f+ smallDeltaGamma     ){
					if (RelativeSignOfBoost < 0) {
						movement.direction.x = -movement.direction.x;
						movement.gammaX    = 2.0f - movement.gammaX + smallDeltaGamma;
						slowingAudio.Play();
					} else {
						movement.gammaX    =  movement.gammaX + smallDeltaGamma;
						boostAudio.Play();
					}
					// not changing direction 
				} else {
					if (RelativeSignOfBoost < 0){
						movement.gammaX = movement.gammaX - smallDeltaGamma; 
						slowingAudio.Play();
					} else {
						movement.gammaX = movement.gammaX + smallDeltaGamma; 
						boostAudio.Play();
					}
				}
				
				
				movement.targetSpeed= GetVxTargetFromGammaX ( movement.gammaX );

				
			}



		} else 
		{
		
			//Animate the boost disk
			ratioOfTimes = ( jennyTime - timeOfBoost)*oneOverTimeBetweenBoosts;
			
			if (ratioOfTimes<1.0f) {
				boostDial.GetComponent<Renderer>().material.SetFloat("_Cutoff", Mathf.Lerp(1,0, ratioOfTimes ) );
			} else if (!boostComplete) {
				boostDial.GetComponent<Renderer>().material.SetFloat("_Cutoff", 0.0f );
				boostComplete=true;
			}


			//IF THE LEFT RIGHT ARROW KEY IS HIT, THE HORIZONTAL SPEED CHANGES
			if ( Input.GetButton ("Horizontal") && canControl && boostComplete ) 
			{	
				
				timeOfBoost = jennyTime;
				boostComplete = false;
				
				float h= Input.GetAxisRaw ("Horizontal");
				//	if (!canControl){
				//		h = 0.0f;
				//	}
					
				//This will give +1 or -1 depending on the direction of the boost 
				float boostdirection= -1.0f+2.0f*Mathf.Sign(h);
				
				if ( movement.direction.x == 0.0f ){
				movement.direction.x = h;
				}		
			
				
				
				/////HERE WE GENERATE THE NEW GAMMA FACTOR
				//A variable that is +ve when boost and dir align and negative otherwise
				float RelativeSignOfBoost=  -1.0f + 2.0f*Mathf.Sign( movement.direction.x * boostdirection );
				//The direction of the character only changes if gamma is less than 1.+ deltagamma, and the old direction
				// and boostdirection are different.	
				//movement.speed= GetGammaX (movement.speed );
				movement.gammaX= GetGammaX (movement.speed );
				if( movement.gammaX < 1.0f+ DeltaGamma     ){
					if (RelativeSignOfBoost < 0) {
						if (movement.gammaX >1.1f)
						{
							slowingAudio.Play();
						} else 
						{
							boostAudio.Play();
						}


						movement.direction.x = -movement.direction.x;
						movement.gammaX    = 2.0f - movement.gammaX + DeltaGamma;

					} else {
						movement.gammaX    =  movement.gammaX + DeltaGamma;
						boostAudio.Play();
					}
				// not changing direction 
				} else {
					if (RelativeSignOfBoost < 0){
						if (movement.gammaX >1.1f)
						{
							slowingAudio.Play();
						} else 
						{
							boostAudio.Play();
						}


						movement.gammaX = movement.gammaX - DeltaGamma; 


					} else {
						movement.gammaX = movement.gammaX + DeltaGamma; 
						boostAudio.Play();
					}
				}
			
		
					movement.targetSpeed= GetVxTargetFromGammaX ( movement.gammaX );
		
				

			
			}





		}









			
	//We have a target speed for the X direction. To convert this to actual speed, we use the 
	//same method used in LorentzContraction	
		XVelocityGoal=movement.targetSpeed*movement.direction.x;


		//THIS WAS MOVED TO FIXED UPDATE TO CORRECTLY MESS WITH THE FORCE
		/*
		if (jennyTransform.rigidbody2D.velocity.x != XVelocityGoal) {
			appliedForce = fudgeConstant*(XVelocityGoal-jennyTransform.rigidbody2D.velocity.x)*jennyTransform.rigidbody2D.mass/SpeedOfLight;
			jennyTransform.rigidbody2D.AddForce(new Vector3(appliedForce,0.0f,0.0f));
			
			
		}
		*/
		
		
		//The speed is smoothed out by lerp		
		//This fundges the speed up factor so it looks like the lorentz contraction is 
		//taking time
		movement.speed = Mathf.Lerp (movement.speed, movement.targetSpeed, curSmooth);
		//Debug.Log("The gammaX is"+GetGammaX (movement.speed ));
		
		//Send the level display the new gammaX;
		levelDisplay.gammaX = GetGammaX (movement.speed );
		levelDisplay.speed  = movement.speed;
		
		movement.hangTime = 0.0f;
		if(holdingSomething){
			if (gammaX != gammaX0ld){
				lorentzContraction.gammaX = GetGammaX (movement.speed);
				gammaX0ld=gammaX;
			}
		}
			
	}

	



	//void  UpdateEffects (){}//see earlier version when this is needed}



	
	
	//This function gets the instantaneous gamma factor in the x direction
	public float  GetGammaX ( float Vx   ){
		float gammaX=1.0f/(   Mathf.Sqrt(   1.0f  -  Mathf.Pow((Vx / SpeedOfLight ), 2 )     )    );
		return gammaX;
	} 
	
	
	//This function will convert the new gamma factor to an xvelocity
	float  GetVxTargetFromGammaX (float GAMMAX ){
		return SpeedOfLight * Mathf.Sqrt(  1.0f -  Mathf.Pow(GAMMAX, -2)   );
	}
	
		
	
	
	void  ApplyJumping (){


		
		if (IsGrounded()){
			jump.NumberOfJumps=0;	
		} else if (jump.NumberOfJumps==0)
		{
			//this keeps me from getting two jumps if jenny runs off a ledge.
			jump.NumberOfJumps = 1;
		}
		
		
		

		if ( jump.NumberOfJumps < MaxNumbOfJumps && Input.GetButtonDown("Jump")    ){
		//    && Time.time > jumpDelay + timeOfJump  ) {

		//play sound
			jumpAudio.Play();


		//	timeOfJump=Time.time;
			jump.NumberOfJumps +=1;
			// Jump
			// - Only when pressing the button down
			// - With a timeout so you can press the button slightly before landing		
			//if (jump.enabled && Time.time < jump.lastButtonTime + jump.timeout) {
				//movement.verticalSpeed = CalculateJumpVerticalSpeed (jump.height);
				//movement.verticalSpeed += SpeedOfJump;
				//movement.inAirVelocity  = lastPlatformVelocity;
				//SendMessage ("DidJump", SendMessageOptions.DontRequireReceiver);
			//}
			jennyTransform.GetComponent<Rigidbody2D>().velocity = new Vector3(jennyTransform.GetComponent<Rigidbody2D>().velocity.x, SpeedOfJump,0.0f);

			//jennyTransform.rigidbody.velocity +=new Vector3(0.0f, SpeedOfJump,0.0f);
			
		}
	}
	
	
	
	void  DidJump (){
		jump.jumping = true;
		jump.reachedApex = false;
		jump.lastTime = Time.time;
		jump.lastStartHeight = jennyTransform.position.y;
		jump.lastButtonTime = -10;
	}
	
	
	
	
		 


	
	





		
/////////////This is the algorythm that picks up the carried object
	void PickUpSwitchStatement ()
	{

		//notHolding       //if not holding, the statement is ignored
		//checkBoxPresence,//checks that it is alright to pick something up
		//automateLift,    // lifts the object into position
		//attachJoint,     // joins the object to Jenny
		//pickUpInterupted,//drops the object, halts the process if interupted 
		//pickUpComplete,  // move object around with Jenny
		//puttingDown      //puts down the object, sets state to not holding

		switch (pickUpState) {
		case (PickUpStates.notHolding):
			break;//this is the default state of the pick up loop

		//In the check box line we see if there is a box to pick up, 
		//if the is remove control and set the pickupstate to picking up something	
		case (PickUpStates.checkBoxPresence):
			directionOfCharacter = GetDirection ();
			liftStartTime = Time.time;
			pickUpRayStart= new Vector2(jennyTransform.position.x, 
			                            jennyTransform.position.y-0.5f*jennyTransform.localScale.y);
			hit2 = Physics2D.Raycast (pickUpRayStart,
			           new Vector2(directionOfCharacter.x,0.0f)
			           , 4.5f, 1<<8) ;
			//layer of pickuppable stuff is 8.
			if ( hit2.collider!= null ) {
				
				//Debug.Log ("The hit object is called"+ hit2.transform.name);
//				Physics2D.Raycast (pickUpRayStart, new Vector2(directionOfCharacter.x,0.0f)  ,  4.5f , layerMaskOfHit) 
//				&& hit2.collider.CompareTag ("PickUp")




				jennyShoot.heldObject = hit2.transform;
				jennyShoot.shotState = JennyShoot.ShotState.startHolding;
				//jennyShoot.canShoot = false;



				
				//There is a box in front of the character it is picked up prefferentially	
//				typeOfPickUp = "fromSide";
				automatedPickUpComplete = false;
				//Remove control from the character Controller during the animation
				canControl = false;
				//the transform of the pick up box;
				itemCurrentlyHeld = hit2.transform;
				//Gravity removed so it doesn't mess with the physics of the jumping				
				itemCurrentlyHeld.GetComponent<Rigidbody2D>().gravityScale = 0.0f;	
				itemCurrentlyHeld.GetComponent<Rigidbody2D>().isKinematic = false;
				//assign the mass of the objec for reference
				massOfObjInitial = itemCurrentlyHeld.GetComponent<Rigidbody2D>().mass;





				//this still doesn't work if the two colliders are touching sweeptest doesn't work here


				///////////////////////////////
				///////////////////////////////
				//There is more than one type of lorentz contraction 
				//it depends on what kind of box we are picking up.
				///////////////////////////////
				///////////////////////////////
				lorentzContraction = 
				 itemCurrentlyHeld.GetComponent ("LorentzContraction") 
				 as LorentzContraction;
				//This allows a c
				lorentzContraction.IsBeingPickedUp = true;
				
				//We need to set the carried objects scale back to it's original settings
				itemCurrentlyHeld.localScale = new Vector3 (lorentzContraction.xScale0,
				lorentzContraction.yScale0, lorentzContraction.zScale0);
				
				//We need to remove the rigidbody x constraint
				//CONSTRAINT MUST BE DONE WITH A JOINT
				//hit2.transform.rigidbody2D.constraints = RigidbodyConstraints.FreezePositionZ
				//	|RigidbodyConstraints.FreezeRotation;
				lorentzContraction.xMotionStatic = false;


						
				//The initial position is needed to do the lift
				initialXposition       = itemCurrentlyHeld.position.x;
				//initialHeightOfCarried = itemCurrentlyHeld.position.y;
				//liftStartTime = Time.time;

				
				//the goal height will be how far above Jenny the block must be (shouldn't calculate more than once.
				goalHeight = 0.5f * jennyTransform.localScale.y + 7.0f + 0.5f*itemCurrentlyHeld.localScale.y;
				riseSpeed  = goalHeight / riseTime;
				slideOverSpeed = jennyTransform.localScale.x / riseTime;

				//make the object lighter for transport and lift
				itemCurrentlyHeld.GetComponent<Rigidbody2D>().mass = massOfObjCarried;

				//This moves the switch statement to the next part of the loop
				pickUpState = PickUpStates.automateLift;
				goto case PickUpStates.automateLift;
				
				
			} else {
				//If there is no box to pick up, leave the loop
				pickUpState = PickUpStates.notHolding;
				break;
			}
			
		case (PickUpStates.automateLift):

			//interupt the lift if it takes too long
			if (Time.time > liftStartTime+maxLiftDuration)
			{
				pickUpState = PickUpStates.pickUpInterupted;
				goto case PickUpStates.pickUpInterupted;
			}


			if (!automatedPickUpComplete) {
				if (itemCurrentlyHeld == null)
				{
					pickUpState = PickUpStates.pickUpInterupted;
					goto case PickUpStates.pickUpInterupted;
				} else 
				{
					automatedPickUpComplete = pickUpAutomation ();//typeOfPickUp);
					break;
				}
			} else {
				pickUpState = PickUpStates.attachJoint;
				goto case PickUpStates.attachJoint;
			}
			
		case (PickUpStates.attachJoint):

			//interupt the lift if it takes too long
			if (Time.time > liftStartTime+maxLiftDuration)
			{
				pickUpState = PickUpStates.pickUpInterupted;
				goto case PickUpStates.pickUpInterupted;
			}

			//Initialize the joint.	
			holdingJoint = jennyTransform.gameObject.AddComponent<SliderJoint2D> ();
			//The carried objects can only pivot around the z axis	
			//holdingJoint.axis = new Vector2 (0.0f, 1.0f);
			//The carried object will be a short distance from 
			holdingJoint.connectedBody = itemCurrentlyHeld.GetComponent<Rigidbody2D>();

			holdingJoint.angle     = 0.0f;
			holdingJoint.anchor    = new Vector2(0.0f,goalHeight/jennyTransform.localScale.y);
			holdingJoint.useLimits = true;
			//holdingJoint2.xMotion = ConfigurableJointMotion.Limited; 
			//Define DeltaXShift to be used by the carried object's script LorentzContractions	
			//DeltaXShift = (itemCurrentlyHeld.position.x - jennyTransform.position.x);

			
			lorentzContraction.IsBeingHeld = true;
		
			holdingSomething = true;





			//We need to remove the rigidbody x constraint
			//CONSTRAINT MUST BE DONE WITH A JOINT
			//hit2.transform.rigidbody.constraints = RigidbodyConstraints.FreezePositionZ
			//	|RigidbodyConstraints.FreezeRotation;
			lorentzContraction.xMotionStatic = false;



			
			//directionAtPickUp=Mathf.Sign(movement.direction.x);
			//The maximum range of xcoordinate is set up by playing 
			//with the joint limits
			SoftJointLimit holdingJointLimits = new SoftJointLimit ();

			//holdingJointLimits.spring = 0.0f;
			//holdingJointLimits.damper = 0.0f;
			holdingJointLimits.bounciness = 0.0f;
			
			holdingJointLimits.limit= lorentzContraction.xScale0;
			
			
			//holdingJointLimits.limit = lorentzContraction.xScale0;
			//holdingJoint2.linearLimit = holdingJointLimits;
		
			//The holding joint's target position is defaulted to zero
			//holdingJoint2.targetPosition = new Vector3 (0.0f, 0.0f, 0.0f);
			
			//The carried object is driven with this force
			JointDrive holdingJointDrive = new JointDrive ();
			holdingJointDrive.mode = JointDriveMode.Position;
			holdingJointDrive.positionSpring = 500.0f;
			holdingJointDrive.positionDamper = 0.0f;
			holdingJointDrive.maximumForce = Mathf.Infinity;
			//holdingJoint2.yDrive = holdingJointDrive;
			
					
			//Return control to the character Controller 
			canControl = true;
			
			
			//Define DeltaXShift to have correct lorentz contractions
			//see the update loop.
			//DeltaXShift = (itemCurrentlyHeld.position.x - jennyTransform.position.x);
			

			pickUpState = PickUpStates.pickUpComplete;
			break;

		case (PickUpStates.pickUpInterupted):
			//If the lift is interupted in any way this gets called
			//May need a different one if it is mid-top-lift

			if (itemCurrentlyHeld != null)
			{
				//return mass to original mass just in case;
				itemCurrentlyHeld.GetComponent<Rigidbody2D>().mass = massOfObjInitial;
				//I need to turn gravity back on for the carried object
				itemCurrentlyHeld.GetComponent<Rigidbody2D>().gravityScale = GameControl.control.gravity;
			}


			//Return control to the character Controller 
			canControl = true;

			//JENNY CAN SHOOT AGAIN
			jennyShoot.shotState = JennyShoot.ShotState.turnOnShoot;
			//jennyShoot.canShoot = true;
			jennyShoot.heldObject = null;


			pickUpState = PickUpStates.notHolding;
			break;
		
		case (PickUpStates.pickUpComplete):


			//The carried box needs to stay centered even under lorentz contraction
			directionOfCharacter = GetDirection ();
			gammaX = lorentzContractionForJenny.gammaX;
			//hold

			//holdingJoint2.targetPosition = new Vector3 (0.0f, 
			//                                            DeltaXShift * directionOfCharacter.x * (1 - 1 / gammaX),
			//                                            0.0f);
			break;



		case (PickUpStates.puttingDown):
			lorentzContraction.IsBeingHeld = false;

			//Gravity removed so it doesn't mess with the physics of the jumping				
			itemCurrentlyHeld.GetComponent<Rigidbody2D>().gravityScale =  GameControl.control.gravity;
			
			Destroy (holdingJoint);
			holdingSomething = false;
			canControl = true;

			
			//SET MOVEMENT SPEED TO ZERO WHEN SOMETHING LET GO			
			movement.targetSpeed=0.0f;
			movement.gammaX = 1f;
			lorentzContractionForJenny.gammaX = 1.0f;
		

			//I need picked up object to have the same velocity as the Jenny when she let go.
						
			if(gammaX<1.5f){
				//If jenny is moving slowly, actively push the block off so it doesn't land on her if possibel
				lorentzContraction.XVelocityGoal=GetVxTargetFromGammaX (1.5f)//lorentzContractionForJenny.GetXVelocity VxNew
					*movement.direction.x;
				
			} else {
				lorentzContraction.XVelocityGoal=lorentzContractionForJenny.VxNew
					*movement.direction.x;
			}
			
			//bumb the carried object a little so it moves away from Jenny.
			itemCurrentlyHeld.transform.GetComponent<Rigidbody2D>().velocity+= new Vector2(0.0f,10.0f);

			//set the objects mass back to original
			itemCurrentlyHeld.GetComponent<Rigidbody2D>().mass= massOfObjInitial;


			//JENNY CAN SHOOT AGAIN
			jennyShoot.shotState = JennyShoot.ShotState.turnOnShoot;
			//jennyShoot.canShoot = true;
			jennyShoot.heldObject = null;



			//When the object is released the state goes back to not holding
			pickUpState = PickUpStates.notHolding;
			break;
		
		case (PickUpStates.objectDestroyed):
			//has the object been destroyed while being carried?

			lorentzContraction.IsBeingHeld = false;
			

			Destroy (holdingJoint);
			holdingSomething = false;
			canControl = true;
			

			//JENNY CAN SHOOT AGAIN
			jennyShoot.shotState = JennyShoot.ShotState.turnOnShoot;
			//jennyShoot.canShoot = true;
			jennyShoot.heldObject = null;
			
			
			
			//When the object is released the state goes back to not holding
			pickUpState = PickUpStates.notHolding;
			break;
		
		}
		
		
	}	
	
	
	

	
	
	
	
	
	
	
	////////////This function describes the pick up automation
	bool pickUpAutomation (){//string typeOfPickUp2){
	//This should be done using forces instead of setting positions.
		//temp bit
		/*
		if (itemCurrentlyHeld.position.y < initialHeightOfCarried + 2.0f)
		{//this is the acceleration phase of the lift
			tempLiftSpeed = Mathf.Lerp (0.0f, riseSpeed, (itemCurrentlyHeld.position.y-initialHeightOfCarried)*0.5f);
			itemCurrentlyHeld.rigidbody.velocity = new Vector3 ( itemCurrentlyHeld.rigidbody.velocity.x,
			                                                    tempLiftSpeed,
			                                                    itemCurrentlyHeld.rigidbody.velocity.z);
			return false;
		} else if (itemCurrentlyHeld.position.y <  jennyTransform.position.y + goalHeight - 2.0f )
		{//constant speed phase
			itemCurrentlyHeld.rigidbody.velocity = new Vector3 ( itemCurrentlyHeld.rigidbody.velocity.x,
			                                                    riseSpeed,
			                                                    itemCurrentlyHeld.rigidbody.velocity.z);
		
			return false;
		} else 
		{//decelerate block
			tempLiftSpeed = Mathf.Lerp (0.0f, riseSpeed,  ((jennyTransform.position.y +goalHeight)- itemCurrentlyHeld.position.y)*0.5f );
			itemCurrentlyHeld.rigidbody.velocity = new Vector3 ( itemCurrentlyHeld.rigidbody.velocity.x,
			                                                    tempLiftSpeed,
			                                                    itemCurrentlyHeld.rigidbody.velocity.z);
			return true;
		}*/


		
		
		
		
		
		
		
		if (itemCurrentlyHeld.position.y < jennyTransform.position.y + goalHeight)
		{
			itemCurrentlyHeld.GetComponent<Rigidbody2D>().velocity = new Vector2 ( itemCurrentlyHeld.GetComponent<Rigidbody2D>().velocity.x,
			                                                    riseSpeed);
			return false;
		} else if (directionOfCharacter.x * itemCurrentlyHeld.position.x > 
		           directionOfCharacter.x * initialXposition - jennyTransform.localScale.x) 
		{ //slide back


			itemCurrentlyHeld.GetComponent<Rigidbody2D>().velocity = new Vector2 ( -slideOverSpeed* directionOfCharacter.x,
			                                                    jennyTransform.GetComponent<Rigidbody2D>().velocity.y);

			//itemCurrentlyHeld.position = new Vector3 (itemCurrentlyHeld.position.x,
			//                                        jennyTransform.position.y + goalHeight,
			//                                     	0.0f);  
		
			return false;
		}  else
		{

			itemCurrentlyHeld.GetComponent<Rigidbody2D>().velocity = jennyTransform.GetComponent<Rigidbody2D>().velocity;
			//this makes 100 percent sure the blocks in the right place
			itemCurrentlyHeld.position = new Vector3 (initialXposition - directionOfCharacter.x * jennyTransform.localScale.x,
			                                          jennyTransform.position.y + goalHeight,
			                                          0.0f);  


			return true;
		}



		
		
		
		
		/*  //this lifts by setting position, not velocity
			if (Time.time < liftStartTime+ 2.0f* riseTime) {
				//the goal height will be how far above Jenny the block must be (shouldn't calculate more than once.
				goalHeight = 0.5f * jennyTransform.localScale.y + 7.0f + 0.5f*itemCurrentlyHeld.localScale.y;
				riseSpeed  = goalHeight / riseTime;
				Debug.Log ("Goal Height is = " + goalHeight);
				//the goalLiftPosY is the curent goal height difference between Jenny and the picked up object
				goalLiftPosY = Mathf.Lerp (0.0f, goalHeight, (Time.time - liftStartTime) / riseTime);
				
				
				if( hit2.transform.rigidbody.SweepTest(Vector3.up , out hit , 0.5f)   )
				{
					pickUpState = PickUpStates.pickUpInterupted;
				} else {
					
					itemCurrentlyHeld.position = new Vector3 (itemCurrentlyHeld.position.x,
					                                          jennyTransform.position.y + goalLiftPosY,
					                                          itemCurrentlyHeld.position.z);  
				}
				
				
				//This kicks in after the block has been lifted (slides it back a bit//
				if (Time.time > liftStartTime + riseTime) {
					
					if (directionOfCharacter.x * itemCurrentlyHeld.position.x > 
					    directionOfCharacter.x * initialXposition - jennyTransform.localScale.x) {
						
						itemCurrentlyHeld.position = new Vector3 (Mathf.Lerp (initialXposition, 
						                                                      initialXposition - directionOfCharacter.x * jennyTransform.localScale.x, 
						                                                      (Time.time - liftStartTime - riseTime) / riseTime),
						                                          itemCurrentlyHeld.position.y,
						                                          itemCurrentlyHeld.position.z);
					}
				}




			return false;
		} else {
			return true;
		}*/



	}
	


	


	
	
	
	// Various helper functions below:
	public float GetSpeed (){
		return movement.speed;
	}
	
	public float GetVerticalSpeed (){
		return movement.verticalSpeed;
	}
	
	//public Vector3 GetVelocity (){
	//	return movement.velocity;
	//}
	
	
	public bool IsMoving (){
		return movement.isMoving;
	}
	
	public bool IsJumping (){
		return jump.jumping;
	}
	
	public bool IsTouchingCeiling (){
		return (movement.collisionFlags & CollisionFlags.CollidedAbove) != 0;
	}
	
	public Vector3 GetDirection (){
		return movement.direction;
	}
	
	public float  GetHangTime (){
		return movement.hangTime;
	}
	
	public void  Reset (){
		gameObject.tag = "Player";
	}
	
	public void  SetControllable ( bool controllable  ){
		canControl = controllable;
	}





	//This will determine if Jenny is grounded// should use boxcast for this 
	public bool IsGrounded(){

		ground = Physics2D.BoxCastAll(new Vector2( jennyTransform.position.x,   
		                                       jennyTransform.position.y), //base of jenny
		                           new Vector2( 4.0f, 2.0f) ,   //size of box to cast
		                           0.0f, 	//angle?
		                           -Vector2.up, //down?
		                           6.0f,//ditance to cast 
		                           ~(1<<9) );//layer mask, ignore jenny's collider READS inverse layer nine
		foreach (RaycastHit2D groundElement in ground)
		{
			if (groundElement.transform !=null && groundElement.collider.isTrigger == false)
			{
				return true;
			}
		}
		return false;
		
	}





	
	
	
	
	
}