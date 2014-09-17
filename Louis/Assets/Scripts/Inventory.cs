using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Inventory : MonoBehaviour {
	TaskManagement taskManagement;
	bool toolOpen = false;
	public bool inventoryOpen = false;
	bool tableOpen = false;
	
	public Texture2D circle;
	public Texture2D bond1;
	public Texture2D bond1horizontal;
	public Texture2D bond2;
	public Texture2D bond2horizontal;
	public Texture2D bond3;
	public Texture2D bond3horizontal;
	Camera mainCamera;
	public string seenObject = "";
	
	public Texture2D periodicTable;
	
	public Texture2D toolDrawing;
	
	private bool useBackup = false;
	
	private int inventoryGrid = -1;
	
	Dictionary<string, int> backupInventory;
	Dictionary<string, int> inventory = new Dictionary<string, int>();
	string[] inventoryArray;
	
	int[] bondPair = new int[4];
	bool newBond;
	Dictionary<string, List<int[]>> bondsLogic;
	Dictionary<Rect, Texture2D> bonds;
	
	int toolSizeX = 6;
	int toolSizeY = 4;
	
	private string[,] toolContents;
	
	int buttonSize = 80;
	int spacing = 3;	  
	int offsetX;
	int offsetY;
	
	string usedCompound;
	
	// Unicode characters for subscripts used in compound formulas
	char c2 = '\u2082';
	char c3 = '\u2083';
	char c4 = '\u2084';
	
	void Start () {
		mainCamera = Camera.main;
	
		Screen.lockCursor = true;
		GameObject tasks = GameObject.Find ("Tasks");
		taskManagement = tasks.GetComponent<TaskManagement>();
		inventory.Add("I",1);
		inventory.Add("II",1);
		
		bonds = new Dictionary<Rect, Texture2D>();
		bondsLogic = new Dictionary<string, List<int[]>>();
		bondsLogic.Add ("I",new List<int[]>());
		bondsLogic.Add ("II",new List<int[]>());		
	
		DontDestroyOnLoad(transform.gameObject);
		
		usedCompound = "";
	}
	
	void OnLevelWasLoaded()
	{
		mainCamera = Camera.main;
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
		
		if (name.Substring(0,2).Equals("Ox"))
		{
			symbol = "O";
			number = 4;
		}
		
		if (name.Substring(0,2).Equals("Hy"))
		{
			symbol = "H";
			number = 16;
		}
		
		if (name.Equals ("Carbone"))
		{
			symbol = "C";
			number = 2;
		}
		
		if (name.Equals ("Fluor"))
		{
			symbol = "F";
			number = 2;
		}
		
		if (name.Equals ("Chlore"))
		{
			symbol = "Cl";
			number= 2;
		}
		
		if (name.Equals ("Soufre"))
		{
			symbol = "S";
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
		
		if (name.Equals("h2so4"))
		{
			symbol = "H"+c2+"SO"+c4;
			number = 1;
		}
		
		if (name.Equals("ccl2f2"))
		{
			symbol = "CCl"+c2+"F"+c2;
			number = 1;
		}
		
		if (name.Equals("ethanol"))
		{
			symbol = "CH"+c3+"CH"+c2+"OH";
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

	void TableToggle()
	{
		if (!tableOpen)
		{
			tableOpen = true;
		}
		else {
			tableOpen = false;
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
			bonds = new Dictionary<Rect, Texture2D>();
			bondsLogic = new Dictionary<string, List<int[]>>();
			bondsLogic.Add ("I",new List<int[]>());
			bondsLogic.Add ("II",new List<int[]>());
			newBond = true;
			
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
			if (usedCompound != "")
			{
				inventory[usedCompound] += -1;
				usedCompound = "";
			}
		}	
	}
	
	void Update () {
		Selection ();
		
		if (Input.GetButtonDown("Open Inventory"))
		{
			TableToggle();
		}
		
		if (Input.GetButtonDown ("Open Tool"))
		{
			ToolToggle ();
		}
		if (Input.GetButtonDown ("Combine") && toolOpen)
		{
			string compound = taskManagement.Combine (toolContents, bondsLogic);
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
		
		GUILayout.BeginArea (new Rect(0.0f,Screen.height/1.4f,Screen.width,250.0f));
				
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		inventoryGrid = GUILayout.SelectionGrid(inventoryGrid, inventoryArray, 5);//inventory.Keys.Count);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea ();
		
		if (inventoryGrid > -1) 
		{
			string[] items = inventoryArray[inventoryGrid].Split ();
			string symbol = items[items.Length-1];
			if (taskManagement.Progress(symbol) == "Done")
			{
				usedCompound = symbol;
				inventory[symbol] += -1;
			}
			
		}
	}
	
	void addBond(Rect bondPosition, string direction, string symbol, int[] bondPair)
	// Creates a bond both for display and logic purposes
	{
		int[] bondPairOpposite = new int[4];
		bondPairOpposite[0] = bondPair[2];
		bondPairOpposite[1] = bondPair[3];
		bondPairOpposite[2] = bondPair[0];
		bondPairOpposite[3] = bondPair[1];
		
		foreach (KeyValuePair<Rect,Texture2D> bond in bonds)
		{
			if (bond.Key.Overlaps(bondPosition))
			{
				bonds.Remove(bond.Key);
				bool replaced = false;
				foreach(KeyValuePair<string,List<int[]>> logicbond in bondsLogic)
				{
					foreach(int[] logicbondposition in logicbond.Value)
					{
						if (bondPair.SequenceEqual(logicbondposition) || bondPairOpposite.SequenceEqual(logicbondposition))
						{
							replaced = true;
							logicbond.Value.Remove (logicbondposition);
							Debug.Log ("replacing!");
							break;
						} 
					}
					if (replaced)
					{
						break;
					}
				}
				break;
			}
		}
		
		if (direction.Equals("vertical"))
		{
			if (symbol.Equals("II"))
			{
				bonds.Add (bondPosition, bond2);
			} else {
				bonds.Add (bondPosition, bond1);
			}
		} else if (symbol.Equals("II"))
		{
			bonds.Add (bondPosition, bond2horizontal);
		} else {
			bonds.Add (bondPosition, bond1horizontal);
		}
	}
	
	void checkBond(Rect position, int[] bondPair, string symbol)
	// Checks the direction in which the bond is created and its position
	{		
		if (bondPair[0] < bondPair[2]) // Left 
		{
			addBond(new Rect(position.x - buttonSize*0.21f, position.y + buttonSize/2f - bond1.width/2f-1, bond1.width, bond1.height) ,"horizontal", symbol, bondPair);	
		}
		else if (bondPair[1] < bondPair[3]) // Upper
		{
			addBond (new Rect(position.x + buttonSize/2f - bond1.width/2f+1, position.y - buttonSize*0.21f, bond1.width, bond1.height) ,"vertical", symbol, bondPair);
		}
		else if (bondPair[0] != bondPair[2]) // Right
		{
			addBond (new Rect(position.x + buttonSize*0.81f, position.y + buttonSize/2f - bond1.width/2f-1, bond1.width, bond1.height) ,"horizontal", symbol, bondPair);
		} else { // Lower
			addBond(new Rect(position.x + buttonSize/2f - bond1.width/2f+1, position.y + buttonSize*0.81f, bond1.width, bond1.height), "vertical", symbol, bondPair);
		}
		
		/*
		Rect bondPosition = new Rect(position.x + buttonSize/2f - bond1.width/2f+1, position.y + buttonSize*0.81f, bond1.width, bond1.height);
		bonds.Add(bondPosition,bond1);
		bondPosition = new Rect(position.x + buttonSize/2f - bond1.width/2f+1, position.y - buttonSize*0.21f, bond1.width, bond1.height);
		bonds.Add(bondPosition,bond1);
		bondPosition = new Rect(position.x + buttonSize*0.81f, position.y + buttonSize/2f - bond1.width/2f-1, bond1.width, bond1.height);
		bonds.Add(bondPosition,bond1horizontal);
		bondPosition = new Rect(position.x - buttonSize*0.21f, position.y + buttonSize/2f - bond1.width/2f-1, bond1.width, bond1.height);
		bonds.Add(bondPosition,bond1horizontal);
		*/
	}
	
	void displayBonds()
	{
		foreach (KeyValuePair<Rect,Texture2D> bond in bonds)
		{
			GUI.DrawTexture(bond.Key, bond.Value);
		}
	}
	
	void displayTool()
	{
		displayInventory();
		
		GUI.DrawTexture(new Rect(0,0,2049,823), toolDrawing); //Original size: (0,0,2049,823) 
		
		GUI.skin.box.fontSize = 50;
		GUI.skin.box.alignment = TextAnchor.MiddleCenter;
		for(int i = 0; i < toolSizeX; i++)
		{
			for(int j = 0; j < toolSizeY; j++)
			{
				Rect position = new Rect(offsetX+i*(buttonSize+spacing), offsetY+j*(buttonSize+spacing), buttonSize, buttonSize);
				if( GUI.Button( position, toolContents[i,j],GUI.skin.box))
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
							if (symbol.Equals("I") || symbol.Equals("II")) // Adds covalent bonds between atoms
							{
								if (!newBond)
								{
									bondPair[2] = i;
									bondPair[3] = j;
									checkBond (position,bondPair,symbol);
									
									bondsLogic[symbol].Add (bondPair);
									
									foreach(KeyValuePair<string, List<int[]>> item in bondsLogic)
									{
										Debug.Log (item.Key);
										foreach(int[] listitem in item.Value)
										{
											Debug.Log (listitem[0]+","+listitem[1]+","+listitem[2]+","+listitem[3]);
										}
									}
									
									bondPair = new int[4];
									newBond = true;
								} else {
									bondPair[0] = i;
									bondPair[1] = j;
									newBond = false;
								}
							} else {
								toolContents[i,j] = symbol;
								inventory[symbol] += -1;
							}
						}
					}
				}
			}
		}
		displayBonds ();
		
	}
	
	//

	void OnGUI()
	{		
	
		GUI.skin.box.fontSize = 50;
		GUI.skin.box.alignment = TextAnchor.MiddleCenter;
		GUI.skin.box.wordWrap = false;
		
		GUI.skin.button.stretchWidth = false;
		GUI.skin.button.fontSize = 50;
		
		if (tableOpen)
		{
			GUI.DrawTexture(new Rect(0,0,1280,773), periodicTable);
		}
		
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
