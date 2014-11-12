using UnityEngine;
using System.Collections;

public class RecordingData : MonoBehaviour {
	//private EyeTrackingClass eyeTrackingData;
	//private EmotivTracking emotivTrackingData;
	
	//private string eyetracking_log = @"C:\Users\locarno\Desktop\DataLog\eyetracking_log";
	//private string eeg_log = @"C:\Users\locarno\Desktop\DataLog\eeg_log";
	
	string logfile = @"C:\Users\locarno\Desktop\DataLog\Louis-JVI-log.txt";
	
	UserModel user;
	public bool logging = false; 

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(transform.gameObject);
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
		double[] gazeData = user.GetGazeData();
		double[] emotivData = user.GetEmotivData();
		float time = user.GetTime();
		float success = user.GetInteractionData(); // 0 is empty, 1 is wrong diagram, 2 is correct diagram.
		System.IO.File.AppendAllText(logfile, System.String.Format("{0} {1}", time, System.Environment.NewLine));
		
		// Logs left gaze position and left and right pupil diameters
		//System.IO.File.AppendAllText(eyetracking_log, System.String.Format("{0} {1} {2} {3} {4} {5}", time, gazeData[0], gazeData[1], gazeData[4], gazeData[5], System.Environment.NewLine));
		//System.IO.File.AppendAllText(eyetracking_log, System.String.Format("{0} {1} {2} {3}", Time.time, gazeData[0], gazeData[1], System.Environment.NewLine));
	}
}
