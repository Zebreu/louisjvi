/*
using UnityEngine;
using System.Collections;
using Tobii

public class Tracking : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		Tobii.EyeTracking.Sdk.Library.Init();
		tracker = Tobii.EyeTracking.Sdk.EyetrackerFactory.CreateEyeTracker();
		
		//Todo: Calibration
		
		tracker.StartTracking();
		
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		Debug.Log("some data");
	}
	
	void OnApplicationQuit()
	{
		tracker.StopTracking();
		//tracker.Dispose();
	}
}
*/