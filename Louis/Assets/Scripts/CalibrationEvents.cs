﻿using UnityEngine;
using System.Collections;

public class CalibrationEvents : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Confirm"))
		{
			Application.LoadLevel ("cavernScene");
		}
	}
}