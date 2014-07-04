using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {
	bool toolOpen = false;
	public bool inventoryOpen = false;
	
	public Texture2D circle;
	public Camera mainCamera;
	public string seenObject = "";
	
	private int inventoryGrid = -1;
	private int toolGrid = -1;
	
	Dictionary<string, int> inventory = new Dictionary<string, int>();
	
	void Start () {
		Screen.lockCursor = true;
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
			inventory[symbol] = inventory[symbol]+number;
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
			}	
		}
		
		if (Input.GetButtonDown ("Open Tool"))
		{
			if (!toolOpen)
			{
				toolOpen = true;
				inventoryOpen = true;
				Screen.lockCursor = false;
			} else {
				toolOpen = false;
				inventoryOpen = false;
				Screen.lockCursor = true;
				inventoryGrid = -1;
			}	
		}
	}
	
	void displayInventory()
	{
		string[] inventoryArray = new string[inventory.Keys.Count];
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
		
		
		
	}

	void OnGUI()
	{
		GUI.DrawTexture(new Rect((Screen.width) / 2 - 18, (Screen.height) /2 - 18, circle.width, circle.height),circle);
		// Value of 18 seems best to center the crosshair, larger than Unity's calculated width (16*2) and smaller than the real texture's width (20*2)
		
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
			}
		}
	}
}
