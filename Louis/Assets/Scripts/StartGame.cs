using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {
	
	public Texture2D orbital;
	string text;
	
	// Use this for initialization
	void Start () {
		text = "Landing successful! You're an astronaut collecting surface samples, and Commander Arnold (his picture appears below when he talks to you) is waiting for you on the orbiter. However, on your way back to the lander, the ground shakes and darkness engulfs you!";	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Confirm"))
		{
			Application.LoadLevel("cavernScene");
		}
	}
	
	void OnGUI()
	{
		
		GUI.skin.box.fontSize = 30;
		GUI.skin.box.alignment = TextAnchor.MiddleCenter;
		GUI.skin.box.wordWrap = true;
		
		Rect labelSize = GUILayoutUtility.GetRect (new GUIContent (text), "box");
		labelSize.center = new Vector2 (Screen.width / 2f, Screen.height / 3f);
		GUI.Label (labelSize, text, "box");
		
		GUI.DrawTexture(new Rect(20,Screen.height-256-20,256,256), orbital);
	
		
	}		
}
