using UnityEngine;
using System.Collections;

public class MeltWall : MonoBehaviour {
	GameObject[] toMelt;
	GameObject[] toMelt2;
	public bool melted1;
	public bool melted2;
	int stopped;
	int stopped2;
	
	GameObject torch;
	// Use this for initialization
	void Start () 
	{
		toMelt = GameObject.FindGameObjectsWithTag("Melt1");
		toMelt2 = GameObject.FindGameObjectsWithTag("Melt2");
		melted1 = false;
		melted2 = false;
		torch = GameObject.Find ("methanetorch");
		torch.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		stopped = 0;
		if (melted1)
		{
			torch.SetActive(true);
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
				torch.SetActive(false);
			}
		}
		
		stopped2 = 0;
		if (melted2)
		{
			torch.SetActive(true);
			foreach(GameObject wallElement in toMelt2)
			{
				if (wallElement.transform.position.y > 32.3f)
				{
					wallElement.transform.Translate(new Vector3(0.0f,-0.3f,0.0f)*Time.deltaTime,Space.World);
				} else 
				{	
					stopped2 += 1;
				}
			}
			if (stopped2 == toMelt2.Length)
			{
				melted2 = false;
				torch.SetActive(false);
			}
		}
	}
}
