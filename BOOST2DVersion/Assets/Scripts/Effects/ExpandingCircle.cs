using UnityEngine;
using System.Collections;
//using Vectrosity;


public class ExpandingCircle : MonoBehaviour {



	//VectorLine circleVec;



	//PhysicsConstants physicsConstants; 
	//GameObject physConst;
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






	LineRenderer[] lRend;

	// Use this for initialization
	void Start () {
		//physConst = GameObject.Find ("PhysicalConstants");		
		//physicsConstants = physConst.GetComponent("PhysicsConstants") as PhysicsConstants;
		SpeedOfLight  = GameControl.control.SpeedOfLight;
		//SpeedOfLight=physicsConstants.SpeedOfLight;

		/*
		VectorLine.SetCanvasCamera(Camera.main);
		circleVec = VectorLine.SetLine3D (Color.green, Vector3.zero ,// transform.position, 
		                                   new Vector3(10f,10f, 0f));
		circleVec.material = greenMaterial;
		circleVec.SetWidth(10f);

		circleVec.MakeCircle(transform.position, 20f);
		circleVec.Draw3D();
		*/


		size =  (int)((2.0f*Mathf.PI)/thetaScale)+2;
		thetaRange = 2.0f*Mathf.PI+thetaScale;
		i=0;


		lRend = transform.GetComponents<LineRenderer>();
		lRend[0].SetVertexCount(size);


		timeOfCircle = Time.time;
		//GENERATE THE CIRCLE


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
	






	//this will set the material to green and spawn it
	void Spawn()
	{
		//lRend[0].material = greenMaterial;
		circleExpanding =true;
		//theMaterialUsed =0;
	}





	// Update is called once per frame
	void Update () 
	{
		if (circleExpanding)
		{
			if (Time.time-timeOfCircle <maxLifeOfCircle) 
			{
				GiveCircleRadius(  (Time.time-timeOfCircle)*SpeedOfLight );
			} else 
			{
				Destroy(gameObject);
			}

		}
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







}
