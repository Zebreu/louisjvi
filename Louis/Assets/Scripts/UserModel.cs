using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UserModel : MonoBehaviour {
	EyeTrackingClass eyetracking;
	EmotivTracking emotiv;
	
	List<double[]> emotionalHistory = new List<double[]>();
	List<double[]> gazeHistory = new List<double[]>();
	
	public double difficultyScore;
	
	void Start () {
		DontDestroyOnLoad(transform.gameObject);
		
		eyetracking = GameObject.Find ("EyeTracking").GetComponent<EyeTrackingClass>();
		emotiv = GameObject.Find ("EEGTracking").GetComponent<EmotivTracking>();
		
		// Default initial difficulty
		difficultyScore = 0.5;
	}
	
	void Update () {
		emotionalHistory.Add(emotiv.getAffectivData());
		gazeHistory.Add(eyetracking.getGazeData());
		
		
	}
}
