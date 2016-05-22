using UnityEngine;
using System.Collections;

public class PhotonPulse : MonoBehaviour {
	
	
	/// <summary>
	///This controls the laser. 
	/// Will need: Physical Constants, A controller,
	/// It will also make it partially transparent until it is clear of jenny, or being absorbed by an opaque object.
	/// </summary>
	
	//The base colour of laser is:
	Color initialColor;
	//This is the colour after redshifting:
	Color shiftedColor;
	
	//Transform endPoint;
	public float numberOfCollumnsVisible;
	
	//PhysicsConstants physicsConstants; 
	//GameObject physConst;
	//float SpeedOfLight;
	public float SpeedOfPulse;
	float pulseEmissionTime;
	
	
	
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
	Transform spawnGlow;
	
	//Has the pulse cleared the gun
	public bool clearOfGun;
	public float gunPulseSeparation;
	bool[] collumnVisible;
	public int numberOfCollums;
	
	
	//for the destruction of the pulse
	bool pulseGettingDestroyed;
	float timeOfPulseCollision, ratioMadeInvis, numOfColsToMakeInvis;
	
	
	// Use this for initialization
	void Start () {
		//endPoint = transform.Find("EndPoint").transform;
		
		//physConst = GameObject.Find ("PhysicalConstants");		
		//physicsConstants = physConst.GetComponent("PhysicsConstants") as PhysicsConstants;
		//SpeedOfLight=physicsConstants.SpeedOfLight;
		
		gun = GameObject.FindWithTag("Gun").transform;
		gunPosition = new Vector3(gun.position.x, 0.0f,0.0f);
		clearOfGun = false;
		
		//This gets called if the pulse is getting destroyed.
		pulseGettingDestroyed = false;
		timeOfPulseCollision= 0.0f;
		ratioMadeInvis = 0.0f;
		numOfColsToMakeInvis = 0.0f;
		
		
		
		//This will be the glow as pulse is spawned
		spawnGlow= transform.FindChild("EndPoint");
		
		
		//wavelength = 1.0f;
		//transform.rigidbody.velocity = new Vector3(SpeedOfLight,0.0f,0.0f);
		
		//pulseMeshRend = gameObject.GetComponent("MeshRenderer") as MeshRenderer;
		mesh = transform.GetComponent<MeshFilter>().mesh;
    	
		//This is mostly to make sure newTriangles is the correct size
		newTriangles = new int[ mesh.triangles.Length ];// GetTriangles(0);	
		numberOfCollums = mesh.triangles.Length/6;
		//Debug.Log("Number of collums="+numberOfCollums);
		//This tells us which collums start visible
		collumnVisible = new bool[numberOfCollums];//since all false, don't need  to initialize values.
		
	//This should define the triangle array so removing the elements can be don collumn by collumn
	//Initialized in such a way that they are not visible to the camera
		//The first 30 entries are the bottom half of triangles
		for(int i=0;i<numberOfCollums;i++){
			newTriangles[3*i  ] = i ;	
			newTriangles[3*i+1] = i+1;
			newTriangles[3*i+2] = numberOfCollums + 1 + i;	
		}
	
		//The last 30 entries are the upper half of triangles
		for(int i=0;i< numberOfCollums ;i++){
			newTriangles[3*(i+numberOfCollums)    ] = numberOfCollums+2+i ;	
			newTriangles[3*(i+numberOfCollums) + 1] = numberOfCollums+1+i   ;
			newTriangles[3*(i+numberOfCollums) + 2] = i+1 ;	
		}
		
		mesh.triangles=newTriangles;
		
		
		
	}
	
	





	//The pulse needs to be reflected.
	void OnReflect (float angleOfMirror) {
		// to start just flip the x component of the velocity

	}



	
	
	/*void OnCollisionEnter (Collision hit){
		Debug.Log("The photon hit something");
	}*/
	
	void OnTriggerEnter (Collider hit){
		//Debug.Log("The photon hit something");
		//hit.gameObject.SendMessage("OnHitByPulse()",SendMessageOptions.DontRequireReceiver);
		//+ wavelength +","+ nameOfShooter +")"
		if (hit.gameObject.name != "JennyAvatar"  && hit.gameObject.name != "MirrorSurface"){
			//destroy or deactivate the collider so one photon can't kill two things
			Destroy(transform.GetComponent<Collider>());
			//initiate the process of invisbling the photon
			pulseGettingDestroyed=true;
			timeOfPulseCollision = Time.time;
			
			
		}
		
		
		hit.gameObject.SendMessage("OnHitByPulse",wavelength,
			SendMessageOptions.DontRequireReceiver);
		
	}
	
	
	
	
	// Update is called once per frame
	void Update () {
		
		//MAKE THE PULSE VISIBLE
		if (!clearOfGun){
			//This calculates the separation between the pulse and the gun
			//gunPosition = new Vector3(gun.localPosition.x,0.0f,0.0f);
			//gunPulseSeparation = Vector3.Distance(transform.position,gunPosition);
			gunPulseSeparation = Vector3.Distance( new Vector3(transform.position.x ,0.0f,0.0f),
				new Vector3(gun.position.x ,0.0f,0.0f)  );
			
			//Debug.Log("localPosition of gun is = " + gun.localPosition.x);
			//Debug.Log("the separation is = " + gunPulseSeparation);
			//Debug.Log("the gun position is      = " + gun.position.x);
			
			if (gunPulseSeparation< transform.localScale.x) {
				numberOfCollumnsVisible = numberOfCollums*gunPulseSeparation/
					(transform.localScale.x);
				for (int i=0; i<numberOfCollumnsVisible ; i++){
					if (!collumnVisible[numberOfCollums-1-i]){
						MakeACollumnOfMeshVisible( numberOfCollums-1-i );
						collumnVisible[numberOfCollums-1-i]=true;
					}
				}	

			//When we've finished messing with newTriangles update the mesh 
				mesh.triangles = newTriangles;
	
			//The glow should be at the back end of the visible pulse				
				spawnGlow.localPosition = new Vector3( 
					-numberOfCollumnsVisible/numberOfCollums   ,0.0f ,0.0f);
			} else {
				//Run this once when the pulse is clear to make sure it's visible
				for (int i=0; i<numberOfCollums ; i++){
					if (!collumnVisible[numberOfCollums-1-i]){
						MakeACollumnOfMeshVisible( numberOfCollums-1-i );
						collumnVisible[numberOfCollums-1-i]=true;
					}
				}
				//When we've finished messing with newTriangles update the mesh 
				mesh.triangles = newTriangles;
				Destroy(spawnGlow.gameObject);
				clearOfGun = true;
			}
		} 
		
		
		
		//MAKE THE PULSE INVISIBLE
		if (pulseGettingDestroyed){
		//On collision with an opaque body. The pulse gets turned off.
			//Figure out what fraction is invisible
			ratioMadeInvis = speedOfPulse*(Time.time-timeOfPulseCollision)
				/transform.localScale.x;
			
			
			//if it is made completely ivisible destroy the pulse
			if(ratioMadeInvis>1.0f){
				Destroy(gameObject);
			}
			
			numOfColsToMakeInvis = ratioMadeInvis*numberOfCollums;
			//use a for loop to make that fraction invisible
			for (int i =0; i < numOfColsToMakeInvis-1; i++){
				
				//temptemp=numberOfCollums-i-1;
				//Debug.Log("The number is" + temptemp );
				if (numberOfCollums-1-i>=0 && collumnVisible[numberOfCollums-1-i]){
						MakeACollumnOfMeshInvisible( numberOfCollums-1-i );
						collumnVisible[numberOfCollums-1-i]=false;
				}
			}
				
				
			
			
			//assign the new triangles to the mesh.
			mesh.triangles = newTriangles;
			
		
		
		}
		
		
		
	}
	
	
	
	
	
	
	
	void MakeACollumnOfMeshVisible(int i ){
	//i is the collumn number counted from RIGHT to LEFT  (<----)
		//Just use the same routine used to fill the triangle mesh with 
		//two vertices swapped
		//Make the upper triangle visible	
		newTriangles[3*i  ] = i+1 ;	
		newTriangles[3*i+1] = i;
		//	newTriangles[3*i+2] = 11 + i;	
		
		
		//Make the lower triangle visible
		newTriangles[3*(i+numberOfCollums)   ] = numberOfCollums+1+i   ;	
		newTriangles[3*(i+numberOfCollums)+1 ] = numberOfCollums+2+i;
		//	newTriangles[3*i+2 + 30] = i+1 ;	
		
		
	}
	
	
	void MakeACollumnOfMeshInvisible(int i ){
	//i is the collumn number counted from RIGHT to LEFT  (<----)
		//Just use the same routine used to fill the triangle mesh with 
		//two vertices swapped
		//Make the upper triangle visible	
		newTriangles[3*i  ] = i ;	
		newTriangles[3*i+1] = i+1;
		//	newTriangles[3*i+2] = 11 + i;	
		
		
		//Make the lower triangle visible
		newTriangles[3*(i+numberOfCollums)   ] = numberOfCollums+2+i   ;	
		newTriangles[3*(i+numberOfCollums)+1 ] = numberOfCollums+1+i;
		//	newTriangles[3*i+2 + 30] = i+1 ;	
		
		
	}
}
