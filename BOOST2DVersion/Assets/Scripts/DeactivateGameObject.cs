using UnityEngine;
using System.Collections;

public class DeactivateGameObject : MonoBehaviour {
	public Transform promtRecording;
	void Awake()
	{
		if (PlayerPrefs.GetInt("AlreadyRead")==5)
		{
			promtRecording.gameObject.SetActive(false);
		} else 
		{
			promtRecording.gameObject.SetActive(true);
		}
	}

	public void Deactivate()
	{
		PlayerPrefs.SetInt("AlreadyRead",5);
		promtRecording.gameObject.SetActive(false);
	}
}
