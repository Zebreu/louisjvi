using UnityEngine;
using System.Collections;

public class Dissolving : MonoBehaviour {
	// Lowers the debris along the global y axis to clear the way for the player, if the player uses sulfuric acid in front of the debris 
	
	public bool dissolve ;
	GameObject hose;
	void Start()
	{
		dissolve = false;
		hose = GameObject.Find ("acidlauncher");
		hose.SetActive(false);
	}
	
	void Update()
	{
		if (dissolve)
		{
			hose.SetActive(true);
			transform.Translate(new Vector3(0.0f, -0.13f, 0.0f)*Time.deltaTime, Space.World);
			if (transform.position.y < 24.5f)
			{
				dissolve = false;
				hose.SetActive(false);
			}
		}
	}
}
