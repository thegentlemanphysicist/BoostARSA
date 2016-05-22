using UnityEngine;
using System.Collections;
using UnityEngine.UI;





 
public class LorentzContractionForJenny : MonoBehaviour {
	
	public static LorentzContractionForJenny lCJenny; 
	public bool canShoot=true;

	Transform jennyTransform;
	
	
	
	//float xScale;
	float xScale0;
	float yScale0;
	float zScale0;
 	public float gammaX;
	public float VxNew;
	public Vector3 VxNew2;
	//Vector3 lastposition, lastposition2, lastposition3;


	//Doppler Circle
	GameObject shotRecovery;
	public float gammaXOld2 = 0f;// this will be used to keep a new mesh on the doppler sircle from being assigned every frame;
	bool gamma1checked;


	//New doppler circle control
	Material dopplerCircleMaterial;
	float softGamma, dopplerSoftening;
	//RawImage dopplerCircle;

	
	//PhysicsConstants physicsConstants;
	float SpeedOfLight;
	//GameObject physConst;
	PlatformerController platformerController;

	void Awake () 
	{
		//makes it a singleton
		if (lCJenny == null)
		{
			lCJenny = this;
		} else if (lCJenny != this)
		{
			Destroy(gameObject);
		}
		
	}




	// Use this for initialization
	void Start () {
		
		
		jennyTransform = PlatformerPushBodies.platformerPushBodies.transform;

		
		shotRecovery = DopplerCircle3.dopplerCircle3.gameObject;
			
		gamma1checked=false;


		///This lets me get the x speed	
		platformerController = PlatformerController.platformerController; 
			



		SpeedOfLight  = GameControl.control.SpeedOfLight;// physicsConstants.SpeedOfLight;
		dopplerSoftening = GameControl.control.dopplerSoftening;


		//assign the doppler circle material
		dopplerCircleMaterial = shotRecovery.transform.FindChild("DopplerCircle").FindChild("DopplerDialTest").GetComponent<RawImage>().material;
		//dopplerCircle = shotRecovery.transform.FindChild("DopplerCircle").FindChild("DopplerDialTest").GetComponent<RawImage>();


	/*	
		lastposition=transform.position;
		lastposition2=transform.position;
		lastposition3=transform.position;
	*/	
		
		
		//xScale = transform.localScale.x;// = 10.0f;
		//Transform mytransform = transform;
		//Scale3Vector0 = transform.localScale;
		
		xScale0= jennyTransform.localScale.x;
		yScale0= jennyTransform.localScale.y;
		zScale0= jennyTransform.localScale.z;
		
//		transform.localScale = new Vector3( 10.0f, yScale0 , zScale0 );
		
		gammaX = 1.0f;
		
	//	zposition = transform.position.z;
//		Scale3Vector0.Set ( 10.0f , yScale0, zScale0 );	
		//mytransform.localScale.Set (10.0f,10.0f,10.0f);
	
	}
	/*
	This did not smooth the velocity out enough
	Vector3 getSmoothVelocity () {
		Vector3 V=(lastposition3-lastposition2+lastposition-
					transform.position)/(2.0f*Time.deltaTime);
		lastposition3=lastposition2;
		lastposition2=lastposition;
		lastposition=transform.position;
		return V;
	}
	*/
	
	
	
	public float GetXVelocity () {
		float Vx = platformerController.GetSpeed();
		//float Vx = rigidbody.velocity.x;
		return Vx;// Vx is the float returned by GetXVelocity
		
	}
	//Gammax is the gamma factor of the object this script is applied to
	float GetGammaX ( float Vcurrent ) {
		float gammaX1 = 1/ Mathf.Sqrt( 1.0f - Mathf.Pow( Vcurrent / SpeedOfLight, 2 )  );
		return gammaX1;
	}
	

	//void FixedUpdate () {
	//	zposition = 0.0f;//new Vector3(0, 0, 0);
	//	rotationOfBody  = new Vector3(0,0,0);
	//}
	
	
	// float CVx = GetXV();
	// Update is called once per frame
	
	

	
	void Update () {
		
	//	if (!rigidbody.isKinematic){ //This keeps carried objects from L.C. twice	
			VxNew = GetXVelocity ();
	//		VxNew2= -(lastposition - transform.position)/Time.deltaTime;
	//		lastposition = transform.position;
			//if (VxNew != Vx) {
			if ( Mathf.Abs(VxNew) <SpeedOfLight){
				gammaX = GetGammaX ( Mathf.Abs(VxNew) );
				jennyTransform.localScale = new Vector3 ( xScale0/gammaX , yScale0, zScale0 );
			}
			//	Vx=VxNew;
			//}

	//	}
		//this will change the update the mesh of the doppler circle, should not be called every frame
		if(canShoot)
		{
			if (Mathf.Abs (gammaX- gammaXOld2)> 0.05f)
			{
				//Don't use this one any more
				//shotRecovery.SendMessage("OnUpdateMesh", VxNew*platformerController.movement.direction.x ,
				 //                        SendMessageOptions.DontRequireReceiver);


				//calculate the softened gamma factor
				//softGamma = gammaX;
				softGamma = 1+dopplerSoftening*(gammaX-1);
				//alter the doppler circle accordingly
				dopplerCircleMaterial.SetFloat("_softGamma", softGamma );
				dopplerCircleMaterial.SetFloat("_directionOfJenny", Mathf.Sign(platformerController.movement.direction.x));
				gammaXOld2= gammaX;
				gamma1checked = false;
			} else if (gammaX < 1.05f && !gamma1checked)
			{
				//shotRecovery.SendMessage("OnUpdateMesh",0f,
				//                         SendMessageOptions.DontRequireReceiver);


				//calculate the softened gamma factor
				//softGamma = gammaX;
				softGamma = 1+dopplerSoftening*(gammaX-1);
				//alter the doppler circle accordingly
				dopplerCircleMaterial.SetFloat("_softGamma", softGamma );
				dopplerCircleMaterial.SetFloat("_directionOfJenny", Mathf.Sign(platformerController.movement.direction.x));

				gamma1checked = true;
			}
		}



	}



}
