using UnityEngine;
using System.Collections;
//using UnityEngine.UI;

public class DopplerRudolfRing : MonoBehaviour {

	public float speedTest;
	
	public Material spectrum;


	public GUISkin slidderSkin, skidderSkinHeader;
	float rInner, rOuter;
	float theta,deltaTheta;
	int numbOfSegments;
	int i;
	
	

	float SpeedOfLight;
	float dopplerSoftening;
	public float baseWavelength;


	[Range(0f,99.99999f)]
	public float percentSpeedLight;
	
	
	
	float theUVxCoord;
	float lambdaF;
	float oneOverConv = 1/(830f-360f);
	float a, b, gammaX;//the dopler shift is a-b*cos(theta)
	
	
	
	float screenWidth;
	float screenHeigth;
	
	
	Rect slidderRect, backingRect, slidderRectLam, backingRectLam;
	Rect labelRecLam, labelRecSpeed;


	Vector3[] vertices;
	Vector3[] normals;
	Vector2[] uvMapping;
	int[] triangles;
	Color[] vertexColours;
	
	
	
	MeshFilter dopplerMeshFilter;
	Mesh mesh;
	
	Transform rudolfTrans;
	float rudolfXscale;
	float nosescale0;
	
	void Start()
	{

		SpeedOfLight     = 30f;

		
		rudolfTrans=transform.parent.transform;
		rudolfXscale = rudolfTrans.localScale.x;
		nosescale0 = transform.localScale.x;
		//baseWavelength = 800f;
		
		rInner=0.75f;

		rOuter=1.25f;
		
		numbOfSegments = 60;
		theta = 0f;
		deltaTheta = 2f*Mathf.PI/numbOfSegments;
		GameObject dopplerCircle = new GameObject("dopplerCircle",typeof(MeshFilter),typeof(MeshRenderer));
		dopplerCircle.transform.parent = transform;
		dopplerCircle.transform.localPosition = Vector3.zero;
		
		
		mesh = new Mesh();
		
		vertices  = new Vector3[2*numbOfSegments + 2];// number of segments for a semicircle
		normals   = new Vector3[2*numbOfSegments + 2];
		uvMapping = new Vector2[2*numbOfSegments + 2];
		triangles = new int[2*3*numbOfSegments];
		
		
		//Generate the mesh (an arch or donut
		
		GenerateTheMesh();
		
		//Colour the mesh.
		//ColourTheMesh();
		//mesh.colors = vertexColours;
		
		
		//to improve performance apply mesh optimise it orders the triangles gooder
		mesh.Optimize();
		
		dopplerMeshFilter = dopplerCircle.GetComponent<MeshFilter>();
		dopplerMeshFilter.mesh = mesh;
		
		MeshRenderer dopplerMeshRend = dopplerCircle.GetComponent<MeshRenderer>();
		dopplerMeshRend.material= spectrum;
		
		
		screenHeigth = Screen.height;
		screenWidth  = Screen.width;
	
	
		dopplerCircle.transform.parent = null;
	




		slidderRect = new Rect(0.1f*screenWidth, 0.15f*screenHeigth, 40, 0.4f*screenHeigth);

		backingRect = new Rect(0.1f*screenWidth-15, 0.12f*screenHeigth, 40, 0.46f*screenHeigth);
	
		slidderRectLam = new Rect(0.9f*screenWidth, 0.15f*screenHeigth, 40, 0.4f*screenHeigth);

		backingRectLam = new Rect(0.9f*screenWidth-15, 0.12f*screenHeigth, 40, 0.46f*screenHeigth);

		labelRecSpeed   = new Rect(0.1f*screenWidth-15-40, 0.58f*screenHeigth+50+15, 120, 60);
		labelRecLam = new Rect(0.9f*screenWidth-15-50, 0.58f*screenHeigth+40+15, 140 , 60);

	}

	
	float onsliderinput(float input2)
	{
		return 1f;
	}
	
	
	
	
	
	void Update()
	{

		speedTest = -percentSpeedLight*SpeedOfLight/100f;
		AssignTheUVMesh( speedTest , baseWavelength);
		dopplerMeshFilter.mesh.uv = uvMapping;

		//rudolfTrans.localScale= new Vector3(
		//	rudolfXscale*(  Mathf.Sqrt(1-Mathf.Pow(speedTest/SpeedOfLight,2.0f))  ),
		//	rudolfTrans.localScale.y,
		//	rudolfTrans.localScale.z);
		transform.localScale = new Vector3( 
		            nosescale0* Mathf.Sqrt(1-Mathf.Pow(speedTest/SpeedOfLight,2.0f)),
		                                   transform.localScale.y,
		                                   transform.localScale.z);


	}
	
	
	void OnGUI()
	{
		GUI.skin=skidderSkinHeader;

		GUI.Box(new Rect(0.25f*screenWidth, 15 ,0.5f*screenWidth , 60),
		        "Rudolph The Relativistic Reindeer");



		GUI.skin=slidderSkin;
		//Rect slider = new Rect (50, 30, 100, 30); 
		//hSliderValue = GUI.HorizontalSlider(slider,hSliderValue,1.0f,100.0f);
		//percentSpeedLight = GUI.VerticalSlider(new Rect(500,  400, 30, 500), 0f, 10.0F, 0.0F);
		GUI.Box(backingRect, "");
		percentSpeedLight = GUI.VerticalSlider(
			slidderRect, percentSpeedLight, 99.999f, 0.0f);
	
	
		GUI.Box(backingRectLam, "");
		baseWavelength = GUI.VerticalSlider(
			slidderRectLam, baseWavelength, 100f, 1000f);

		GUI.Box(labelRecSpeed, "Speed\n="+ ( (int)(100*percentSpeedLight)*0.01f    ).ToString ()+"%c");
		GUI.Box(labelRecLam, "Wavelength\n="+((int)baseWavelength).ToString()+"nm");

		GUI.Box(new Rect(0.9f*screenWidth-15-65, 0.58f*screenHeigth+125, 170 , 60),
		        "Colour of nose\nat rest.");


		GUI.Box(new Rect(0.1f*screenWidth-15-65, 0.58f*screenHeigth+125, 170 , 60),
		        "Rudolph's speed\nto the left.");


		if (GUI.Button (new Rect(screenHeigth*0.05f,screenHeigth*0.05f,screenHeigth*0.1f,screenHeigth*0.1f), "Reset"))
	   {
			baseWavelength = 615f;
			percentSpeedLight = 0f;
		};



		    //                , "Info")){
		//if (GUI.Button (new Rect((screenWidth-smallButtonWidth)*0.5f-smallButtonWidth*0.55f, screenHeight*0.2f, smallButtonWidth, smallButtonHeight )
		//                , "Info")){
			//Go back to menu
			//GUI.Box();
		//}
	}

	

















	
	void GenerateTheMesh()
	{
		
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
		/*for (i =0; i< numbOfSegments +1; i++)
		{
			uvMapping[2*i  ] = new Vector2(0.936f,0.05f);//inside coord
			uvMapping[2*i+1] = new Vector2(0.936f,0.91f);//outside coords
		}//*/
		
		AssignTheUVMesh(0f, baseWavelength); 
		
		
		
		mesh.vertices  = vertices;
		mesh.triangles = triangles;
		mesh.normals   = normals;
		mesh.uv		   = uvMapping;
		
	}
	
	
	
	
	
	
	
	//this will get c
	void AssignTheUVMesh(float Vx, float lambda0) 
	{
		
		
		
		a=1f/(  Mathf.Sqrt(1-Mathf.Pow(Vx/SpeedOfLight,2.0f))  );
		b=a*(Vx/SpeedOfLight);
		//With dopplerSmoothing
		//gammaX = 1f/(  Mathf.Sqrt(1-Mathf.Pow(Vx/SpeedOfLight,2.0f))  );
		//a = 1f+dopplerSoftening*(gammaX-1f);
		//b = dopplerSoftening*gammaX*(Vx/SpeedOfLight);//removed the absolute value 
		
		
		
		theta = 0f;
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




}
