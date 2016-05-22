using UnityEngine;
using System.Collections;

public class CatController : MonoBehaviour {
	public float halfLife=5f;
	float timeBorn, dampingFactor, decayFraction;
	float randomNumber;
	public Sprite deadCat, liveCat, propegatingCat;
	SpriteRenderer spriteRenderer;

	//used to display the percent chance alive
	TextMesh percentLiveMesh;
	string percentLive;
	float chanceAlive= 1f;

	int i = 0;
	bool hitWall = false;



	void Start () 
	{

		spriteRenderer = transform.GetComponent<SpriteRenderer>();
		percentLiveMesh =transform.FindChild("ChanceAlive").GetComponent<TextMesh>();

		//Debug.Log("the number is"+randomNumber);
		timeBorn= Time.time;
		dampingFactor = Mathf.Log(2f)/halfLife;

	}

	void Update()
	{
		if (percentLiveMesh !=null)
		{
			chanceAlive = Mathf.Exp(-(Time.time-timeBorn)*dampingFactor);
			percentLive = ( Mathf.Round(chanceAlive*100f) ).ToString();
			percentLiveMesh.text = percentLive+"%";
		}
	}
//	percentSpeedLight = Mathf.Round(speed*oneOverC*1000f)*0.1f;
//	velocityDispText.text  =  percentSpeedLight.ToString();



	void OnTriggerEnter2D (Collider2D  hit)
	{
		if ( !hitWall && !hit.isTrigger && hit.name != "CatBox" )
		{
			Destroy(percentLiveMesh.gameObject);
			hitWall = true;
			//generate a random number between 0 and 1;
			randomNumber = Random.Range(0f,1f);
			//if the number is below the eponential decay the cat is alive
			decayFraction = Mathf.Exp(-(Time.time-timeBorn)*dampingFactor);

			if (randomNumber < decayFraction)
			{//cat alive
				spriteRenderer.sprite = liveCat;
				//Debug.Log("Cat Alive");

			} else 
			{//cat dead
				spriteRenderer.sprite = deadCat;
				//Debug.Log ("Cat Dead");


			}

			//makes the cat fall
			transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			transform.GetComponent<Rigidbody2D>().gravityScale = 4f;

		}  else if (hitWall && !hit.CompareTag("Player") && !hit.isTrigger)
		{
			transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			transform.GetComponent<Rigidbody2D>().gravityScale = 0f;
			//start a death coroutine
			StartCoroutine( DeathCoroutine());
		}

	}


	IEnumerator DeathCoroutine()
	{
		while (i<5)
		{
			spriteRenderer.enabled = !spriteRenderer.enabled;
			i+=1;
			yield return new WaitForSeconds(.2f);
		}

		Destroy(gameObject);

	}



}
