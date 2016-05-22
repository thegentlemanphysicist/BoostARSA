using UnityEngine;
using System.Collections;

public class ConversationTrigger : MonoBehaviour {
		//public string triggerName;


	void  OnTriggerEnter2D (Collider2D hit)
	{
		if (hit.name == "JennyAvatar" )
		{

			transform.parent.SendMessage("OnDialogueTrigger", transform.name);// ,SendMessageOptions.DontRequireReceiver);

		}

		
	}

	void OnTriggerExit2D (Collider2D hit)
	{
		if (hit.name == "JennyAvatar" )
		{
			transform.parent.SendMessage("OnDialogueExit");//,SendMessageOptions.DontRequireReceiver);
		
		}
		
	}



}
