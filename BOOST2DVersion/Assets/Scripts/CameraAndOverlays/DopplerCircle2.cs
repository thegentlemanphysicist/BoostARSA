using UnityEngine;
using System.Collections;

public class DopplerCircle2 : MonoBehaviour {
	//////////////
	/// <summary>
	/// This will be the pie timer, it is controled by TimeDialationSimple
	/// </summary>/
	//////////////
	
	//float[] mData;

    int mSlices;
    float mRotationAngle;
    float mRadius;
    Material[] mMaterials;

    Vector3[] mVertices;
    Vector3[] mNormals;
	     
    Vector3 mNormal = new Vector3(0f, 0f, -1f);
    Vector2[] mUvs;
    int[] mTriangles;

    MeshRenderer mMeshRenderer;
 
	//Initiates the pie chart
    public void Init(int slices, float rotatioAngle, float radius, Material[] materials)
    {
        //mData = data;
        mSlices = slices;
        mRotationAngle = rotatioAngle;
        mRadius = radius;
		//I should only do this part once.
        // Get Mesh Renderer
        mMeshRenderer = gameObject.GetComponent("MeshRenderer") as MeshRenderer;
        if (mMeshRenderer == null)
        {
            gameObject.AddComponent<MeshRenderer>();
            mMeshRenderer = gameObject.GetComponent("MeshRenderer") as MeshRenderer;
        }
        
        mMaterials = materials;


        Init();
    }
	
	//Initiates the pie chart
    public void Init()
    {
        mSlices = 100;
        mRotationAngle = 90f;
        mRadius = 0.3f;

       // mData = data;
    }
	
	/*
    public void Draw(float[] data)
    {
        //mData = data;
        Draw();
    }*/

    public void Draw()
    {  

        // Init vertices and triangles arrays
        mVertices = new Vector3[mSlices * 3];
        mNormals = new Vector3[mSlices * 3];
        mUvs = new Vector2[mSlices * 3];
        mTriangles = new int[mSlices * 3];

        //gameObject.AddComponent("MeshFilter");
        //gameObject.AddComponent("MeshRenderer");

        Mesh mesh = ((MeshFilter)GetComponent("MeshFilter")).mesh;

        mesh.Clear();

        mesh.name = "Mesh Of Dopler Circle";

        // Rotation offset (to get star point to "12 o'clock")
        float rotOffset = mRotationAngle / 360f * 2f * Mathf.PI;

        // Calc the points in circle
        float angle;
        float[] x = new float[mSlices];
        float[] y = new float[mSlices];
////these will be the coordinates of the tiny slices		
        for (int i = 0; i < mSlices; i++)
        {
            angle = i * 2f * Mathf.PI / mSlices;
            x[i] = (Mathf.Cos(angle + rotOffset) * mRadius);
            y[i] = (Mathf.Sin(angle + rotOffset) * mRadius);
        }

        // Generate mesh with slices (vertices and triangles)
        for (int i = 0; i < mSlices; i++)
        {
			//I switched the order of these two (the 1 and the 0) to get
			//the image on the correct side
            mVertices[i * 3 + 1] = new Vector3(0f, 0f, 0f);
            mVertices[i * 3 + 0] = new Vector3(x[i], y[i], 0f);
            // This will ensure that last vertex = first vertex..
            //the percent operator applies remainders
			mVertices[i * 3 + 2] = new Vector3(x[(i + 1) % mSlices], y[(i + 1) % mSlices], 0f);

            mNormals[i * 3 + 0] = mNormal;
            mNormals[i * 3 + 1] = mNormal;
            mNormals[i * 3 + 2] = mNormal;

            mUvs[i * 3 + 0] = new Vector2(0f, 0f);
            mUvs[i * 3 + 1] = new Vector2(x[i], y[i]);
            // This will ensure that last uv = first uv..
            mUvs[i * 3 + 2] = new Vector2(x[(i + 1) % mSlices], y[(i + 1) % mSlices]);

            mTriangles[i * 3 + 0] = i * 3 + 0;
            mTriangles[i * 3 + 1] = i * 3 + 1;
            mTriangles[i * 3 + 2] = i * 3 + 2;
        }


        // Assign verts, norms, uvs and tris to mesh and calc normals
        mesh.vertices = mVertices;
        mesh.normals = mNormals;

		mesh.uv = mUvs;
        //mesh.triangles = triangles;
		
		
        mesh.subMeshCount = 1;
		int[] subTris;// = new int[];
		
		int countedSlices = 0;
		
		// Set sub meshes

		// Every triangle has three veritces..
		subTris = new int[mSlices * 3];


		// Add tris to subTris
		for (int j = 0; j < mSlices; j++)
		{
			subTris[j * 3 + 0] = mTriangles[countedSlices * 3 + 0];
			subTris[j * 3 + 1] = mTriangles[countedSlices * 3 + 1];
			subTris[j * 3 + 2] = mTriangles[countedSlices * 3 + 2];
			
			countedSlices++;
		}
		mesh.SetTriangles(subTris, 0);

		
	}
	
	// Properties
	//public float[] Data { get { return mData; } set { mData = value; } }
	
	public int Slices { get { return mSlices; } set { mSlices = value; } }

    public float RotationAngle { get { return mRotationAngle; } set { mRotationAngle = value; } }

    public float Radius { get { return mRadius; } set { mRadius = value; } }

    public Material[] Materials { get { return mMaterials; } set { mMaterials = value; } }

	
	
}
