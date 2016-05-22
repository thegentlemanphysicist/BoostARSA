using UnityEngine;
using System.Collections;

public class ShapeOfExplosion : MonoBehaviour {
	////////////////////////////////////////////////
	////////////////////////////////////////////////
	////////////When an explosion hits a wall it must be flatend so it won't 
	////////////be seen on the other side.
	////////////
	////////////////////////////////////////////////
	////////////////////////////////////////////////
	//NOTE: THIS WILL ONLY WORK ON HORIZONTAL AND VERTICAL COLLISIONS!!!!
//	Mesh sphereMesh;
	
//	public Vector3[] oldVertices;
    //public Vector2[] oldUV;
    //public int[] oldTriangles;
	
//	public bool hitLeft, hitRight, hitTop, hitBottom;
//	public float coordLeft, coordRight, coordTop, coordBottom;
	//bool hitSomethingBreakable;
	// Use this for initialization
	void Start () {
		
		//hitSomethingBreakable=false;
		
		
		
		
		
		//sphereMesh=gameObject.GetComponent<MeshFilter>().mesh;
		//oldVertices=sphereMesh.vertices;
		
		//hitLeft   = false;
		//hitRight  = false;
		//hitTop    = false;
		//hitBottom = false;
	}
	
	
	
	
	
	
	
	
	// Use this to tell the object hitting it that it's supposed to die
	void OnTriggerEnter (Collider colliderThatHit){
		//if (colliderThatHit.transform.name == "JennyAvatar"){
			//hitSomethingBreakable=true;	
		//}
		
		
		colliderThatHit.gameObject.SendMessage("OnDeath",SendMessageOptions.DontRequireReceiver);
		colliderThatHit.transform.parent.SendMessage("OnDeath",SendMessageOptions.DontRequireReceiver);
		/*if (colliderThatHit.CompareTag("InlasticWall") || colliderThatHit.CompareTag("FixedFloor")){
			//check which side the collider is on.
			//The collision is on the left
			if( !hitLeft &&
				colliderThatHit.transform.position.x< transform.position.x &&
				colliderThatHit.transform.position.y+colliderThatHit.transform.localScale.y/2.0f > transform.position.y &&
				colliderThatHit.transform.position.y-colliderThatHit.transform.localScale.y/2.0f < transform.position.y){
				
				Debug.Log("Hit registered");
				
				hitLeft=true;
				coordLeft = colliderThatHit.transform.position.x
					+ colliderThatHit.transform.localScale.x/2.0f; 
				
				Debug.Log("Hit left registered");
			    Debug.Log("The collider position is x=" + colliderThatHit.transform.position.x+" y=" 
					+colliderThatHit.transform.position.y);
				Debug.Log("The explosion position is "+ transform.position.x + " y=" + transform.position.y);
			}
			
			
		}*/
	
	}//*/
	
	
	void OnCollisionStay(Collision colliderThatHit){
//		if (colliderThatHit.transform.name == "JennyAvatar"){
		//	hitSomethingBreakable=true;	
		//}
		//TO PREVENT A SHEILDED COLLISION FROM KILLING JENNY DO A RAYCAST TO MAKE 
		//SURE THAT JENNY IS THE FIRST HIT.
		Debug.Log("Explosion Hit"+ colliderThatHit.transform.name);
		colliderThatHit.gameObject.SendMessage("OnDeath",SendMessageOptions.DontRequireReceiver);
		colliderThatHit.transform.parent.SendMessage("OnDeath",SendMessageOptions.DontRequireReceiver);
		
	
	}//*/
	
	
	
	
	
	/*void OnCollisionEnter  (Collision collision){
		collision.gameObject.SendMessage("OnDeath",SendMessageOptions.DontRequireReceiver);
		Debug.Log("Hit registered");
	
	}//*/
	
	// Update is called once per frame
	void Update () {
	
		
		
		
		
		
		
		
		
		
		/*	//Only run this if the explosion hits something
		if (hitLeft || hitRight || hitTop || hitBottom){
			oldVertices=sphereMesh.vertices;
			for ( int i =0; i <  oldVertices.Length;i++ ){
				Debug.Log("The left coord is "+ coordLeft);
				if (hitLeft){	
					if (oldVertices[i].x<coordLeft){
						oldVertices[i].x=coordLeft;				
					}
				}
				
				if (hitRight){				
					if (oldVertices[i].x>coordRight){
						oldVertices[i].x=coordRight;				
					}
				}	
				
				if (hitBottom){				
					if (oldVertices[i].y<coordBottom){
						oldVertices[i].y=coordBottom;				
					}
				}
				
				if (hitTop){				
					if (oldVertices[i].y > coordTop){
						oldVertices[i].y=coordTop;				
					}
				}	
					
			}
			//After redefining the vertices
			sphereMesh.vertices=oldVertices;
		}//*/
		
	}
	
	

}
