using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System;

public class TextManagerTests : MonoBehaviour {
	
	public bool display = false;
	string textType = "";
	string textID = "";
	string text = "";
	string gameinfo_filename = @"Assets/Scripts/TriggeredGameInfo.xml";
	string comm_filename = @"Assets/Scripts/TriggeredComm.xml";
	string[] gameinfos;
	string[] comms;
	
	public Texture2D orbital; 
	
	int stringStart = 14;
	
	InventoryTests inventoryJournal;
	
	void Start()
		// Reads in the files storing the text for the game hints and character communications
	{
		gameinfos = File.ReadAllText (gameinfo_filename).Split (new string[] {"</gameinfo>\n"}, System.StringSplitOptions.None);
		comms = File.ReadAllText (comm_filename).Split (new string[] {"</communic>\n"}, System.StringSplitOptions.None);
		inventoryJournal = GameObject.Find ("InventoryTests").GetComponent<InventoryTests>();
		
		GetTrigger ("gameinfo_pre1");
	}
	
	public void GetTrigger(string triggerName)
		// Called by invisible box colliding with the player
	{
		while (display)
		{
			CycleTextBox();
		}
		string[] TypeAndID = triggerName.Split('_');
		textType = TypeAndID[0];
		textID = TypeAndID[1];
		display = FindText();
		
	}
	
	bool FindText()
		// Finds the right string to display depending on the box reached by the player
	{
		string[] searchable;
		if (textType.Equals ("gameinfo"))
		{
			searchable = gameinfos;	
		} else {
			searchable = comms;
		}
		
		foreach(string item in searchable)
		{
			if (item.Contains(textID))
			{
				text = item.Substring (stringStart+textID.Length+2);
				inventoryJournal.journal += text+"\n\n";
				inventoryJournal.scrollPosition = new Vector2(inventoryJournal.scrollPosition.x, Mathf.Infinity);
				return true;
			}
		}
		return false;		
	}
	
	void CycleTextBox()
		// Allows the player to turn off the string displayed. Links IDs by their number (inventory1, inventory2, etc.) 
		// as well as between character comms and game hints (comm_light1 to gameinfo_light1)
	{
		display = false;
		
		// Assumes no more than 9 strings will be linked
		string tempString = textID.Substring(textID.Length-1);
		int tempInt = Convert.ToInt32 (tempString)+1;
		tempString = Convert.ToString (tempInt);
		
		textID = textID.Substring(0,textID.Length-1)+tempString;		
		display = FindText ();
		
		if (textType.Equals("communic") && !display)
		{
			textType = "gameinfo";
			textID = textID.Substring(0,textID.Length-1)+"1";
			display = FindText ();			
		}
		
	}
	
	void Update()
		
	{
		if (Input.GetButtonDown ("Confirm") && display)
		{
			CycleTextBox();
		}
	}
	
	void OnGUI()
	{
		if (display)
		{
			GUI.skin.box.fontSize = 30;
			GUI.skin.box.alignment = TextAnchor.MiddleCenter;
			GUI.skin.box.wordWrap = true;
			
			Rect labelSize = GUILayoutUtility.GetRect (new GUIContent (text), "box");
			labelSize.center = new Vector2 (Screen.width / 2f, Screen.height / 2 * 1.2f);
			GUI.Label (labelSize, text, "box");
			if (textType.Equals("communic"))
			{
				GUI.DrawTexture(new Rect(20,Screen.height-256-20,256,256), orbital);
			}
		}
	}		
}
