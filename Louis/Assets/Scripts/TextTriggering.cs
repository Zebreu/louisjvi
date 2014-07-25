using UnityEngine;
using System.Collections;

public class TextTriggering : MonoBehaviour {
	
	public bool display = false;
	public bool turnedOff = false;
	string textType = "";
	string textID = "";
	string text = "";
	
	public void GetTrigger(string triggerName)
	{
		string[] TypeAndID = triggerName.Split("_");
		textType = TypeAndID[0];
		textID = TypeAndID[1]; 
	}
	
	void GetText()
	{
		text = "Testing id "+textID;
	}
	
	void OnGUI()
	{
		if (display && !turnedOff)
		{
			Rect labelSize = GUILayoutUtility.GetRect (new GUIContent (text), "box");
			labelSize.center = new Vector2 (Screen.width / 2f, Screen.height / 2 * 1.2f);
			GUI.Label (labelSize, text, "box");
		}
	}		
}
