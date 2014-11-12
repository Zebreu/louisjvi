using UnityEngine;
using System.Collections;

public class Descent : MonoBehaviour {
	GameObject rocketFire;
	float toAdd;
	bool landed;
	float finalY = 31.133f;
	
	void Start()
	{	
		
		landed = false;
		rocketFire = GameObject.FindGameObjectWithTag("fireforrocket");
		toAdd = 1.0f;
	}
		
	// Update is called once per frame
	void Update () {
		if (!landed)
		{
			//Debug.Log("Hm?");
			transform.Translate(new Vector3(0f,-toAdd,0f));
			toAdd = toAdd/1.1f;
			if (toAdd < 0.2)
			{
				toAdd = 0.2f;
			}
			if (transform.position.y - finalY < 3)
			{
				rocketFire.SetActive(false);
				toAdd = 0.1f;
			}
			if (transform.position.y-toAdd < finalY)
			{
				landed = true;
				rocketFire.SetActive(false);
			}
			
		}
	}
}
