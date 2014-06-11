/*
using UnityEngine;
using System.Collections;
using Tobii.EyeTracking.IO;

public class Tracking : MonoBehaviour {
	private IETracker tracker;
	private bool tracking;
	private string ipAddress = "";

	// Use this for initialization
	void Start () 
	{
		Library.Init();
		IEyeTrackerFactoryInfo connectionInfo = new CreateFactoryInfoByIpAddress(ipAddress, 4455, 4457);
		tracker = connectionInfo.CreateEyeTracker;
		
		//Todo: Calibration
		
		tracker.StartTracking();
		tracker.GazeDataReceived += tracker_GazeDataReceived;
		tracking = true;
	}

	void tracker_GazeDataReceived(object sender, GazeDataEventArgs e)
	{
		
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