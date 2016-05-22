using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapCameraPanning: MonoBehaviour {
	public static bool insideRectangle;
	RectTransform theRectTransform;
	public Transform miniRect;
	//public RectTransform miniRect;
	bool miniActive = false;
	Vector2 localPoint;
	Vector2 mousePosition;
	float minX, maxX, minY, maxY;
	Material zoomedMaterial;


	//this will map the uv mesh onto the minirectangle in such a way that it magnifies the screen
	MeshFilter meshFilterMiniMap;
	Vector2[] uvMapping;

	// Use this for initialization
	void Start () {
		theRectTransform = gameObject.GetComponent<RectTransform>();
		//Bounds b = RectTransformUtility.CalculateRelativeRectTransformBounds (theRectTransform);
		//Debug.Log("The bounds are=" + b);
		//Debug.Log("Screen width =" + Screen.width);
		//Debug.Log("Screen height=" + Screen.height);
		//minX = (float)Screen.width*0.5f - b.extents.x;
		//maxX = (float)Screen.width*0.5f + b.extents.x;
		//minY = (float)Screen.height*0.5f - b.extents.y;
		//maxY = (float)Screen.height*0.5f + b.extents.y;
		//Debug.Log("minX =" + minX);
		//Debug.Log("maxX =" + maxX);
		//Debug.Log("minY =" + minY);
		//Debug.Log("maxY =" + maxY);

		//zoomedMaterial = miniRect.renderer.material;
		//zoomedMaterial.mainTextureScale = new Vector2(  3f , 3f) ;
		meshFilterMiniMap = miniRect.GetComponent<MeshFilter>();
		//Debug.Log(meshFilterMiniMap.mesh.uv[0]);
		//Debug.Log(meshFilterMiniMap.mesh.uv[1]);
		//Debug.Log(meshFilterMiniMap.mesh.uv[2]);
		//Debug.Log(meshFilterMiniMap.mesh.uv[3]);
		uvMapping = new Vector2[4];
		uvMapping[0] = new Vector2(0.5f+0.1f , 0.5f + 0.1f);
		uvMapping[1] = new Vector2(0.5f-0.1f , 0.5f - 0.1f);
		uvMapping[2] = new Vector2(0.5f-0.1f , 0.5f + 0.1f);
		uvMapping[3] = new Vector2(0.5f+0.1f , 0.5f - 0.1f);
		meshFilterMiniMap.mesh.uv = uvMapping;
	}




	// Update is called once per frame
	void Update () {

		/*mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		if (mousePosition.x > minX && mousePosition.x < maxX && mousePosition.y > minY && mousePosition.y < maxY)
		{
			Debug.Log ("the coordinates are = "+mousePosition);
		}*/


		insideRectangle = RectTransformUtility.ScreenPointToLocalPointInRectangle(
			theRectTransform,
			Input.mousePosition,//Vector2 screenPoint,
			Camera.main,
			out localPoint); 
		//the local point inside the square is +-0.5f 

		//if (Mathf.Abs ( localPoint.x) < 0.5f  &&  Mathf.Abs(localPoint.y) < 0.5f)
		if (Mathf.Abs ( localPoint.x) < 0.4f  &&  Mathf.Abs(localPoint.y) < 0.4f)
		{		

			//enable a sub window
			if (!miniActive){
				miniRect.gameObject.SetActive(true);
				miniActive = true;
			} 
			//this tracks the position of the mouse on the map.
			miniRect.localPosition = localPoint;
			 



			//stretch the tiling so there is a zoomed in version of the map
			uvMapping[0] = new Vector2(localPoint.x+0.5f+0.1f , localPoint.y+0.5f + 0.1f);
			uvMapping[1] = new Vector2(localPoint.x+0.5f-0.1f , localPoint.y+0.5f - 0.1f);
			uvMapping[2] = new Vector2(localPoint.x+0.5f-0.1f , localPoint.y+0.5f + 0.1f);
			uvMapping[3] = new Vector2(localPoint.x+0.5f+0.1f , localPoint.y+0.5f - 0.1f);
			meshFilterMiniMap.mesh.uv = uvMapping;

			//AppliedTo.mainTextureScale = new Vector2(  screeRatio , 1f) ;
			//AppliedTo.mainTextureOffset= new Vector2(0.5f*screeRatio, 0f) ;



		} else
		{

			//deactivate after a pause.
			if (miniActive){
				miniRect.gameObject.SetActive(false);
				miniActive = false;
			} 
		}

	}
}
