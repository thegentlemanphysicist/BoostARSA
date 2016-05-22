using UnityEngine;
using System.Collections;

public class tiledConcrete : MonoBehaviour {

	float tileWidth,tileHeight;
	float widthOfWall, heightOfWall;
	float scaleX, scaleY;


	// Use this for initialization
	void Start () {
		tileWidth  = 15.0f;
		tileHeight = 20.0f;
		widthOfWall  = transform.localScale.x;
		heightOfWall = transform.localScale.y;

		scaleX = widthOfWall/tileWidth;
		scaleY = heightOfWall/tileHeight;
		//This will scale the tiles correctly, but has them start at the top corner
		//to fix it, the object is always put into the scene rotated 180 degrees.

		GetComponent<Renderer>().material.mainTextureScale = new Vector2 (scaleX,scaleY);

		//just in case it gets rotated by accident I will send a warning message
		if (Mathf.Abs ( transform.eulerAngles.z -180.0f) > 0.1f)
		{
			Debug.LogWarning("The back tile is not at 180 degrees, my result in bad tiling" +transform.eulerAngles.z);
		}


	}

}
