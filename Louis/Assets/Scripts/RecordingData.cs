using UnityEngine;
using System.Collections;

public class RecordingData : MonoBehaviour {
	private EyeTrackingClass eyeTrackingData;
	private EmotivTracking emotivTrackingData;
	private string eyetracking_log = @"C:\Users\locarno\Desktop\DataLog\eyetracking_log";
	private string eeg_log = @"C:\Users\locarno\Desktop\DataLog\eeg_log";
	public bool logging = false; 

	// Use this for initialization
	void Start () {
		gameObject.SetActive(logging);
		if (logging)
		{
			GameObject eyeTracking = GameObject.Find("EyeTracking");
			eyeTrackingData = eyeTracking.GetComponent<EyeTrackingClass>();
			emotivTrackingData = GameObject.Find ("EEGTracking").GetComponent<EmotivTracking>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		double[] gazeData = eyeTrackingData.getGazeData();
		// Logs left gaze position and left and right pupil diameters
		System.IO.File.AppendAllText(eyetracking_log, System.String.Format("{0} {1} {2} {3} {4} {5}", Time.time, gazeData[0], gazeData[1], gazeData[4], gazeData[5], System.Environment.NewLine));
		//System.IO.File.AppendAllText(eyetracking_log, System.String.Format("{0} {1} {2} {3}", Time.time, gazeData[0], gazeData[1], System.Environment.NewLine));
	}
}
