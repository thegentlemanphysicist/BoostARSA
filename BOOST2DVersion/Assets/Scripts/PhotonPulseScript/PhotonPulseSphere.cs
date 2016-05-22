using UnityEngine;
using System.Collections;


[RequireComponent(typeof(AudioSource))]
public class PhotonPulseSphere : MonoBehaviour {
	
	
	/// <summary>
	///This controls the laser. 
	/// Will need: Physical Constants, A controller,
	/// It will also make it partially transparent until it is clear of jenny, or being absorbed by an opaque object.
	/// This controls sphere pulses NO MESH STUFF NEEDED
	/// </summary>
	
	//The base colour of laser is:
	Color initialColor;
	//This is the colour after redshifting:
	Color shiftedColor;
	
	public float sonicDopplerShift;

	//public float numberOfCollumnsVisible;
	
	//PhysicsConstants physicsConstants; 
	//GameObject physConst;
	//float SpeedOfLight;
	public float SpeedOfPulse;
	float pulseEmissionTime;
	float cX,cY; //speed of light in x and y direction
	float oldSpeedSquared;
	
	public float wavelength;
	public string nameOfShooter;
	

	float gammaX;
	Vector3 startingposition;
	float direction;
	public Vector3 itsvelocity;
	public float speedOfPulse;
	
	
	////////////////////////////////////////////////////////////
	////we need some variables for transparency of a mesh.  ////
	////////////////////////////////////////////////////////////
	MeshRenderer pulseMeshRend;
	//Vector3[] mVertices;
    //Vector3[] mNormals;
	public Mesh mesh;
	
    public int[] newTriangles;
	public float widthOfMeshTriangle;
	
	public Transform gun;
	public Vector3 gunPosition;

	
	//Has the pulse cleared the gun
	public bool clearOfGun;
	public float gunPulseSeparation;
	bool[] collumnVisible;
	public int numberOfCollums;
	
	
	//for the destruction of the pulse
	bool pulseGettingDestroyed;
	float timeDestroyed, delayInDestroy=3.0f;


	//These variables are for the pulse bouncing off a mirror
	//called in the function ReflectPulse()
	float incomingVelX, incomingVely, outgoingVelX, outgoingVelY
		,outgoingVelXTemp, outgoingVelYTemp;
	float cosZRot, sinZRot;
	public float exitAngle, tempSpeed, correctSpeed, ratioOfSpeeds;
	float scaledTempSpeed;

	Transform haloTransform;



	
	// Use this for initialization
	void Start () {

		
		//physConst = GameObject.Find ("PhysicalConstants");		
		//physicsConstants = physConst.GetComponent("PhysicsConstants") as PhysicsConstants;
		cX = GameControl.control.SpeedOfLight;
		cY = GameControl.control.SpeedOfLightY;

		
		gun = GameObject.FindWithTag("Gun").transform;
		gunPosition = new Vector3(gun.position.x, 0.0f,0.0f);
		clearOfGun = false;
		
		//This gets called if the pulse is getting destroyed.
		pulseGettingDestroyed = false;

		haloTransform = transform.FindChild("halo");
	
		
	}
	
	void PlayShotSound()
	{
		GetComponent<AudioSource>().pitch = GetComponent<AudioSource>().pitch/sonicDopplerShift;
		GetComponent<AudioSource>().Play();
	}
	
	
	
	/*void OnCollisionEnter (Collision hit){
		Debug.Log("The photon hit something");
	}*/
	
	void OnTriggerEnter2D (Collider2D hit){
		//Debug.Log("The photon hit something");
		//hit.gameObject.SendMessage("OnHitByPulse()",SendMessageOptions.DontRequireReceiver);
		//+ wavelength +","+ nameOfShooter +")"
		if (hit.gameObject.name == "MirrorSurface") {
			Debug.Log("The photon hit the mirror");
			//make a function that is called to reflect the pulse
			ReflectPulse(hit);
			//once the pulse is reflected it should be lethal to jenny




		} else if (  hit.gameObject.name != "GunBarrell" && !hit.isTrigger   
		    && hit.gameObject.name != "JennyAvatar"){
			//destroy or deactivate the collider so one photon can't kill two things
			Destroy(transform.GetComponent<Collider2D>());
			haloTransform.gameObject.SetActive(false);
			transform.GetComponent<Renderer>().enabled= false;
			//initiate the process of invisbling the photon
			transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			pulseGettingDestroyed=true;
			timeDestroyed = Time.time;

			
			hit.gameObject.SendMessage("OnHitByPulse",wavelength,
			                           SendMessageOptions.DontRequireReceiver);
			if (hit.transform.parent != null)
			{
				hit.transform.parent.SendMessage("OnHitByPulse",wavelength,
			                                 SendMessageOptions.DontRequireReceiver);
			}
			
		}
		
	/*	
		hit.gameObject.SendMessage("OnHitByPulse",wavelength,
			SendMessageOptions.DontRequireReceiver);

		hit.transform.parent.SendMessage("OnHitByPulse",wavelength,
		                           SendMessageOptions.DontRequireReceiver);
	*/	
	}
	
	
	
	
	// Update is called once per frame
	void Update () {
		
		//MAKE THE PULSE VISIBLE
		if (!clearOfGun && transform != null ){
			//This calculates the separation between the pulse and the gun
			//gunPosition = new Vector3(gun.localPosition.x,0.0f,0.0f);
			//gunPulseSeparation = Vector3.Distance(transform.position,gunPosition);
			gunPulseSeparation = Vector3.Distance( new Vector3(transform.position.x ,0.0f,0.0f),
				new Vector3(gun.position.x ,0.0f,0.0f)  );
			
			//Debug.Log("localPosition of gun is = " + gun.localPosition.x);
			//Debug.Log("the separation is = " + gunPulseSeparation);
			//Debug.Log("the gun position is      = " + gun.position.x);
			
			if (gunPulseSeparation> transform.localScale.x) {
				clearOfGun = true;
			}
		} 
		
		
		
		//MAKE THE PULSE INVISIBLE
		if (pulseGettingDestroyed){
		//On collision with an opaque body. The pulse gets turned off.

			if (Time.time > timeDestroyed+delayInDestroy){
			//if it is made completely ivisible destroy the pulse
				Destroy(gameObject);
			}
		
		
		}
		
		
		
	}
	
	
	//This function reflects the pulses of laser light shot into mirrors.
	void ReflectPulse(Collider2D mirrorWeHit){


		//get the angle and goal speed of the mirror
		float zRotation;
		//This is the angle of the mirrors normal in world space 
		zRotation =	mirrorWeHit.transform.eulerAngles.z;

		//Debug.Log ("The zRotation is"+zRotation);
		//if the angle of the mirror is 0, switch the x component of the velocity
		//however using dot product identities we can now do the completely general case

		cosZRot = Mathf.Cos(Mathf.Deg2Rad*zRotation);
		sinZRot = Mathf.Sin(Mathf.Deg2Rad*zRotation);
		//The vector components of the velocity of the pusle coming into the mirror
		incomingVelX = transform.GetComponent<Rigidbody2D>().velocity.x;
		incomingVely = transform.GetComponent<Rigidbody2D>().velocity.y;

		//These will give the correct direction of the pulse leaving the mirror,
		//but not the correct speed.  They chance the sign of the component of 
		//the velocity parallel to tmirrors normal
		outgoingVelXTemp = incomingVelX-2.0f*(incomingVelX*cosZRot+incomingVely*sinZRot)*cosZRot;
		outgoingVelYTemp = incomingVely-2.0f*(incomingVelX*cosZRot+incomingVely*sinZRot)*sinZRot;



		//to get the final speed, get a ratio of the final speed to the previous speed and scale both components
		tempSpeed = Mathf.Sqrt (Mathf.Pow(outgoingVelXTemp,2.0f) + Mathf.Pow(outgoingVelYTemp,2.0f));
		//The correct speed is cos(theta)*speeLightX+sin(theta)*speedLightY;
		//use cos(theta)=tempSpeedX/TempSpeed
		//Theta is messyier, there is a scalefactor on the speed of light
		//check JennyShoot.cs for details on speed.  The correct speed comes from a basic derivation from 
		//vector and trig identities.

		oldSpeedSquared = (Mathf.Pow(outgoingVelXTemp/cX,2.0f) +Mathf.Pow(outgoingVelYTemp/cY,2.0f));
		//this check needs to be done because of edge cases with hitting through colliders into another
		if ( oldSpeedSquared >0.0f)
		{
			correctSpeed = Mathf.Sqrt ( (Mathf.Pow(outgoingVelXTemp,2.0f) +Mathf.Pow(outgoingVelYTemp,2.0f)) /
			                 oldSpeedSquared );


			ratioOfSpeeds= correctSpeed/tempSpeed;
			//Since the outgoing angle of the pulse is correct, we need to scale both components of the velocoty
			//by the same amount
			outgoingVelX = ratioOfSpeeds*outgoingVelXTemp;
			outgoingVelY = ratioOfSpeeds*outgoingVelYTemp;


			//The final velocity is a vector
			//Debug.Log("The outgoing velocity X is =" + outgoingVelX);
			transform.GetComponent<Rigidbody2D>().velocity = new Vector2 ( outgoingVelX, outgoingVelY); 


		}





		//how does it reflect things when a mirror is moving?
	}
	

	
	
	

	

}
