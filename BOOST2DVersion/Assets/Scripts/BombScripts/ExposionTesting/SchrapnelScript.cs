using UnityEngine;
using System.Collections;

public class SchrapnelScript : MonoBehaviour {

	float lifeTime;//timeStart
	public float blastRadius;
	PhysicsConstants physicsConstants; 
	GameObject physConst;
	float SpeedOfLight;
	bool lifeTimeAssigned = false;

	void start()
	{
		physConst = GameObject.Find ("PhysicalConstants");		
		physicsConstants = physConst.GetComponent("PhysicsConstants") as PhysicsConstants;
		SpeedOfLight=physicsConstants.SpeedOfLight;


		//timeStart=0.0f;
		blastRadius=15.0f;
		//lifeTime= blastRadius/(SpeedOfLight*0.2f)+timeStart;


	}

	void OnLifeAssigned(float newLifeTime)
	{
		lifeTimeAssigned = true;
		lifeTime= Time.time+ newLifeTime;
	}



	void OnTriggerEnter2D(Collider2D hit)
	{
		if (!hit.isTrigger)
		{
		//send death message

			hit.gameObject.SendMessage("OnHitByBomb",SendMessageOptions.DontRequireReceiver);
			if (hit.transform.parent!=null)
			{
				hit.transform.parent.SendMessage("OnHitByBomb",SendMessageOptions.DontRequireReceiver);
			}
		//destroy the schrapnel
		Destroy(gameObject);
		}
	
	
	}




	void Update ()
	{
		//after a set time destroy the schrapnel
		if (lifeTimeAssigned && Time.time> lifeTime)
		{
			Destroy(gameObject);
		}


	}




}
