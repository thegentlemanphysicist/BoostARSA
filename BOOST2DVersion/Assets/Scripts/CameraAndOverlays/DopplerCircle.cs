using UnityEngine;
using System.Collections;

public class DopplerCircle : MonoBehaviour {


	PhysicsConstants physicsConstants; 
	GameObject physConst;
	float SpeedOfLight;

	public Material greenMaterial;
	public Material redMaterial;






	float thetaScale = 0.1f;
	int size;
	int i;
	float radius = 10.0f, x,y;
	float thetaRange;

	float timeOfCircle;
	public float maxLifeOfCircle = 20.0f;
	bool circleExpanding=false;
	int theMaterialUsed;

	float baseWavelength= 400f;//get this from Jenny Shoot
	float newWavelength;
	float a;
	float b;




	LineRenderer[] lRend;

	// Use this for initialization
	void Start () {
		physConst = GameObject.Find ("PhysicalConstants");		
		physicsConstants = physConst.GetComponent("PhysicsConstants") as PhysicsConstants;
		SpeedOfLight=physicsConstants.SpeedOfLight;




		size =  (int)((2.0f*Mathf.PI)/thetaScale)+2;
		thetaRange = 2.0f*Mathf.PI+thetaScale;
		i=0;


		lRend = transform.GetComponents<LineRenderer>();
		lRend[0].SetVertexCount(size);


		timeOfCircle = Time.time;
		//GENERATE THE CIRCLE
		GiveCircleRadius(radius);

		/*for (float theta= 0.0f; theta < thetaRange; theta += thetaScale)
		{
			x = radius*Mathf.Cos( theta   );
			y = radius*Mathf.Sin( theta   );
			Vector3 pos = new Vector3(x,y,0.0f);
			lRend[0].SetPosition(i, pos);
			i++;
		}*/
		//GiveCircleRadius(radius);


	}
	








	// Update is called once per frame
	void Update () 
	{




	}








	void GiveCircleRadius(float r)
	{
		i=0;
		for (float theta= 0.0f; theta < thetaRange; theta += thetaScale)
		{
			x = r*Mathf.Cos( theta   );
			y = r*Mathf.Sin( theta   );
			Vector3 pos = new Vector3(x,y,0.0f);
			lRend[0].SetPosition(i, pos);
			i++;
		}
	}

	void GiveCircleSegmentsColour(float Vx)
	{
		//This script will give each segment of the circle a new colour dependant on its angle and velocity
	

		a =baseWavelength/Mathf.Sqrt(1-Mathf.Pow(Vx/SpeedOfLight,2f));
		b = a*Vx/SpeedOfLight;

		i=0;
		for (float theta= 0.0f; theta < thetaRange; theta += thetaScale)
		{
			newWavelength = a -b*Mathf.Cos( theta   );
			//lRend[0].SetPosition(i, pos);
			i++;
		}
	}





}
