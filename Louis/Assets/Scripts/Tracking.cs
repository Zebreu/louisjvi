
using UnityEngine;
using System.Collections;
using Tobii.EyeTracking.IO;

public class Tracking : MonoBehaviour {

	private EyeTrackerBrowser browser;
	private IEyeTracker tracker;
	//private string ipAddress = "169.254.219.4"; // IP might be wrong
	
	private double leftGaze_X = 0.0;
	private double leftGaze_Y=0.0;
	//private double leftGaze_Z=0.0;

	private double rightGaze_X = 0.0;
	private double rightGaze_Y=0.0;
	//private double rightGaze_Z=0.0;

	public Texture2D circle;

	void Start () 
	{
		Library.Init();

		browser = new EyeTrackerBrowser (EventThreadingOptions.BackgroundThread);
		browser.EyeTrackerFound += browser_EyetrackerFound;
		browser.StartBrowsing();

		//browser.EyeTrackerRemoved += browser_EyetrackerRemoved;
		//browser.EyeTrackerUpdated += browser_EyetrackerUpdated;
		//IEyeTrackerFactoryInfo connectionInfo = new CreateFactoryInfoByIpAddress(ipAddress, 4455, 4457);
		//tracker = connectionInfo.CreateEyeTracker;

	}

	private void browser_EyetrackerFound(object sender, EyeTrackerInfoEventArgs e)
	{
		// Finds an eye tracker and connects to it. Starts the tracking.
		Debug.Log ("Found one");
		tracker = e.EyeTrackerInfo.Factory.CreateEyeTracker(EventThreadingOptions.BackgroundThread);
		Debug.Log ("Connected");
		tracker.StartTracking();
		tracker.GazeDataReceived += tracker_GazeDataReceived;

		browser.StopBrowsing();
		Debug.Log ("Ready");
		//Todo: Calibration
	}

	void tracker_GazeDataReceived(object sender, GazeDataEventArgs e)
	{
		// Manages the data received by the eye tracker 

		leftGaze_X = e.GazeDataItem.LeftGazePoint2D.X;
		leftGaze_Y = e.GazeDataItem.LeftGazePoint2D.Y;
		//leftGaze_Z = e.GazeDataItem.LeftGazePoint3D.Z;

		rightGaze_X = e.GazeDataItem.RightGazePoint2D.X;
		rightGaze_Y = e.GazeDataItem.RightGazePoint2D.Y;
		//rightGaze_Z = e.GazeDataItem.RightGazePoint3D.Z;

	}

	void Update () 
	{
		Debug.Log("some data: "+leftGaze_X+", "+rightGaze_X);
	}


	void OnGUI()
	{
		float xPosition = (float)(leftGaze_X)*Screen.width;
		float yPosition = (float)(leftGaze_Y)*Screen.height;
		GUI.DrawTexture(new Rect(xPosition, yPosition, circle.width, circle.height), circle);
	}
	
	void OnApplicationQuit()
	{
		//tracker.GazeDataReceived -= tracker_GazeDataReceived;
		tracker.StopTracking();
		tracker.Dispose();
	}
}
