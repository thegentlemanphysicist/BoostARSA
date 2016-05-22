using UnityEngine;
using System.Collections;


	/// SOURCE OF THIS CODE
	/// Shttps://www.youtube.com/watch?v=VIBWZYmZ0Vc&feature=youtu.be	
	///http://pastebin.com/NByJir5H






[RequireComponent(typeof(Camera))]
public class CamRenderTo : MonoBehaviour {
	
	Texture2D texture;
	public Material AppliedTo;
	float screeRatio;
	//Turn this on to automatically set depth
	public bool autoDepth;

	void Start () {
		if(autoDepth && GetComponent<Camera>() != Camera.main)
			GetComponent<Camera>().depth=Camera.main.depth+1;
		texture = new Texture2D(Mathf.RoundToInt(Screen.width), Mathf.RoundToInt(Screen.height), TextureFormat.ARGB32, false);
		//need to stretch the tiling so it stays a square
		screeRatio=((float)Screen.height)/((float)Screen.width);
		AppliedTo.mainTextureScale = new Vector2(  screeRatio , 1f) ;
		AppliedTo.mainTextureOffset= new Vector2(0.5f*screeRatio, 0f) ;



	}
	
	//Called after current camera has rendered
	void OnPostRender () {
		texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
		texture.Apply();
		
		AppliedTo.mainTexture=texture;
		
	}
}