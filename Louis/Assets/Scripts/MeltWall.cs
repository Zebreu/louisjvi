using UnityEngine;
using System.Collections;

public class MeltWall : MonoBehaviour {
	GameObject[] toMelt;
	public bool melted1;
	
	// Use this for initialization
	void Start () 
	{
		toMelt = GameObject.FindGameObjectsWithTag("Melt1");
		melted1 = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		int stopped = 0;
		if (melted1)
		{
			foreach(GameObject wallElement in toMelt)
			{
				if (wallElement.transform.position.y > 14.8f)
				{
					wallElement.transform.Translate(new Vector3(0.0f,-0.3f,0.0f)*Time.deltaTime,Space.World);
				} else 
				{	
					stopped += 1;
				}
			}
			if (stopped == toMelt.Length)
			{
				melted1 = false;
			}
		}
	}
}
