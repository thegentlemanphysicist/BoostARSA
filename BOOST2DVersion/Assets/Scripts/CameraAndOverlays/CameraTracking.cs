using UnityEngine;
using System.Collections;

public class CameraTracking : MonoBehaviour {
	
	
	GameObject player;
	public float cameraDistance;
	//public float horizontalOffset;
	float cameraHorizontalPosition;
	float cameraVerticalPosition;
	public static CameraTracking cameraTracking;

	Vector3 cameraPositionVector;



	//random screen shake variables
	float shakeAmplitude;
	bool randomShake=false;// is random shaking hapening? 
	float decreaseFactor=0.5f;
	float timeOfDetonation, explosionDelay=0.2f;

	//kick back variables
	bool shotKickBackHapening = false;
	float angleOfGun, kickBackReturnTime=3f, kickBackMagnitude, kickBackAmplitude = 2f;

	//wall hit 
	bool hitWall=false;
	float gammaFactorWithSign, wallHitRelaxationTime = 10f, wallHitDisplacement;
	float directionOfCollision;
	float gamma;


	//vertical camera jerk
	bool verticalColision= false;
	float verticalDisplacement, verticalDispDirection, verticalRelaxationTime = 10f;







	void Awake () 
	{
		//makes it a singleton
		if (cameraTracking == null)
		{
			//DontDestroyOnLoad(gameObject);
			cameraTracking = this;
		} else if (cameraTracking != this)
		{
			Destroy(gameObject);
		}
	
	}

	/*
	//what follows is to keep the camerafrom going off the screen.
	float screenWidth, screenHeight;
	float fieldOfViewRad;
	float yCameraRange,xCameraRange;
	public float xLeftBound, xRightBound, yBottomBound, yTopBound;
	float xCamMin,xCamMax, yCamMin, yCamMax;
*/



	// Use this for initialization
	void Start () {
		player =  PlatformerPushBodies.platformerPushBodies.gameObject;// GameObject.Find ("JennyAvatar");
		cameraDistance = -75.0f;

		//Try directing the camera at a slight angleLOOKS REALLY WEIRD
		//horizontalOffset = 10f;





		/*
		//what follows is to keep the camerafrom going off the screen.
		screenWidth = Screen.width;
		screenHeight= Screen.height;
		//the y position field of view
		fieldOfViewRad= 0.5f*camera.fieldOfView*Mathf.Deg2Rad;
		yCameraRange = cameraDistance* Mathf.Tan(fieldOfViewRad);
		xCameraRange = screenWidth*yCameraRange/screenHeight;
		xCamMin = xLeftBound  + xCameraRange;
		xCamMax = xRightBound - xCameraRange;
		yCamMin = yBottomBound+ yCameraRange;
		yCamMax = yTopBound   - yCameraRange;
	    */


	}
	
	// Update is called once per frame
	void Update () {

		cameraPositionVector =new Vector3(player.transform.position.x,
		                                  player.transform.position.y,
		                                  cameraDistance);


		if (shotKickBackHapening)
		{
			if (kickBackMagnitude > 0f)
			{
				cameraPositionVector += new Vector3(
					kickBackMagnitude*Mathf.Cos(angleOfGun),
					kickBackMagnitude*Mathf.Sin(angleOfGun),
					0f);
				kickBackMagnitude -= Time.deltaTime *kickBackReturnTime*kickBackAmplitude;
			} else
			{
				shotKickBackHapening = false;
			}
		}


		if (hitWall)
		{
			if (wallHitDisplacement> 0f)
			{
				cameraPositionVector += new Vector3(
					directionOfCollision*wallHitDisplacement,
					0f,
					0f);
				wallHitDisplacement -= wallHitRelaxationTime*Time.deltaTime;
			} else
			{
				hitWall = false;
			}
		}


		if (verticalColision)
		{
			if (verticalDisplacement>0f)
			{
				cameraPositionVector += new Vector3(0f,
				                                    -verticalDisplacement*directionOfCollision,
				                                    0f);
				verticalDisplacement -= verticalRelaxationTime*Time.deltaTime;
			} else 
			{
				verticalColision = false;
			}

		}


		//add this on last
		if (randomShake && Time.time>timeOfDetonation+explosionDelay ) 
		{
			if (shakeAmplitude>0)
			{
				cameraPositionVector+= Random.insideUnitSphere*shakeAmplitude;
				shakeAmplitude -= Time.deltaTime * decreaseFactor; 
			} else 
			{
				randomShake = false;
			}
		}






		if (player != null){
			transform.position= cameraPositionVector;//new Vector3(player.transform.position.x,//+horizontalOffset,
			                    //            player.transform.position.y,
			                    //            cameraDistance);
		} else if (PlatformerPushBodies.platformerPushBodies!=null)
		{
			player =  PlatformerPushBodies.platformerPushBodies.gameObject;
		}
		 
	
	
	}


	//screen kick functions
	public void BombShake( float distance)
	{
		//shake the screen with a size proportional to distance
		shakeAmplitude = 20f/distance;
		randomShake = true;
		timeOfDetonation = Time.time;
		decreaseFactor = 0.5f*(1f+10f/distance);
	}

	public void ShotKickBack(float angleOfGun2)
	{
		shotKickBackHapening = true;
		//kick back in direction oposite the angle of the gun
		angleOfGun = angleOfGun2;
		kickBackMagnitude = kickBackAmplitude;
	
	}

	public void WallCollisionKick()
	{
		if (!hitWall)
		{	
			gamma = PlatformerController.platformerController.movement.direction.x*
				LorentzContractionForJenny.lCJenny.gammaX;
			if (Mathf.Abs(gamma) >1.5f){
				hitWall = true;
				//gamma has a sign of direction, kick proportional to gamma factor 
				directionOfCollision = Mathf.Sign(gamma);
				wallHitDisplacement = Mathf.Min(Mathf.Abs(gamma)-1f, 5f);
			}
		}
	}

	public void HitFloorJerk(float velocityOfJenny, float directionOfCollision)
	{
		//Debug.Log("Hit floor speed="+velocityOfJenny);
		if (!verticalColision && Mathf.Abs(velocityOfJenny)>10f)
		{

			//some kind of jerk
			verticalColision = true;
			verticalDisplacement = 2f;
			verticalDispDirection= directionOfCollision;

		}

	}

	public void JumpJerk()
	{

	}





}
