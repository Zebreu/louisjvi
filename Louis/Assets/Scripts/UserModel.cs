using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class UserModel : MonoBehaviour {
	EyeTrackingClass eyetracking;
	EmotivTracking emotiv;
	TaskManagement taskManagement;
	AdaptationElements adaptation;

	List<double[]> emotionalHistory = new List<double[]>();
	List<double[]> gazeHistory = new List<double[]>();
	List<float> timeHistory = new List<float>(); 
	List<float> progressionHistory = new List<float>();
	int currentProgress;

	List<double> frustrationHistory = new List<double> ();
	List<double> motivationHistory = new List<double>();	
	float timeWindow = 10.0f;
	float startWindow;
	int indexWindow;
	int currentIndex;
	
	float timeWindowAdaptation = 40.0f;
	float lastChange = 0.0f;
	float startWindowAdaptation;

	double windowMotivation = 0.0;
	double windowFrustration = 0.0;

	public bool adapting = false;
	
	void Start () {
		DontDestroyOnLoad(transform.gameObject);
		
		eyetracking = GameObject.Find ("EyeTracking").GetComponent<EyeTrackingClass>();
		emotiv = GameObject.Find ("EEGTracking").GetComponent<EmotivTracking>();
		taskManagement = GameObject.Find ("Tasks").GetComponent<TaskManagement>();
		adaptation = GameObject.Find ("Adaptation").GetComponent<AdaptationElements>();

		timeHistory.Add(Time.time);
		emotionalHistory.Add(emotiv.getAffectivData());
		gazeHistory.Add(eyetracking.getGazeData());
		progressionHistory.Add (timeHistory.Last());
		currentProgress = 0;

		startWindow = timeHistory.Last ();
		indexWindow = 0;
		currentIndex = 0;

		startWindowAdaptation = timeHistory.Last ();

	}
	
	void Update () {
		timeHistory.Add(Time.time);
		emotionalHistory.Add(emotiv.getAffectivData());
		gazeHistory.Add(eyetracking.getGazeData());


		
		// Marks the timeline with progress events
		if (taskManagement.progressionIndex != currentProgress)
		{
			progressionHistory.Add(timeHistory.Last());
		}

		if ((timeHistory.Last () - startWindow) / timeWindow > 1.0f)
		{
			CalculateMotivation();
			indexWindow = currentIndex;
			startWindow = timeHistory.Last ();
		}

		if (adapting)
		{
			if ((timeHistory.Last () - startWindowAdaptation) / timeWindowAdaptation > 1.0f)
			{
				TriggerAdaptation();
				startWindowAdaptation = timeHistory.Last();
			}
		}

		currentIndex += 1;


	}
	
	public float GetTime()
	{
		return timeHistory.Last();
	}

	void TriggerAdaptation()
	{
		int index = motivationHistory.Count;

		if (motivationHistory[index-1]*motivationHistory[index-2] > 0)
		{
			if (motivationHistory[index-1] < 0 && frustrationHistory[index-1] > 0)
			{
				adaptation.Assist();
			}
			else if (motivationHistory[index-1] < 0)
			{
				adaptation.Challenge();
			}
		}

	}

	void CalculateMotivation()
	// 4 is the index of the Engagement/Boredom measure
	// 3 is Frustration
	{
		//double e0 = emotionalHistory[indexWindow] [4];
		//double eLast = emotionalHistory[currentIndex] [4];

		windowMotivation = FindTrend (4);
		windowFrustration = FindTrend (3);
	
		//motivationHistory.Add (windowMotivation);
		//frustrationHistory.Add (windowFrustration);
	}
	
	double FindTrend(int measure)
	//Least-square linear regression
	{
		double sumx = 0;
		double sumx2 = 0;
		
		double sumy = 0;
		double sumy2 = 0;
		double sumxy = 0;
		
		double y = 0;
		for(int i = indexWindow; i < currentIndex+1; i++)
		{
			sumx += timeHistory[i];
			sumx2 += timeHistory[i]*timeHistory[i];

			y = emotionalHistory[i][measure];

			sumy += y;
			sumy2 += y*y;
			sumxy += y*timeHistory[i];
		}
		
		double sxx = sumx2 - ((sumx * sumx) / (currentIndex-indexWindow+1));
		double syy = sumy2 - ((sumy * sumy) / (currentIndex-indexWindow+1));
		double sxy = sumxy - ((sumx * sumy) / (currentIndex-indexWindow+1));
		
		return sxy/sxx;
	}
	
}
