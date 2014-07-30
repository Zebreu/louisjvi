using UnityEngine;
using System.Collections.Generic;
using System;

public class Inventory : MonoBehaviour {
	TaskManagement taskManagement;
	bool toolOpen = false;
	public bool inventoryOpen = false;
	
	public Texture2D circle;
	public Camera mainCamera;
	public string seenObject = "";
	
	private bool useBackup = false;
	
	private int inventoryGrid = -1;
	
	Dictionary<string, int> backupInventory;
	Dictionary<string, int> inventory = new Dictionary<string, int>();
	string[] inventoryArray;
	
	int toolSizeX = 6;
	int toolSizeY = 4;
	
	private string[,] toolContents;
	
	int buttonSize = 80;
	int spacing = 3;	  
	int offsetX;
	int offsetY;
	
	// Unicode characters for subscripts used in compound formulas
	char c2 = '\u2082';
	//char c3 = '\u2083';
	char c4 = '\u2084';
	
	void Start () {
		Screen.lockCursor = true;
		GameObject tasks = GameObject.Find ("Tasks");
		taskManagement = tasks.GetComponent<TaskManagement>();

	}
	
	void CalculateOffset()
	{
		int totalwidth = toolSizeX*(buttonSize+spacing);
		int totalheight = toolSizeY*(buttonSize+spacing);
		offsetX = (Screen.width-totalwidth)/2;
		offsetY = (Screen.height-totalheight)/2;
	}
		
	void InventoryFill(string name)
	/// <summary>
	/// Adds the chemical element to the inventory depending on the selected object 
	/// </summary>
	{
		string symbol = "";
		int number = 0;
		
		if (name.Equals("Oxygen tank"))
		{
			symbol = "O";
			number = 4;
		}
		
		if (name.Equals("Hydrogen tank"))
		{
			symbol = "H";
			number = 4;
		}
		
		if (name.Equals ("Carbon"))
		{
			symbol = "C";
			number = 2;
		}
		
		if (name.Equals ("h2o"))
		{
			symbol = "H"+c2+"O";
			number = 1;
		}
		
		if (name.Equals ("ch4"))
		{
			symbol = "C"+"H"+c4;
			number = 1;
		}
		
		if (inventory.ContainsKey(symbol))
		{
			inventory[symbol] += number;
		} else 
		{
			inventory.Add (symbol, number);
		}
		
		Debug.Log (inventory[symbol]+" "+symbol);
	}

	void Selection()
	/// <summary>
	/// Finds the object under the cursor (at the center of the screen)
	/// </summary>
	{
		if (Screen.lockCursor) 
		{
			Ray ray = mainCamera.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.0f));
			RaycastHit hit;
			if ((Physics.Raycast (ray, out hit)) && (hit.transform.tag == "Resource"))
			{
				seenObject = hit.transform.name;
				
				// Always removes the last two characters of a string, which should be a whitespace and a prefab ID number.
				seenObject = seenObject.Remove (seenObject.Length-2);		
				
				if (Input.GetMouseButton(0)  && (hit.distance < 3.0f))
				{
					InventoryFill(seenObject);
					Destroy(hit.transform.gameObject);					
				}
			}
			else 
			{
				seenObject = "";
			}
		}
	}

	void InventoryToggle()
	{
		if (!inventoryOpen)
		{
			inventoryOpen = true;
			Screen.lockCursor = false;
			
		} else {
			inventoryOpen = false;
			toolOpen = false;
			inventoryGrid = -1;
			Screen.lockCursor = true;
			if (useBackup) 
			{
				inventory = new	Dictionary<string, int>(backupInventory);
			}
		}	
	}
	
	void ToolToggle()
	{
		if (!toolOpen)
		{
			toolOpen = true;
			inventoryOpen = true;
			Screen.lockCursor = false;
			
			//Creates an empty tool
			toolContents = new string[toolSizeX,toolSizeY];
			for(int i = 0; i < toolSizeX; i++)
			{
				for(int j = 0; j< toolSizeY; j++)
				{ 
					toolContents[i,j] = "";
				}
			}	
			CalculateOffset();
			useBackup = true;
			backupInventory = new Dictionary<string, int>(inventory);
		} else {
			toolOpen = false;
			inventoryOpen = false;
			Screen.lockCursor = true;
			inventoryGrid = -1;
			if (useBackup) 
			{
				inventory = new	Dictionary<string, int>(backupInventory);
			}
		}	
	}
	
	void Update () {
		Selection ();
		
		if (Input.GetButtonDown("Open Inventory"))
		{
			InventoryToggle();
		}
		
		if (Input.GetButtonDown ("Open Tool"))
		{
			ToolToggle ();
		}
		if (Input.GetButtonDown ("Combine") && toolOpen)
		{
			string compound = taskManagement.Combine (toolContents);
			if (compound == "None")
			{
				Debug.Log("Not available");
			} else {
				Debug.Log (compound);
				InventoryFill(compound);
				useBackup = false;
				ToolToggle ();
				ToolToggle ();
			}
		}
	}
	
	void displayInventory()
	{
		inventoryArray = new string[inventory.Keys.Count];
		string toAdd = "";
		
		int index = 0;
		foreach (KeyValuePair<string,int> item in inventory)
		{
			toAdd = item.Value+" "+item.Key;
			inventoryArray[index] = toAdd;
			index += 1;
		}
		
		GUILayout.BeginArea (new Rect(0.0f,Screen.height/1.2f,Screen.width,100.0f));
				
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		inventoryGrid = GUILayout.SelectionGrid(inventoryGrid, inventoryArray,inventory.Keys.Count);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea ();
	}
	
	void displayTool()
	{
		displayInventory();
		
		GUI.skin.box.fontSize = 50;
		GUI.skin.box.alignment = TextAnchor.MiddleCenter;
		for(int i = 0; i < toolSizeX; i++)
		{
			for(int j = 0; j < toolSizeY; j++)
			{
				if( GUI.Button( new Rect(offsetX+i*(buttonSize+spacing), offsetY+j*(buttonSize+spacing), buttonSize, buttonSize), toolContents[i,j],GUI.skin.box))
				{
					if (inventoryGrid != -1)
					{
						
						string[] items = inventoryArray[inventoryGrid].Split ();
						string symbol = items[items.Length-1];
						if (symbol.Length > 2){
							Debug.Log ("You cannot use compounds like that");
						}
						else if (inventory[symbol] > 0)
						{
							toolContents[i,j] = symbol;
							inventory[symbol] += -1;
						}
					}
				}
			}
		}
		
	}
	
	//

	void OnGUI()
	{		
	
		GUI.skin.box.fontSize = 50;
		GUI.skin.box.alignment = TextAnchor.MiddleCenter;
		GUI.skin.box.wordWrap = false;
		
		GUI.skin.button.stretchWidth = false;
		GUI.skin.button.fontSize = 50;
		
		if (seenObject != "") 
		{
			Rect labelSize = GUILayoutUtility.GetRect (new GUIContent (seenObject), "box");
			labelSize.center = new Vector2 (Screen.width / 2f, Screen.height / 2 * 0.8f);
			GUI.Label (labelSize, seenObject, "box");
		}
		
		if (toolOpen)
		{
			displayTool();
		} else {
			if (inventoryOpen)
			{
				displayInventory();
			} else {
				GUI.DrawTexture(new Rect((Screen.width) / 2 - 18, (Screen.height) /2 - 18, circle.width, circle.height),circle);
				// Value of 18 seems best to center the crosshair, larger than Unity's calculated width (16*2) and smaller than the real texture's width (20*2)
			}
		}
	}
}
