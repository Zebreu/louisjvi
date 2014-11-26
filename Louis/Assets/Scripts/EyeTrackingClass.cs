
using UnityEngine;
using System.Collections;
using Tobii.EyeTracking.IO;

/*
//From StackOverflow's AngryAnt:
Process otherProcess = new Process ();
otherProcess.StartInfo.FileName = path;
otherProcess.StartInfo.CreateNoWindow = true;
otherProcess.StartInfo.UseShellExecute = false;
otherProcess.StartInfo.RedirectStandardInput = true;
otherProcess.StartInfo.RedirectStandardOutput = true;
 
// Now communicate via streams
//     otherProcess.StandardOutput
// and
//     otherProcess.StandardInput
*/

public class EyeTrackingClass : MonoBehaviour {

	private EyeTrackerBrowser browser;
	private IEyeTracker tracker;
	
	private double leftGaze_X = 0.0;
	private double leftGaze_Y= 0.0;
	//private double leftGaze_Z= 0.0;

	private double rightGaze_X = 0.0;
	private double rightGaze_Y= 0.0;
	//private double rightGaze_Z= 0.0;

	private float left_pupil_diameter = 0.0f;
	private float right_pupil_diameter = 0.0f;

	private bool calibrating = false;
	private float[] calibrators = new float[10] {0.2f,0.2f,0.2f,0.8f,0.8f,0.8f,0.8f,0.2f,0.5f,0.4f};
	private int calibration_index = 0;

	private float time_elapsed;

	public Texture2D circle;

	public bool tracking = false;

	void Start () 
	{
		DontDestroyOnLoad(transform.gameObject);
		tracking = true;
		gameObject.SetActive (tracking); // Disables eye tracking 
		if (tracking)
		{
			Library.Init();

			browser = new EyeTrackerBrowser (EventThreadingOptions.BackgroundThread);
			browser.EyeTrackerFound += browser_EyetrackerFound;
			browser.StartBrowsing();
		}
	}

	private void browser_EyetrackerFound(object sender, EyeTrackerInfoEventArgs e)
	{
		// Finds an eye tracker and connects to it, and starts the tracking and calibration.

		Debug.Log ("Found one");
		tracker = e.EyeTrackerInfo.Factory.CreateEyeTracker(EventThreadingOptions.BackgroundThread);
		Debug.Log ("Connected");
		tracker.StartTracking();
		tracker.GazeDataReceived += tracker_GazeDataReceived;

		browser.StopBrowsing();
		Debug.Log ("Ready");

		tracker.StartCalibration ();
		calibrating = true;

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
		
		left_pupil_diameter = e.GazeDataItem.LeftPupilDiameter;
		right_pupil_diameter = e.GazeDataItem.RightPupilDiameter;

	}

	void Update () 
	{
		//Debug.Log (left_pupil_diameter+" "+right_pupil_diameter);
		
		/*
			if (Mathf.Ceil(Time.time) != time_elapsed) 
			{
				time_elapsed = Mathf.Ceil(Time.time);
				Debug.Log ("Left eye: " + leftGaze_X + ", " + leftGaze_Y+ ". Right eye: " + rightGaze_X+ ", " + rightGaze_Y);
			}
		*/

		if (calibrating)
		{
			if (Input.anyKeyDown)
			{
				tracker.AddCalibrationPoint(new Point2D((double) calibrators[calibration_index], (double) calibrators[calibration_index+1]));
				calibration_index += 2;
				Debug.Log ("Changing point");
				if (calibration_index > 8)                        
				{
					Debug.Log ("Computing calibration");
					tracker.ComputeCalibration();
					tracker.StopCalibration();
					calibrating = false;
					Debug.Log ("Calibration completed");
				}
			}
		}
	}
	
	
	void OnGUI()
	{
		// Doesn't use right eye gaze information as my right eye can deviate outward (Seb)
		float xPosition = (float)(leftGaze_X)*Screen.width;
		float yPosition = (float)(leftGaze_Y)*Screen.height;

		if (calibrating)
		{
			GUI.DrawTexture(new Rect(calibrators[calibration_index]*Screen.width-20, calibrators[calibration_index+1]*Screen.height-20, circle.width, circle.height), circle);
		} else // Shows a circle that follow the user's gaze
		{
			if (false) //Not useful for participants
			{
				GUI.DrawTexture(new Rect(xPosition-18, yPosition-18, circle.width, circle.height), circle);
			}
		}


	}
	
	void OnApplicationQuit()
	{
		//tracker.GazeDataReceived -= tracker_GazeDataReceived;
		tracker.StopTracking();
		tracker.Dispose();
	}

	public double[] getGazeData()
	{
		return new double[] {leftGaze_X, leftGaze_Y, rightGaze_X, rightGaze_Y, (double)left_pupil_diameter, (double)right_pupil_diameter};
		//return new double[] {leftGaze_X, leftGaze_Y, rightGaze_X, rightGaze_Y};
	}
}
