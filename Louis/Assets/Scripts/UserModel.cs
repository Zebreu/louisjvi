using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class UserModel : MonoBehaviour {
	EyeTrackingClass eyetracking;
	EmotivTracking emotiv;
	TaskManagement taskManagement;
	
	List<double[]> emotionalHistory = new List<double[]>();
	List<double[]> gazeHistory = new List<double[]>();
	List<float> timeHistory = new List<float>(); 
	List<float> progressionHistory = new List<float>();
	int currentProgress;
	
	public double difficultyScore;
	
	void Start () {
		DontDestroyOnLoad(transform.gameObject);
		
		eyetracking = GameObject.Find ("EyeTracking").GetComponent<EyeTrackingClass>();
		emotiv = GameObject.Find ("EEGTracking").GetComponent<EmotivTracking>();
		taskManagement = GameObject.Find ("Tasks").GetComponent<TaskManagement>();
		
		// Default initial difficulty
		difficultyScore = 0.5;
		
		timeHistory.Add(Time.time);
		emotionalHistory.Add(emotiv.getAffectivData());
		gazeHistory.Add(eyetracking.getGazeData());
		progressionHistory.Add (timeHistory.Last());
		currentProgress = 0;

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
		
		CalculateMotivation();
	}
	
	public float GetTime()
	{
		return timeHistory.Last();
	}
	
	void CalculateMotivation()
	{
		
	}
	
	void CalculateDifficultyScore()
	{
		
	}
	
}
