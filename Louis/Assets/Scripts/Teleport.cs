using UnityEngine;
using System.Collections;

public class Teleport : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter (Collider col)
	{
		col.gameObject.transform.position = new Vector3(-45f,14f,68);
	}
}
