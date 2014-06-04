using UnityEngine;
using System.Collections.Generic;

public class GameControl : MonoBehaviour {
	bool toolOpened;
	public Texture2D circle;
	public Camera mainCamera;
	public string seenObject = "";
	//public Transform selected = null;
	List<Transform> inventory = new List<Transform>();

	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;
	}
	
	void InventoryFill()
	{
		//nothing yet
	}

	void Selection()
	/// <summary>
	/// Will find the object under the cursor (at the center of the screen)
	/// </summary>
	{
		if (Screen.lockCursor) 
		{
			Ray ray = mainCamera.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.0f));
			RaycastHit hit;
			if ((Physics.Raycast (ray, out hit)) && (hit.transform.tag == "Resource"))
			{
				seenObject = hit.transform.name;
				if (Input.GetMouseButton(0))
				{
					inventory.Add (hit.transform);
				}
			}
			else 
			{
				seenObject = "";
			}
		}
	}

	void Update () {
		Selection ();
	}

	void OnGUI()
	{
		GUI.DrawTexture(new Rect((Screen.width - circle.width) / 2, (Screen.height - circle.height) /2, circle.width, circle.height),circle);
		if (seenObject != "") 
		{
			Rect labelSize = GUILayoutUtility.GetRect (new GUIContent (seenObject), "box");
			labelSize.center = new Vector2 (Screen.width / 2f, Screen.height / 2 * 0.8f);
			GUI.Label (labelSize, seenObject, "box");
		}
	}
}
