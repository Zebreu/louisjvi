using UnityEngine;
using System.Collections;

public class LevelLoading : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		if (other.name.Equals("Player"))
		{
			Application.LoadLevel("surfaceScene");
		}
	}
}
