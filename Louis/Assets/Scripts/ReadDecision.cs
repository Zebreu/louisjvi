using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ReadDecision : MonoBehaviour {
	
	string decisionfile = @"Log\Louis-JVI-decision.txt";
	float time_counter;
	List<string> alldecisions;
	
	// Use this for initialization
	void Start () 
	{
		DontDestroyOnLoad(transform.gameObject);
		time_counter = 0;
		alldecisions = new List<string>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		float new_time = Time.time;
		if (new_time - time_counter > 10 && System.IO.File.Exists (decisionfile))
		{
			alldecisions.Clear ();
			string[] decisionlines = System.IO.File.ReadAllLines(decisionfile);
			alldecisions.AddRange (decisionlines);
			//Debug.Log (decisionlines.Last());
			time_counter = new_time;
		}
	}
	
	public List<string> getDecisions()
	{
		return alldecisions;
	}
}
