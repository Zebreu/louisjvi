using UnityEngine;
using System.Collections;

public class EvaluationTextfield : MonoBehaviour {
	public bool display;
	public string textInput;

	void Start()
	{
		display = false;
		textInput = "Décris ici comment tu as résolu le probleme";
	}

	void OnGUI()
	{
		if (display)
		{
			Screen.lockCursor = false;

			textInput = GUI.TextArea (new Rect(20,20,Screen.width-40,Screen.height/5f), textInput);
			GUI.skin.button.fontSize = 16;
			
			if (GUI.Button (new Rect(Screen.width/2f-50, Screen.height/5f+30, 100, 40),"Envoyer"))
			{
				Screen.lockCursor = true;
				textInput = "Décris ici comment tu as résolu le probleme";
				display = false;
			}
		}
	}
	
}
