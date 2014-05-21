using UnityEngine;
using System.Collections;

public class PlayerLook : MonoBehaviour {

	public float sensibility = 2.5f;
	public GameObject body;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		body.transform.Rotate (new Vector3 (0, Input.GetAxis ("Mouse X") * sensibility, 0), Space.World);
		transform.Rotate (new Vector3 (-Input.GetAxis ("Mouse Y") * sensibility, 0, 0));
	}
}
