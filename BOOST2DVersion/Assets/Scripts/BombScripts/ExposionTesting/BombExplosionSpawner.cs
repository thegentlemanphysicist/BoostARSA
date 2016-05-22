using UnityEngine;
using System.Collections;



[RequireComponent(typeof(AudioSource))]
public class BombExplosionSpawner : MonoBehaviour {
	public Transform explosionSprite;
	Transform explosionSpriteClone;

	float theta= 22.5f*Mathf.Deg2Rad;

	int i=0;
	public bool goBoom;


	//PhysicsConstants physicsConstants; 
	//GameObject physConst;
	float SpeedOfLight;



	float reangeOfBlast, speedOfBlast, lifeOfSchrapInBomb;// all in bomb frame
	float lifeOfSchrapInLab;
	Vector2 velOfSchrapnelInBombFrame;
	public Vector2 velOfBomb;
	float wx, wy;// vector components for lorentz trans function at end
	float explosionFallSmoothing;


	//Transform bombTransform;

	public Vector3 bombPosition;
	Vector3 schrapPosition;


	// Use this for initialization
	void Start () {
		//physConst = GameObject.Find ("PhysicalConstants");		
		//physicsConstants = physConst.GetComponent("PhysicsConstants") as PhysicsConstants;
		SpeedOfLight=GameControl.control.SpeedOfLight;
		explosionFallSmoothing =GameControl.control.explosionFallSmoothing;
		//bombTransform = transform.FindChild("Bomb") as Transform;








		reangeOfBlast = 10.0f;
		speedOfBlast  = SpeedOfLight*0.8f;
		lifeOfSchrapInBomb = reangeOfBlast/speedOfBlast; 


		goBoom = false;
	}
	
	// Update is called once per frame
	void Update () {
	if (goBoom)
		{
			OnBoom();
			goBoom=!goBoom;
			//don't let the bomb get closer than 5 units
			CameraTracking.cameraTracking.BombShake( Mathf.Max(
				Vector3.Distance(PlatformerPushBodies.platformerPushBodies.transform.position,
			                 bombPosition), 5f) );

		}
	}

	void OnBoom()
	{
		//the spawn point is shifted by half the scale due to where the anchor pts on the mesh are
		schrapPosition = new Vector3(bombPosition.x,//+ 2.5f,
		                             bombPosition.y,
									0.0f);

		//must assign the velocity and position of the bomb in the explosion script

		//this makes the explosion sound play
		GetComponent<AudioSource>().Play();


		//spawn a bunch of clones
		while (i*theta <= 2*Mathf.PI)
		{
			explosionSpriteClone = Instantiate(explosionSprite, schrapPosition, Quaternion.identity)
				as Transform;


			///Need to calculate the final velocity of each collider by using relativistic addition of velocities
			///		calculate speed 
			/// 	calculate redshift
			/// 	calculate lifetime
			/// 	assign velocity
			velOfSchrapnelInBombFrame = new Vector2(speedOfBlast*Mathf.Cos(i*theta),
			              speedOfBlast*Mathf.Sin(i*theta));




			explosionSpriteClone.GetComponent<Rigidbody2D>().velocity //= new Vector2(0.0f,0.0f);
				= LorentzTransOfVelocity( velOfBomb, velOfSchrapnelInBombFrame);

			//This makes it so the schrapnel all die at the same time in the rest frame
			lifeOfSchrapInLab *=  Mathf.Sqrt(     1  -  Mathf.Pow(speedOfBlast*Mathf.Cos(i*theta)/SpeedOfLight, 2.0f)   );
			
			explosionSpriteClone.gameObject.SendMessage("OnLifeAssigned",lifeOfSchrapInLab,
			                           SendMessageOptions.DontRequireReceiver); 
			i++;
		}
		
		
	}


	///this function takes the velocity of the bomb and adds the velocity 
	/// of schrapnel to give the correct final speed
	Vector2 LorentzTransOfVelocity( Vector2 velOfBomb, Vector2 velOfSchrapnel)
	{

		wx= (velOfSchrapnel.x+velOfBomb.x)/(1.0f+velOfSchrapnel.x*velOfBomb.x/Mathf.Pow(SpeedOfLight,2.0f));

		// and on velofBomb.y since there's no relativistic effect in that direction
		wy= velOfSchrapnel.y*Mathf.Sqrt(    1 - Mathf.Pow(velOfBomb.x/SpeedOfLight, 2.0f)    )
			/(    1.0f+velOfSchrapnel.x*velOfBomb.x/Mathf.Pow(SpeedOfLight,2.0f)    ) 
			+ velOfBomb.y*explosionFallSmoothing;
		// multiply by explosionFallSmoothing so it doesn't fall too fast.
		//this is not obvious since there's no effect from 
		lifeOfSchrapInLab = lifeOfSchrapInBomb/Mathf.Sqrt(     1  -  Mathf.Pow(wx/SpeedOfLight, 2.0f)   );
		return new Vector2(wx, wy);
	}





}
