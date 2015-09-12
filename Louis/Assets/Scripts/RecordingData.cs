using UnityEngine;
using System.Collections;

public class RecordingData : MonoBehaviour {
	//private EyeTrackingClass eyeTrackingData;
	//private EmotivTracking emotivTrackingData;
	
	//private string eyetracking_log = @"C:\Users\locarno\Desktop\DataLog\eyetracking_log";
	//private string eeg_log = @"C:\Users\locarno\Desktop\DataLog\eeg_log";
	
	string logfile = @"Log\Louis-JVI-log.txt";
	string easyfile = @"Log\Louis-JVI-calibEasy.txt";
	string hardfile = @"Log\Louis-JVI-calibHard.txt";

	int calibCounter = 0;
	UserModel user;
	bool logging; 
	bool eyetrackingEnabled;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(transform.gameObject);
		logging = true;
		eyetrackingEnabled = true;
		gameObject.SetActive(logging);
		if (logging)
		{
			//GameObject eyeTracking = GameObject.Find("EyeTracking");
			//eyeTrackingData = eyeTracking.GetComponent<EyeTrackingClass>();
			//emotivTrackingData = GameObject.Find ("EEGTracking").GetComponent<EmotivTracking>();
			user = GameObject.Find ("UserModelling").GetComponent<UserModel>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		double[] gazeData;
		if (eyetrackingEnabled) // Eyetracking disabled
		{
			gazeData = user.GetGazeData();
		} else {
			gazeData = new double[]{0.0,0.0,0.0,0.0,0.0,0.0};
		}
		double[] emotivData = user.GetEmotivData();
		float time = user.GetTime();
		string currentTime = user.GetCurrentTime ();
		float success = user.GetInteractionData(); // 0 is empty, 1 is wrong diagram, 2 is correct diagram.
		System.IO.File.AppendAllText(logfile, System.String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12}", time, gazeData[0], gazeData[1], gazeData[4], gazeData[5], emotivData[0], emotivData[1], emotivData[2], emotivData[3], emotivData[4], success, currentTime, System.Environment.NewLine));
		if (Application.loadedLevelName.Equals("calibrationScene"))
		{
			if (Input.GetButtonDown("Combine"))
			{
				
				if (calibCounter == 0)
				{
					System.IO.File.AppendAllText(easyfile, System.String.Format ("{0} {1}", time, currentTime));
					calibCounter += 1;
				}
				else if (calibCounter == 1){
					System.IO.File.AppendAllText(hardfile, System.String.Format ("{0} {1}", time, currentTime));
					calibCounter += 1;
				}
			}

		}

		// Logs left gaze position and left and right pupil diameters
		//System.IO.File.AppendAllText(eyetracking_log, System.String.Format("{0} {1} {2} {3} {4} {5}", time, gazeData[0], gazeData[1], gazeData[4], gazeData[5], System.Environment.NewLine));
		//System.IO.File.AppendAllText(eyetracking_log, System.String.Format("{0} {1} {2} {3}", Time.time, gazeData[0], gazeData[1], System.Environment.NewLine));
	}
}
