using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FaceReaderAPI;
using FaceReaderAPI.Data;

/* DOESN'T WORK, NOT SURE WHY */

public class FaceReading : MonoBehaviour {
	FaceReaderController mFaceReaderController = new FaceReaderController("127.0.0.1", 9090);
	
	int frameCounter = 0;
	
	List<string> faceReaderOutput = new List<string>();

	// Use this for initialization
	void Start () {
		try
		{
			// register the events
			mFaceReaderController.ClassificationReceived += 
				new EventHandler<ClassificationEventArgs>(a_faceReaderController_ClassificationReceived);
			
			mFaceReaderController.Disconnected += 
				new EventHandler(a_faceReaderController_Disconnected);
			
			mFaceReaderController.Connected += 
				new EventHandler(a_faceReaderController_Connected);
			
			mFaceReaderController.ActionSucceeded += 
				new EventHandler<MessageEventArgs>(a_faceReaderController_ActionSucceeded);
			
			mFaceReaderController.ErrorOccured += 
				new EventHandler<ErrorEventArgs>(a_faceReaderController_ErrorOccured);
			
			mFaceReaderController.AvailableStimuliReceived += 
				new EventHandler<AvailableStimuliEventArgs>(a_faceReaderController_AvailableStimuliReceived);
			
			mFaceReaderController.AvailableEventMarkersReceived += 
				new EventHandler<AvailableEventMarkersEventArgs>(a_faceReaderController_AvailableEventMarkersReceived);
			
			
			// connect to FaceReader. If the connection was succesful, Connected will fire, otherwise Disconnected will fire
			mFaceReaderController.ConnectToFaceReader();
			//Debug.Log (mFaceReaderController.IsConnected.ToString ());
			//mFaceReaderController.StartLogSending(FaceReaderAPI.Data.LogType.DetailedLog);
			//mFaceReaderController.StartAnalyzing();
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
		
		
	}
	
	void a_faceReaderController_AvailableStimuliReceived(object sender, AvailableStimuliEventArgs e)
	{
	}
	
	void a_faceReaderController_AvailableEventMarkersReceived(object sender, AvailableEventMarkersEventArgs e)
	{
	}
	
	
	void a_faceReaderController_ErrorOccured(object sender, ErrorEventArgs e)
	{
		Debug.Log("Error occured\t-> " + e.Exception.Message);
	}
	
	void a_faceReaderController_ActionSucceeded(object sender, MessageEventArgs e)
	{
		Debug.Log("Action Succeeded\t-> " + e.Message);
	}
	
	void a_faceReaderController_Connected(object sender, EventArgs e)
	{
		Debug.Log("Connection to FaceReader was succesful");
		mFaceReaderController.StartLogSending(FaceReaderAPI.Data.LogType.DetailedLog);
	}
	
	void a_faceReaderController_Disconnected(object sender, EventArgs e)
	{
		Debug.Log("Disconnected");
	}
	
	void a_faceReaderController_ClassificationReceived(object sender, ClassificationEventArgs e)
	{
		// get the classification from the event arguments
		FaceReaderAPI.Data.Classification classification = e.Classification;
		
		// if a classification was received
		if (classification != null)
		{
			// if the classification is in the form of a StateLogs
			if (classification.LogType == FaceReaderAPI.Data.LogType.StateLog)
			{  
				// show the information
				faceReaderOutput.Add(classification.ToString());
			}
			// if the classification is in the form of a DetailedLog
			else
			{
				// show the information
				faceReaderOutput.Add(classification.ToString());
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Debug.Log (mFaceReaderController.IsConnected.ToString ());
		frameCounter += 1;
		if (frameCounter == 600)
		{
			mFaceReaderController.StartLogSending(FaceReaderAPI.Data.LogType.DetailedLog);
		}
		if (frameCounter == 1200)
		{
			mFaceReaderController.StartAnalyzing();
		}
		
		//Debug.Log("Reading your face");
		//Debug.Log (faceReaderOutput.Count());
		//Debug.Log (faceReaderOutput.Last ());
	}
	
	void OnApplicationQuit()
	{
		Debug.Log ("What");
		mFaceReaderController.StopAnalyzing();
		mFaceReaderController.Dispose ();
		mFaceReaderController = null;
	}
}
