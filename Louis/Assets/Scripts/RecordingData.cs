﻿using UnityEngine;
using System.Collections;

public class RecordingData : MonoBehaviour {
	private EyeTrackingClass eyeTrackingData;
	private string eyetracking_log = @"C:\Users\locarno\Desktop\DataLog\eyetracking_log";
	public bool logging = false; 

	// Use this for initialization
	void Start () {
		gameObject.SetActive(logging);
		if (logging)
		{
			GameObject eyeTracking = GameObject.Find("EyeTracking");
			eyeTrackingData = eyeTracking.GetComponent<EyeTrackingClass>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		double[] gazeData = eyeTrackingData.getGazeData();
		System.IO.File.AppendAllText(eyetracking_log, System.String.Format("{0} {1} {2} {3}", Time.time, gazeData[0], gazeData[1], System.Environment.NewLine));
	}
}