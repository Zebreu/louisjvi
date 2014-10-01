﻿using UnityEngine;
using System.Collections;

public class Launch : MonoBehaviour {
	bool launched;
	float toAdd;
	GameObject player;
	// Use this for initialization
	void Start () {
		launched = false;
		player = GameObject.Find ("Player");
	}
	
	public void Launching()
	{
		launched = true;
		toAdd = 0.1f;
		Destroy(player.GetComponent<PlayerMove>());
		
		Destroy(player.GetComponent<PlayerLook>());
		
		GameObject inventory = GameObject.Find ("Inventory");
		inventory.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if (launched)
		{
			transform.Translate(new Vector3(0f,toAdd,0f));
			Vector3 toLookAt = transform.position;
			toLookAt.y = player.transform.position.y+toLookAt.y/2;
			
			Camera.main.transform.LookAt(toLookAt);
			toAdd += 0.001f;
			
		}
	}
}
