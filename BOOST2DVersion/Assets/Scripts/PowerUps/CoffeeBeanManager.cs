using UnityEngine;
using System.Collections;

public class CoffeeBeanManager : MonoBehaviour {

	public int coffeeBeansCaptured = 0;

	public static CoffeeBeanManager coffeeBeanManager;

	void Awake()
	{
		//makes it a singleton
		if (coffeeBeanManager == null)
		{
			coffeeBeanManager  = this;
		} else if (coffeeBeanManager  != this)
		{
			Destroy(gameObject);
		}


		coffeeBeansCaptured = 0;	
	
	}





	
	void GotABean()
	{
		coffeeBeansCaptured +=1;
	}



	// Update is called once per frame
	void Update () {
	
	}
}
