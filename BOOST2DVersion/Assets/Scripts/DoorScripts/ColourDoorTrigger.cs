using UnityEngine;
using System.Collections;

public class ColourDoorTrigger : MonoBehaviour {
	//This script needs to animate the turning off of the gate
		//colliders all get turned off.
		//triger falls down, as it falls the door collapses after it
		//trigger bounces once or twice
	//game object distroyed


	//This scipt has a custom editor attached.  Look in Editor/DoorTriggerEditor to 
	//modify it.

	//may make it a moving target eventually
	public Transform targetTransform, laserTransform, labelTransform;




	public int pulseWavelengthKey;
	public Color beamColour;
	Color returnedColour;
	float hueOfPulse;

	
	public float maxGateHeight = 25.0f;//This is the max length of the laser  must change for taller gaps
									//kept small to save memory.
	float oldBeamLength = 25.0f;


	string nameOfShooter;



	TextMesh wavelengthLabelMesh;

	//These come into play when the target is shrinking to nothingness.
	float deathTime=0.60f, maxDeathTime = 5.0f;//controls how long the death animation takes
	float timeOfDeath, timeOfHit, initialTransformScale, oneOverDeathTime;
	float scaleFactorOfTarget;


	float minBeamThickness, maxBeamThickness;
	float meanBeamThickness, currentBeamThickness;



	bool laserOn=true;
	RaycastHit  firstObjHit;
	//Transform whatIsTheBeamitting;



	public enum StateOfGate
	{
		gateOn,
		targetTriggered,
		targetFall,
		destroyBeam,
		targetBounce,
		destroyObject
	};
	StateOfGate stateOfGate = StateOfGate.gateOn;


 






	// Use this for initialization
	void Start () {
		//Debug.Log("The wavelength is" + pulseWavelengthKey );
		//targetTransform= transform.FindChild("Target");
		//laserTransform=transform.FindChild("CylinderLaserBarrier");
		//MAKE TARGET AND DOOR GLOW THE PROPER COLOUR

		initialTransformScale= targetTransform.localScale.x;
		oneOverDeathTime = 1.0f/deathTime;

	//Widen and narrow the beam to look cool
		maxBeamThickness= laserTransform.localScale.x;
		minBeamThickness= 0.5f*maxBeamThickness;
		meanBeamThickness = 0.5f*(maxBeamThickness+minBeamThickness);

	
	//Set the colour of the beam to  the wavelenth
		//Step 1) convert the wavelength to a colour
		beamColour = CalculateTheColour( (float)pulseWavelengthKey );
		//Step 2) set the colour of the beam
		laserTransform.GetComponent<Renderer>().materials[0].SetColor("_Color",beamColour);
		//Step 3) put the right coloured halo up

		//Step 4) display the wavelength on the target
		//labelTransform = targetTransform.FindChild("TargetMesh").FindChild("wavelengthLabel");
		//labelTransform = targetTransform.FindChild("wavelengthLabel");

		labelTransform.GetComponent<TextMesh>().text = pulseWavelengthKey.ToString();
	//		ToString("pulseWavelengthKey");


	
	}





	 
	void OnCollision2DEnter(){
		if (stateOfGate == StateOfGate.targetFall){
			stateOfGate=StateOfGate.destroyBeam;
		}
	}








	void OnHitByPulse(float pulseWavelength){
//		Debug.Log("Door Trigget Got shot");
//		Debug.Log("The pulse wavelength is ="+ pulseWavelength);
//		Debug.Log("The wavelength is ="+ pulseWavelengthKey);


		//NEEDS TO CHECK IF THE PULS IS ACCEPTABLE

		//IF ACCEPTABLE SEND MESSAGE TO DOOR TO OPEN &
		//TURN OFF THE COLLIDER AND RENDERER OF THE TARGET

		//MESSAGE CAN'T TRAVEL FASTER THAN LIGHT





		if (pulseWavelength < (float)pulseWavelengthKey  ){
			stateOfGate = StateOfGate.targetTriggered;
			//Destroy(gameObject);	
		}
		
	}
	
	// Update is called once per frame
	void Update () {

		//This has to be called whenever the beam collides with a new object, 
		//or when it loses contact with an old one



		if (laserOn){
			//find out how long the beam has to be
			Physics.Raycast(targetTransform.position,Vector3.down,out firstObjHit, maxGateHeight);
			//only call this if the distance has changed.  
			if (firstObjHit.distance != oldBeamLength){
				//whatIsTheBeamitting=firstObjHit.transform;

				//makes sure the beam is the correct length
				laserTransform.localScale = new Vector3(laserTransform.localScale.x, 
				                                        0.5f*firstObjHit.distance,
				                                        laserTransform.localScale.z);
				//makes sure the beam is the correct position
				laserTransform.localPosition = new Vector3(laserTransform.localPosition.x, 
				                                           targetTransform.localPosition.y-0.5f*firstObjHit.distance,
															laserTransform.localPosition.z);
				oldBeamLength=firstObjHit.distance;
			}
		}
		GateSwitchStatement();




		if (laserTransform != null){
			currentBeamThickness = meanBeamThickness + 
				0.5f*(maxBeamThickness-minBeamThickness)*Mathf.Sin(5.0f*Time.time);
			laserTransform.localScale = new Vector3(currentBeamThickness,
			                                   laserTransform.localScale.y,
			                                   currentBeamThickness);

		}






	
	}


	void GateSwitchStatement(){
		switch(stateOfGate)  {

		//case (StateOfGate.gateSpawning) :
			//need to extend the cylinder until the laser collides with something
			//need 


		case (StateOfGate.gateOn) :
			break;
		
		case (StateOfGate.targetTriggered):

			targetTransform.GetComponent<Rigidbody2D>().isKinematic = false;
			//apply a small force downward. for some reason it doesn't fall
			targetTransform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.0f, -1.0f) );
			timeOfHit=Time.time;

			stateOfGate = StateOfGate.targetFall;
			goto case StateOfGate.targetFall;
		case (StateOfGate.targetFall):
			//let the target fall to the ground(taken care of above
			//shrink the beam as it goes (taken care of by ray cast)
			if (laserTransform.localScale.y > 1.5f){
				break;
			} else {
			
				stateOfGate = StateOfGate.destroyBeam;
				goto case StateOfGate.destroyBeam;
			}
		case (StateOfGate.destroyBeam):
			//when the target hits the ground the beam is done for
			laserTransform.GetComponent<Renderer>().enabled = false;
			//turn off the coliders of the object
			laserTransform.GetComponent<Collider2D>().enabled = false;

			laserOn= false;
			timeOfDeath=Time.time;


			stateOfGate = StateOfGate.targetBounce;
			goto case StateOfGate.targetBounce;

		case (StateOfGate.targetBounce):
			//animate the target shrinking to nothing

			if (Time.time<timeOfDeath+deathTime && Time.time <timeOfHit+maxDeathTime){
				scaleFactorOfTarget = Mathf.Lerp (initialTransformScale, 0.0f,(Time.time-timeOfDeath)*oneOverDeathTime);
				targetTransform.localScale = new Vector3( scaleFactorOfTarget,
				                                         scaleFactorOfTarget,
				                                   		1.0f);

				break;
			} else {
				stateOfGate = StateOfGate.destroyObject;
				goto case StateOfGate.destroyObject;

			}

		case (StateOfGate.destroyObject):
			//when the target's clear of the screen destroy it
			Destroy(gameObject);
			break;			
		
		}
	}




	/*
	public void createTheDoor(int wavelength){
		targetTransform= transform.FindChild("Target");
		laserTransform=transform.FindChild("CylinderLaserBarrier");
		//MAKE TARGET AND DOOR GLOW THE PROPER COLOUR
		
		initialTransformScale= targetTransform.localScale.x;

		//Set the colour of the beam to  the wavelenth
		//Step 1) convert the wavelength to a colour
		beamColour = CalculateTheColour( (float)pulseWavelengthKey );
		//Step 2) set the colour of the beam
		laserTransform.renderer.materials[0].SetColor("_Color",beamColour);

		//Step 4) display the wavelength on the target
		labelTransform = targetTransform.FindChild("wavelengthLabel");
		
		labelTransform.GetComponentInChildren<TextMesh>().text = pulseWavelengthKey.ToString();
		//		ToString("pulseWavelengthKey");
		


	}*/

	public Color CalculateTheColour(float wavelengthOfBeam ){


		if (wavelengthOfBeam >390 && wavelengthOfBeam < 700){
			//VISIBLE LIGHT
			hueOfPulse = HueOfWavelength( wavelengthOfBeam );
			//This function comes from http://wiki.unity3d.com/index.php?title=HSBColor
			returnedColour = ToColor(hueOfPulse ,1.0f,1.0f,1.0f);


		} else if (wavelengthOfBeam > 700) {
			//INFRARED LIGHT
			//hueOfPulse = HueOfWavelength(blueShift*initialPulseColour);
			//This function comes from http://wiki.unity3d.com/index.php?title=HSBColor
			returnedColour = ToColor(0.0f, 70.0f/360.0f ,0.0f,1.0f);
			
		} else {
			//ULTRA VIOLET
			//hueOfPulse = HueOfWavelength(blueShift*initialPulseColour);
			//This function comes from http://wiki.unity3d.com/index.php?title=HSBColor
			returnedColour = ToColor(274.0f/360.0f, 70.0f/360.0f ,0.0f,1.0f);
		}


		return returnedColour;


	}

	//Taken from http://wiki.unity3d.com/index.php?title=HSBColor
	public static Color ToColor(float hh, float ss, float bb, float aa)    {
		float r = bb;
		float g = bb;
		float b = bb;
		if (ss != 0)
		{
			float max = bb;
			float dif = bb * ss;
			float min = bb - dif;
			
			float h = hh * 360f;
			
			if (h < 60f)
			{
				r = max;
				g = h * dif / 60f + min;
				b = min;
			}
			else if (h < 120f)
			{
				r = -(h - 120f) * dif / 60f + min;
				g = max;
				b = min;
			}
			else if (h < 180f)
			{
				r = min;
				g = max;
				b = (h - 120f) * dif / 60f + min;
			}
			else if (h < 240f)
			{
				r = min;
				g = -(h - 240f) * dif / 60f + min;
				b = max;
			}
			else if (h < 300f)
			{
				r = (h - 240f) * dif / 60f + min;
				g = min;
				b = max;
			}
			else if (h <= 360f)
			{
				r = max;
				g = min;
				b = -(h - 360f) * dif / 60 + min;
			}
			else
			{
				r = 0;
				g = 0;
				b = 0;
			}
		}
		
		return new Color(Mathf.Clamp01(r),Mathf.Clamp01(g),Mathf.Clamp01(b),aa);
	}
	
	
	public float HueOfWavelength(float wavelength) {
		//This only works for wavelenths between 390nm and 700nm.
		//Violet coresponds to a hue h=274/360;
		//390 is the wavelenth of violet.
		//700 is the wavelenth of red.
	
		float newHue;
		newHue = (274.0f/360.0f)/(390.0f-700.0f)*(wavelength-700.0f);
		return newHue;
	}


}
