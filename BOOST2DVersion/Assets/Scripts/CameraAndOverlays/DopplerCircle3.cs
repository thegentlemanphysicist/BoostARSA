using UnityEngine;
using System.Collections;

public class DopplerCircle3 : MonoBehaviour {
	public static DopplerCircle3 dopplerCircle3;

	public float speedTest;

	public Material spectrum, pulseColourMaterial;

	float rInner, rOuter, angleBelowHorizontal;
	float theta,deltaTheta;
	int numbOfSegments;
	int i;


	//GameObject physConst;
	//PhysicsConstants physicsConstants;
	float SpeedOfLight;
	float dopplerSoftening;
	float baseWavelength;

	//[Range(0f,30f)]
	//public float testSpeed=15f;



	float theUVxCoord;
	float lambdaF;
	float oneOverConv = 1/(830f-360f);
	float a, b, gammaX;//the dopler shift is a-b*cos(theta)

// these are for the doppler Circle
//	Vector3[] vertices;
//	Vector3[] normals;
//	Vector2[] uvMapping;
//	int[] triangles;
//	Color[] vertexColours;

//	MeshFilter dopplerMeshFilter;
//	Mesh mesh;

//now we need to spawn a mesh for the pulse colour
	float PCRadius, PCAngleWedge, deltaThetaPC;
	int PCNumSegments;

	Vector3[] PCvertices;
	Vector3[] PCnormals;
	Vector2[] PCuvMapping;
	int[] 	  PCtriangles;
	Color[]	  PCvertexColours;
	
	MeshFilter  PCMeshFilter;
	Mesh 		PCmesh;


	//JennyShoot jennyShoot;
	Transform jenny;

	void Awake () 
	{
		//makes it a singleton
		if (dopplerCircle3 == null)
		{
			dopplerCircle3 = this;
		} else if (dopplerCircle3 != this)
		{
			//Destroy(gameObject);
		}
		
	}





	void Start()
	{

		SpeedOfLight     = GameControl.control.SpeedOfLight;
		dopplerSoftening = GameControl.control.dopplerSoftening;
		baseWavelength   = GameControl.control.baseWavelength;

		//this will give us the player's transform 
		jenny      = PlatformerController.platformerController.transform;
		//jennyShoot = JennyShoot.jennyShoot;


	//Generate the doppler circle
		rInner=5f;
		rOuter=10f;
		angleBelowHorizontal = JennyShoot.jennyShoot.angleBelowHorizontal;


		numbOfSegments = 30;
		theta = 0f;
		deltaTheta = (Mathf.PI+2f*angleBelowHorizontal)/numbOfSegments;
		GameObject dopplerCircle = transform.FindChild("DopplerCircle").gameObject;
		//dopplerCircle.AddComponent<MeshFilter>();
		//dopplerCircle.AddComponent<MeshRenderer>();


		//	new GameObject("dopplerCircle",typeof(MeshFilter),typeof(MeshRenderer));
		//dopplerCircle.transform.parent = transform;
		//dopplerCircle.transform.localPosition = new Vector3(0f,0f,0.3f);


//		mesh = new Mesh();

//		vertices  = new Vector3[2*numbOfSegments + 2];// number of segments for a semicircle
//		normals   = new Vector3[2*numbOfSegments + 2];
//		uvMapping = new Vector2[2*numbOfSegments + 2];
//		triangles = new int[2*3*numbOfSegments];


	//Generate the mesh (an arch or donut

//		GenerateTheMesh();

		//to improve performance apply mesh optimise it orders the triangles gooder
//		mesh.Optimize();

//		dopplerMeshFilter = dopplerCircle.GetComponent<MeshFilter>();
//		dopplerMeshFilter.mesh = mesh;

//		MeshRenderer dopplerMeshRend = dopplerCircle.GetComponent<MeshRenderer>();
//		dopplerMeshRend.material= spectrum;


	//generate the pulse colour wedge
		PCNumSegments =10;
		PCRadius = 1.2f*rOuter;
		PCAngleWedge = Mathf.PI/18f;//10 degrees 
		deltaThetaPC = PCAngleWedge/PCNumSegments;


		GameObject pulseColour = dopplerCircle.transform.FindChild("PulseColour").gameObject;
		pulseColour.AddComponent<MeshFilter>();
		pulseColour.AddComponent<MeshRenderer>();
		//	new GameObject("pulseColour",typeof(MeshFilter),typeof(MeshRenderer));
		//pulseColour.transform.parent = dopplerCircle.transform;
		pulseColour.transform.localPosition = Vector3.zero;// this may not be correct

		
		PCmesh = new Mesh();
		
		PCvertices  = new Vector3[PCNumSegments+1];// number of segments for a semicircle
		PCnormals   = new Vector3[PCNumSegments+1];
		PCuvMapping = new Vector2[PCNumSegments+1];
		PCtriangles = new int[3*PCNumSegments];

		GenerateThePCMesh();

		PCMeshFilter  = pulseColour.GetComponent<MeshFilter>();
		PCMeshFilter.mesh = PCmesh;
		
		MeshRenderer PCMeshRend = pulseColour.GetComponent<MeshRenderer>();
		PCMeshRend.material= pulseColourMaterial;
	//must change this up 
		pulseColour.transform.localPosition += new Vector3(0f,0f,0.05f);//shift it it front of the doppler circle.


		jenny.SendMessage("AssignThePulseColour");
		//Send message AssignThePulseColour

	}







	/*void OnUpdateMesh(float xSpeed)
	{
		AssignTheUVMesh( xSpeed , baseWavelength);
		dopplerMeshFilter.mesh.uv = uvMapping;

	}*/



	

	/*
	void GenerateTheMesh()
	{
		theta = -angleBelowHorizontal;
		//This assigns the vertices of the triangles on the arch 
		for (i =0; i< 2*numbOfSegments + 2 ; i= i+2)
		{
			

			//odd on the outside of the circle
			vertices[i+1] = new Vector3(
				rOuter*Mathf.Cos(theta),
				rOuter*Mathf.Sin(theta),
				0f
				);
			
			
			//even on the inside of the circle
			vertices[i] = new Vector3(
				rInner*Mathf.Cos(theta),
				rInner*Mathf.Sin(theta),
				0f
				);
			theta+= deltaTheta;
			//i+=2;
		}


		for (i =0; i< 2*numbOfSegments + 2 ; i++)
		{
			normals[i] = new Vector3(0f,0f,-1f);
		}
		
		
		//I'll assign the triangles by doing 1 quad per loop//optimise the mesh later
		for (i =0; i< numbOfSegments ; i++)
		{
			triangles[6*i  ] = 2*i+1;
			triangles[6*i+1] = 2*i  ;
			triangles[6*i+2] = 2*i+3;
			triangles[6*i+3] = 2*i+3;
			triangles[6*i+4] = 2*i  ;
			triangles[6*i+5] = 2*i+2;
		}



		// Assign the UV maping from the spectrum texture to the arch
		//for (i =0; i< numbOfSegments +1; i++)
		//{
		//	uvMapping[2*i  ] = new Vector2(0.936f,0.05f);//inside coord
		//	uvMapping[2*i+1] = new Vector2(0.936f,0.91f);//outside coords
		//}

		AssignTheUVMesh(0f, baseWavelength); 

		
		
		mesh.vertices  = vertices;
		mesh.triangles = triangles;
		mesh.normals   = normals;
		mesh.uv		   = uvMapping;

	}

	*/




	/*
	//this will get c
	void AssignTheUVMesh(float Vx, float lambda0) 
	{



		//a=1f/(  Mathf.Sqrt(1-Mathf.Pow(Vx/SpeedOfLight,2.0f))  );
		//b=a*Mathf.Abs(Vx/SpeedOfLight);
		//With dopplerSmoothing
		gammaX = 1f/(  Mathf.Sqrt(1-Mathf.Pow(Vx/SpeedOfLight,2.0f))  );
		a = 1+dopplerSoftening*(gammaX-1);//a is now the softened gamma factor.  Still has the range [1,infinity]
		//a = 1f+dopplerSoftening*(gammaX-1f);
		//b = dopplerSoftening*gammaX*(Vx/SpeedOfLight);//removed the absolute value 
		b = a*Mathf.Sign(Vx) * Mathf.Sqrt(1-Mathf.Pow(a,-2f)) ;


		theta = -angleBelowHorizontal;
		for (i =0; i< numbOfSegments + 1 ; i++)
		{
			//calculate the dopler shift
			lambdaF = lambda0*( a - b*Mathf.Cos (theta)  );
			//lambdaF = lambda0;
			//convert that wavelength to the wavelentght to a uv coord
			if ( lambdaF>360f &&lambdaF < 830f)
			{
				theUVxCoord = (lambdaF-360f)*oneOverConv;
			} else if (lambdaF < 830f)
			{
				theUVxCoord = 0f; //not sure why this is not 1
			} else
			{
				theUVxCoord = 1f; //not sure why this is not 0
			}
			uvMapping[2*i  ] = new Vector2(theUVxCoord ,0.5f);//inside coord
			uvMapping[2*i+1] = new Vector2(theUVxCoord ,0.5f);//outside coords


			theta+= deltaTheta;


		}

	}
	*/



	void GenerateThePCMesh()
	{
		theta = -0.5f*PCAngleWedge;
		//assign the center pt
		PCvertices[0] = Vector3.zero;


		//This assigns the vertices of the triangles on the arch 
		for (i =1; i< PCNumSegments+1 ; i++)
		{
			
			
			//odd on the outside of the circle
			PCvertices[i] = new Vector3(
				PCRadius*Mathf.Cos(theta),
				PCRadius*Mathf.Sin(theta),
				0f
				);
			
			

			theta += deltaThetaPC;
			//i+=2;
		}
		
		
		for (i =0; i< PCNumSegments+1 ; i++)
		{
			PCnormals[i] = new Vector3(0f,0f,-1f);
		}
		
		
		//I'll assign the triangles by doing 1 quad per loop//optimise the mesh later
		for (i =0; i< PCNumSegments/2 ; i++)
		{
			PCtriangles[6*i  ] = 0;
			PCtriangles[6*i+1] = PCNumSegments -2*i    ;
			PCtriangles[6*i+2] = PCNumSegments -2*i - 1;
			PCtriangles[6*i+3] = PCNumSegments -2*i - 1;
			PCtriangles[6*i+4] = PCNumSegments -2*i - 2;
			PCtriangles[6*i+5] = 0;
		}
		
		
		
		// Assign the UV maping from the spectrum texture to the arch
//		for (i =0; i< PCNumSegments; i++)
//		{
//			uvMapping[i  ] = new Vector2(0.5f,0.5f);//inside coord
//
//		}
		
		//AssignTheUVMesh(0f, baseWavelength); 
		
		
		
		PCmesh.vertices  = PCvertices;
		PCmesh.triangles = PCtriangles;
		PCmesh.normals   = PCnormals;
		PCmesh.uv		   = PCuvMapping;
		
	}
	

		








}
