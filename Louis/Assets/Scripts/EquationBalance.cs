using UnityEngine;
using System.Collections;

public class EquationBalance : MonoBehaviour {

	public bool display;
	public string equation;   
	public string answer;
	
	//sample: "\u2610 H + \u2610 O \u2192 3 H\u2082O"
	
	void Start()
	{
		display = false;
		equation = "";
		answer = "";
	}
		
	void OnGUI()
	{
		if (display)
		{
			Screen.lockCursor = false;
			GUI.skin.textField.fontSize = 30;
			
			Rect labelSize = GUILayoutUtility.GetRect (new GUIContent (equation), "textField");
			equation = GUI.TextField (new Rect(Screen.width/2f-labelSize.width/2,40,labelSize.width,labelSize.height), equation);
			
			GUI.skin.button.fontSize = 16;
			
			if (GUI.Button (new Rect(Screen.width/2f-50, 90, 100, 40),"Envoyer"))
			{
				Screen.lockCursor = true;
				answer = equation;
				display = false;
			}
		}
	}
}
