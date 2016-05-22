
// Does this script currently respond to Input?
var canControl = true;

// The character will spawn at spawnPoint's position when needed.  This could be changed via a script at runtime to implement, e.g. waypoints/savepoints.
var spawnPoint : Transform;


//We need to define the speed of light
@System.NonSerialized
var SpeedOfLight = 15.0;
//The size of the change in gamma upon keystroke is
@System.NonSerialized
var DeltaGamma = 0.5;
//for now the velocity of the jump will be the speed of light, may play with it later
@System.NonSerialized
var SpeedOfJump = 3.0*SpeedOfLight;
//Define the maximum number of jumps 
@System.NonSerialized
var MaxNumbOfJumps=3;



class PlatformerControllerMovement {



//Leave the gravity stuff alone for now:
	var inAirControlAcceleration = 1.0;

	// The gravity for the character
	var gravity = 60.0;
	var maxFallSpeed = 200.0;

	// How fast does the character change speeds?  Higher is faster.
	var speedSmoothing = 5.0;
//I will need to sort out what to do with this later
	// This controls how fast the graphics of the character "turn around" when the player turns around using the controls.
	var rotationSmoothing = 10.0;

	// The current move direction in x-y.  This will always been (1,0,0) or (-1,0,0)
	// The next line, @System.NonSerialized , tells Unity to not serialize the variable 
	//	or show it in the inspector view.  Very handy for organization!
	//@System.NonSerialized
	var direction = Vector3 (1,0,0);

	// The current vertical speed
	@System.NonSerialized
	var verticalSpeed = 0.0;

	// The current movement speed.  This gets smoothed by speedSmoothing.
	//I ASSUME THIS IS THE VARIABLE FOR SPEED IN THE X-DIRECTION
	//Comment out the NonSerialized so I can track the speed
	//@System.NonSerialized
	var speed = 0.0f;

	// Is the user pressing the left or right movement keys?
	//THIS WILL HAVE A DIFFERENT APPLICATION
	@System.NonSerialized
	var isMoving = false;

	// The last collision flags returned from controller.Move
	@System.NonSerialized
	var collisionFlags : CollisionFlags; 

	// We will keep track of an approximation of the character's current velocity, so that we return it from GetVelocity () for our camera to use for prediction.
	@System.NonSerialized
	var velocity : Vector3;
	
	// This keeps track of our current velocity while we're not grounded?
	@System.NonSerialized
	var inAirVelocity = Vector3.zero;

	// This will keep track of how long we have we been in the air (not grounded)
	@System.NonSerialized
	var hangTime = 0.0;

	//THIS WILL LET ME DEFINE THE GAMMA FACTOR AT ANY GIVEN TIME, THE SIGN WILL NOT MATTER
	//@System.NonSerialized
	var gammaX = 1.0;
	
	
	//Declare the target speed here
	var targetSpeed=0.0f;
}

var movement : PlatformerControllerMovement;



// We will contain all the jumping related variables in one helper class for clarity.
class PlatformerControllerJumping {
	// Can the character jump?
	var enabled = true;

	// How high do we jump when pressing jump and letting go immediately
	var height = 1.0;
	// We add extraHeight units (meters) on top when holding the button down longer while jumping
	var extraHeight = 4.1;
	
	// This prevents inordinarily too quick jumping
	// The next line, @System.NonSerialized , tells Unity to not serialize the variable or show it in the inspector view.  Very handy for organization!
	@System.NonSerialized
	var repeatTime = 0.05;

	@System.NonSerialized
	var timeout = 0.15;

	// Are we jumping? (Initiated with jump button and not grounded yet)
	@System.NonSerialized
	var jumping = false;
	
	@System.NonSerialized
	var reachedApex = false;
  
	// Last time the jump button was clicked down
	@System.NonSerialized
	var lastButtonTime = -10.0;
	
	// Last time we performed a jump
	@System.NonSerialized
	var lastTime = -1.0;

	// the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
	@System.NonSerialized
	var lastStartHeight = 0.0;
	
	//The number of jumps I have done
	//@System.NonSerialized
	var NumberOfJumps =0;
}

var jump : PlatformerControllerJumping;

private var controller : CharacterController;

// Moving platform support.
private var activePlatform : Transform;
private var activeLocalPlatformPoint : Vector3;
private var activeGlobalPlatformPoint : Vector3;
private var lastPlatformVelocity : Vector3;

// This is used to keep track of special effects in UpdateEffects ();
private var areEmittersOn = false;

function Awake () {
	movement.direction = transform.TransformDirection (Vector3.forward);
	controller = GetComponent (CharacterController);
	Spawn ();
}

function Spawn () {
	// reset the character's speed
	movement.verticalSpeed = 0.0;
	movement.speed = 0.0;
	
	// reset the character's position to the spawnPoint
	transform.position = spawnPoint.position;
	
}

function OnDeath () {
	Spawn ();
}

	

		
	//This is where the left right movement of the character is controled
	//I REMOVED THE AIR CONTROLERS BECAUSE I FEEL IT SHOULD NOT MATTER
	//THE isMoving COMAND SEEMS USELESS FOR THE IMPLEMENTATION I WANT
function UpdateSmoothedMovementDirection () {	
	// Smooth the speed based on the current target direction
	var curSmooth = movement.speedSmoothing * Time.deltaTime;
	
	
	//FOR SOME REASON THE Z DIRECTION KEEPS BECOMING NON ZERO THIS WILL FORCE IT TO THAT	
	movement.direction.z=0.0;
	
//THE POINT OF THIS IS TO SLOW THE SPRITE TO A STOP WHEN THE DOWN KEY IS PRESSED.	
	var brake = Input.GetAxisRaw("VerticalDown");
	if (Mathf.Abs (brake) > 0.01) {
		movement.targetSpeed=0.0;
	}
	
//IF THE LEFT RIGHT ARROW KEY IS HIT, THE HORIZONTAL SPEED CHANGES
	if ( Input.GetButtonDown ("Horizontal") ) {	
	
		var h = Input.GetAxisRaw ("Horizontal");
		//	if (!canControl){
		//		h = 0.0;
		//	}
		

			
		//This will give +1 or -1 depending on the direction of the boost 
		var boostdirection = -1.0+2.0*Mathf.Sign(h);
		
		if ( movement.direction.x == 0.0 ){
		movement.direction.x = h;
		}		
	
		
		
		/////HERE WE GENERATE THE NEW GAMMA FACTOR
		//A variable that is +ve when boost and dir align and negative otherwise
		var RelativeSignOfBoost =  -1.0 + 2.0*Mathf.Sign( movement.direction.x * boostdirection );
		//The direction of the character only changes if gamma is less than 1.+ deltagamma, and the old direction
		// and boostdirection are different.	
		GetGammaX (movement.speed );
		if( movement.gammaX < 1.0+ DeltaGamma     ){
			if (RelativeSignOfBoost < 0) {
				movement.direction.x = -movement.direction.x;
				movement.gammaX    = 2.0 - movement.gammaX + DeltaGamma;
			} else {
				movement.gammaX    =  movement.gammaX + DeltaGamma;
			}
		// not changing direction 
		} else {
			if (RelativeSignOfBoost < 0){
				movement.gammaX = movement.gammaX - DeltaGamma; 
			} else {
				movement.gammaX = movement.gammaX + DeltaGamma; 
			}
		}
		
		//Convert the gamma factor to a new target speed
		GetVxTargetFromGammaX ( );
	}
		
		
		//The speed is smoothed out by lerp		
		movement.speed = Mathf.Lerp (movement.speed, movement.targetSpeed, curSmooth);
		GetGammaX (movement.speed );
		movement.hangTime = 0.0;
		
}


//This function gets the instantaneous gamma factor in the x direction
function GetGammaX (Vx : float ) {
	movement.gammaX=1.0/(   Mathf.Sqrt(   1.0  -  Mathf.Pow((Vx / SpeedOfLight ), 2 )     )    );
} 


//This function will convert the new gamma factor to an xvelocity
function GetVxTargetFromGammaX ( ) {
	movement.targetSpeed = SpeedOfLight * Mathf.Sqrt(  1.0 -  Mathf.Pow(movement.gammaX, -2)   );
}









function ApplyJumping () {
	// Prevent jumping too fast after each other
	//if (jump.lastTime + jump.repeatTime > Time.time)
	//	return;

	if (controller.isGrounded){
		jump.NumberOfJumps = 0;
	} else if ( Input.GetButtonDown("Jump") ) {
		jump.NumberOfJumps = Mathf.Max(1, jump.NumberOfJumps)+1;
	}
		
		
	if ( jump.NumberOfJumps < MaxNumbOfJumps && Input.GetButtonDown("Jump")) {
		// Jump
		// - Only when pressing the button down
		// - With a timeout so you can press the button slightly before landing		
		//if (jump.enabled && Time.time < jump.lastButtonTime + jump.timeout) {
			//movement.verticalSpeed = CalculateJumpVerticalSpeed (jump.height);
			movement.verticalSpeed += SpeedOfJump;
			movement.inAirVelocity  = lastPlatformVelocity;
			SendMessage ("DidJump", SendMessageOptions.DontRequireReceiver);
		//}
	}
}

//
//We don't mess with gravity in this program
//


function ApplyGravity () {
	/*
	// Apply gravity
	var jumpButton = Input.GetButton ("Jump");
	
	if (!canControl)
		jumpButton = false;
	
	// When we reach the apex of the jump we send out a message
	if (jump.jumping && !jump.reachedApex && movement.verticalSpeed <= 0.0) {
		jump.reachedApex = true;
		SendMessage ("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
	}
	
	// * When jumping up we don't apply gravity for some time when the user is holding the jump button
	//   This gives more control over jump height by pressing the button longer
	var extraPowerJump =  jump.jumping && movement.verticalSpeed > 0.0 
	&& jumpButton && transform.position.y < jump.lastStartHeight 
	+ jump.extraHeight && !IsTouchingCeiling ();
	*/
	
	
	//if (extraPowerJump)
	//	return;
	//else 
	if (controller.isGrounded)
		movement.verticalSpeed = -movement.gravity * Time.deltaTime;
	else
		movement.verticalSpeed -= movement.gravity * Time.deltaTime;
		
	// Make sure we don't fall any faster than maxFallSpeed.  This gives our character a terminal velocity.
	//I will make sure it is very high
	movement.verticalSpeed = Mathf.Max (movement.verticalSpeed, -movement.maxFallSpeed);
}



/*
function CalculateJumpVerticalSpeed (targetJumpHeight : float) {
	// From the jump height and gravity we deduce the upwards speed 
	// for the character to reach at the apex.
	return Mathf.Sqrt (2 * targetJumpHeight * movement.gravity);
}
*/


function DidJump () {
	jump.jumping = true;
	jump.reachedApex = false;
	jump.lastTime = Time.time;
	jump.lastStartHeight = transform.position.y;
	jump.lastButtonTime = -10;
}

//An empty start function
/*
function Start() {
}
*/



function UpdateEffects () {
	wereEmittersOn = areEmittersOn;
	areEmittersOn = jump.jumping && movement.verticalSpeed > 0.0;
	
	// By comparing the previous value of areEmittersOn to the new one, we will only update the particle emitters when needed
	if (wereEmittersOn != areEmittersOn) {
		for (var emitter in GetComponentsInChildren (ParticleEmitter)) {
			emitter.emit = areEmittersOn;
		}
	}
}



function FixedUpdate () {
	// Make sure we are absolutely always in the 2D plane.
	transform.position.z = 0;

}




function Update () {
	if (Input.GetButtonDown ("Jump") && canControl) {
		jump.lastButtonTime = Time.time;
	}
	
	
	
	UpdateSmoothedMovementDirection();

	
	// Apply gravity  
	// - extra power jump modifies gravity
	ApplyGravity ();

	// Apply jumping logic
	ApplyJumping ();
	
	// Moving platform support
	if (activePlatform != null) {
		var newGlobalPlatformPoint = activePlatform.TransformPoint(activeLocalPlatformPoint);
		var moveDistance = (newGlobalPlatformPoint - activeGlobalPlatformPoint);
		transform.position = transform.position + moveDistance;
		lastPlatformVelocity = (newGlobalPlatformPoint - activeGlobalPlatformPoint) / Time.deltaTime;
	} else {
		lastPlatformVelocity = Vector3.zero;	
	}
	
	activePlatform = null;
	
	// Save lastPosition for velocity calculation.
	lastPosition = transform.position;
	
	// Calculate actual motion
	var currentMovementOffset = movement.direction * movement.speed + Vector3 (0, movement.verticalSpeed, 0) 
	+ movement.inAirVelocity;
	
	// We always want the movement to be framerate independent.  Multiplying by Time.deltaTime does this.
	currentMovementOffset *= Time.deltaTime;
	
   	// Move our character!
	movement.collisionFlags = controller.Move (currentMovementOffset);
	
	// Calculate the velocity based on the current and previous position.  
	// This means our velocity will only be the amount the character actually moved as a result of collisions.
	movement.velocity = (transform.position - lastPosition) / Time.deltaTime;
	
	
	
	//If the movement speed and velocity are off by a lot, set targetspeed and movement speed to movement.velocity.c
	//THIS WILL PROBABLY NEED TO BE FIXED (This messes up backwards motion :( )
	
	//if (Mathf.Abs(movement.speed-movement.velocity.x) > SpeedOfLight/10.0) {
	//	movement.speed = movement.velocity.x;
	//	movement.targetSpeed=movement.velocity.x;
	//} else {
	//	movement.speed = movement.velocity.x;
	//}
	
	
	
	// Moving platforms support
	if (activePlatform != null) {
		activeGlobalPlatformPoint = transform.position;
		activeLocalPlatformPoint = activePlatform.InverseTransformPoint (transform.position);
	}
	
	// Set rotation to the move direction	
	if (movement.direction.sqrMagnitude > 0.01){
		var intermediateQuarenion= Quaternion.LookRotation (movement.direction);
		//The extra rotation of 90 degrees is so I wont have to worry about the Lorentz 
		//contraction script acting up.
		intermediateQuarenion.eulerAngles.y-=90.0;
		transform.rotation = Quaternion.Slerp (transform.rotation, 
		intermediateQuarenion, Time.deltaTime * movement.rotationSmoothing);
//		transform.rotation = Quaternion.Slerp (transform.rotation, 
//		Quaternion.LookRotation (movement.direction), Time.deltaTime * movement.rotationSmoothing);
		}


	// We are in jump mode but just became grounded
	if (controller.isGrounded) {
		movement.inAirVelocity = Vector3.zero;
		if (jump.jumping) {
			jump.jumping = false;
			SendMessage ("DidLand", SendMessageOptions.DontRequireReceiver);

			var jumpMoveDirection = movement.direction * movement.speed + movement.inAirVelocity;
			if (jumpMoveDirection.sqrMagnitude > 0.01)
				movement.direction = jumpMoveDirection.normalized;
		}
	}	

	// Update special effects like rocket pack particle effects
	UpdateEffects ();
}

function OnControllerColliderHit (hit : ControllerColliderHit)
{
	if (hit.moveDirection.y > 0.01) 
		return;
	
	// Make sure we are really standing on a straight platform
	// Not on the underside of one and not falling down from it either!
	if (hit.moveDirection.y < -0.9 && hit.normal.y > 0.9) {
		activePlatform = hit.collider.transform;	
	}
}

// Various helper functions below:
function GetSpeed () {
	return movement.speed;
}

function GetVelocity () {
	return movement.velocity;
}


function IsMoving () {
	return movement.isMoving;
}

function IsJumping () {
	return jump.jumping;
}

function IsTouchingCeiling () {
	return (movement.collisionFlags & CollisionFlags.CollidedAbove) != 0;
}

function GetDirection () {
	return movement.direction;
}

function GetHangTime() {
	return movement.hangTime;
}

function Reset () {
	gameObject.tag = "Player";
}

function SetControllable (controllable : boolean) {
	canControl = controllable;
}

// Require a character controller to be attached to the same game object
@script RequireComponent (CharacterController)
@script AddComponentMenu ("2D Platformer/Platformer Controller")



