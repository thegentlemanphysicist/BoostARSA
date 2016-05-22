using UnityEngine;

public class DopplerCircleMeshController : MonoBehaviour
{
    DopplerCircle2 dopCic2;
    float[] mData;

    void Start()
    {
        dopCic2 = gameObject.AddComponent<DopplerCircle2>() as DopplerCircle2;
		if (dopCic2 != null)
        {
			dopCic2.Init( 100, 0, 100, null);
            
			dopCic2.Draw();
		}

    }

    void Update()
    {
        /*if (Input.GetKeyDown("a"))
        {

            mPieChart.Draw(mData);
        }*/
    }

   
}
