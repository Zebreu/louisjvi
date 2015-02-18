using UnityEngine;
using System.Linq;
using System.Collections;

public class ReadDecision : MonoBehaviour {
	
	string decisionfile = @"Log\Louis-JVI-decision.txt";
	float time_counter;
	// Use this for initialization
	void Start () 
	{
		DontDestroyOnLoad(transform.gameObject);
		time_counter = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		float new_time = Time.time;
		if (new_time - time_counter > 10)
		{
			string[] decisionlines = System.IO.File.ReadAllLines(decisionfile);
			Debug.Log (decisionlines.Last());
			time_counter = new_time;
		}
	}
}
