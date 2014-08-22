using UnityEngine;
using System.Collections;

public class PlayerLook : MonoBehaviour {

	public float sensibility = 2.5f;
	GameObject body;
	GameObject headlight;
	Inventory inventory; 
	
	// Use this for initialization
	void Start () {
		body = GameObject.Find("Player");
		GameObject inventoryObject = GameObject.Find("Inventory");
		headlight = GameObject.Find ("Headlight"); 
		inventory = inventoryObject.GetComponent<Inventory>();		
		
		headlight.SetActive (false);
		if (transform.position.x > 250.0f)
		{
			headlight.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!inventory.inventoryOpen)
		{
			body.transform.Rotate (new Vector3 (0, Input.GetAxis ("Mouse X") * sensibility, 0), Space.World);
			transform.Rotate (new Vector3 (-Input.GetAxis ("Mouse Y") * sensibility, 0, 0));
		}
		if (Input.GetButtonDown ("Turn headlight on/off"))
		{
			if (headlight.activeSelf)
			{
				headlight.SetActive(false);
			} else {
				headlight.SetActive(true);
			}
		}
	}
}
