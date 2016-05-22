using UnityEngine;
using System.Collections;

public class CatSpawnerScript : MonoBehaviour {

	////The cat is spawned to attack the jenny
	public Transform catTransform;
	Transform catClone;
	//speed of cat
	float catSpeed, catGamma;
	float speedOfLight;

	float chargeUpTime= 2f;



	// Use this for initialization
	void Start () {
		speedOfLight = GameControl.control.SpeedOfLight;
		InvokeRepeating("Spawning", 2f,2f);
		//StartCoroutine(SpawningCoroutine(catTransform));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*IEnumerator SpawningCoroutine(Transform transformToBeSpawned)
	{
		catClone = Instantiate(catTransform,transform.position,Quaternion.identity) as Transform;
		catClone.rigidbody2D.velocity = new Vector2(speedOfLight,0f);
		yield return new WaitForSeconds(3f);
	}*/
	void Spawning()
	{
		catClone = Instantiate(catTransform,transform.position,Quaternion.identity) as Transform;
		catClone.GetComponent<Rigidbody2D>().velocity = new Vector2(speedOfLight,0f);
		catClone.parent = transform;
		//yield return new WaitForSeconds(3f);
	}

}
