using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {
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
	
	void Start () {
		Screen.lockCursor = true;
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
		
		if (name.CompareTo("OxygenTank") == 0)
		{
			symbol = "O";
			number = 2;
		}
		
		if (name.CompareTo("HydrogenTank") == 0)
		{
			symbol = "H";
			number = 2;
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
				if (Input.GetMouseButton(0)  && (hit.distance < 3.0f))
				{
					InventoryFill(hit.transform.name);
					Destroy(hit.transform.gameObject);					
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
		
		if (Input.GetButtonDown("Open Inventory"))
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
		
		if (Input.GetButtonDown ("Open Tool"))
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
		GUI.skin.button.stretchWidth = false;
		GUI.skin.button.fontSize = 30;
		
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
						if (inventory[symbol] > 0)
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
