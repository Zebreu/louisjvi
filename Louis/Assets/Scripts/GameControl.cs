using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {
	bool toolOpened;
	public Texture2D circle;

	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI()
	{
		GUI.DrawTexture(new Rect((Screen.width - circle.width) / 2, (Screen.height - circle.height) /2, circle.width, circle.height),circle);
	}
}
