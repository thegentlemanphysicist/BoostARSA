using UnityEngine;
using System.Collections;

public class Explosion2DRaycasting : MonoBehaviour {

	int numbOfRaycasts= 36;//scan every 10 degrees in the base case
	RaycastHit2D[] hits; 

	float intToRad, intToDeg;
	float lifeTimeTemp,lifeTimeOfParts;


	public ParticleSystem theExplosionParticles;
	ParticleSystem theExplosionParticlesClone;
	//Quaternion directionOfRaycast;
	// Use this for initialization


	void Start () {
		hits = new RaycastHit2D[numbOfRaycasts];
		//This gives me the angle of the casting
		intToRad = 360.0f/numbOfRaycasts*Mathf.Deg2Rad;
		intToDeg = 360.0f/numbOfRaycasts;

		//may need to call multiple times as explosion expands
		//scan for hits
		ScanForHits(12.1f,intToRad);


		//directionOfRaycast = theExplosionParticles.transform.rotation;
		lifeTimeOfParts = 1.01f;//theExplosionParticles.particleSystem.startLifetime;
		//Debug.Log ("The start lifetime is=" +lifeTimeOfParts);


	}





	// Update is called once per frame
	void Update () {
	
	}



	//sweeps a circle, seeing what is hit
	void ScanForHits(float lengthOfCast, float intToRad2)
	{
		for (int i=0; i<numbOfRaycasts; i++)
		{
			hits[i]= Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y),
			                           new Vector2(Mathf.Cos(i*intToRad2),Mathf.Sin(i*intToRad2)),
			                           lengthOfCast);


			theExplosionParticlesClone = Instantiate(theExplosionParticles, transform.position, 
			                                         Quaternion.identity) as ParticleSystem;

			theExplosionParticlesClone.transform.Rotate(-i* intToDeg ,90.0f,90.0f);
			//theExplosionParticlesClone.transform.Rotate(0.0f,90.0f,0.0f);
			//directionOfRaycast = Quaternion.AngleAxis(intToDeg, new Vector3(0.0f,0.0f,1.0f));
			//send bombed messaged to each raycast

			if(hits[i]!=null && hits[i].transform!=null)
			{
				hits[i].transform.gameObject.SendMessage("OnHitByBomb",SendMessageOptions.DontRequireReceiver);
				hits[i].transform.parent.SendMessage("OnHitByBomb",SendMessageOptions.DontRequireReceiver);
				 

				lifeTimeTemp = hits[i].distance*lifeTimeOfParts/lengthOfCast;
				Debug.Log ("The hit distance is= "   +hits[i].distance);
				Debug.Log ("The lifetime of parts = "+lifeTimeOfParts);
				Debug.Log ("The length of cast is = "+lengthOfCast);
				Debug.Log ("The lifetime is= "+lifeTimeTemp);
				theExplosionParticlesClone.GetComponent<ParticleSystem>().startLifetime = lifeTimeTemp;
				//theExplosionParticlesClone.particleSystem.lifeTime =lifeTimeTemp; 
			}


			
			//insantiate a particle system with narrow beam of spawned particles
			//designed to die at the edge of the explosion




			//now I can do a couple of things I can either spawn a particle system every
			////for every angle I test,  or I can create 'ranges where the raycaste returned null
			/// and have one particle system for each of the large ranges




			//if (hits[i]!=null)
			//{
			//	GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			//	sphere.collider.enabled = false;
			//	sphere.transform.position = hits[i].point;
			//}
		}
	}



}
