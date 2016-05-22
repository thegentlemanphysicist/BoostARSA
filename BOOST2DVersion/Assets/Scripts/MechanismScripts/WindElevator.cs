using UnityEngine;
using System.Collections;

public class WindElevator : MonoBehaviour {

	// Use this for initialization
	//void Start () {
	
	//}
	void OnTriggerStay2D(Collider2D hit)
	{
		if (hit.attachedRigidbody && !hit.GetComponent<Collider2D>().isTrigger)
		{
			//Debug.Log("This was called");
			if (hit.attachedRigidbody.velocity.y <50f)
			{
				hit.attachedRigidbody.AddForce(
					hit.attachedRigidbody.mass
					*hit.attachedRigidbody.gravityScale
					*Vector2.up, ForceMode2D.Impulse);
			}
		}
	}


	// Update is called once per frame
	//void Update () {
	
	//}
}
