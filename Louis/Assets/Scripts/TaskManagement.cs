using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class TaskManagement : MonoBehaviour {
	string[,] h2o = new string[,] {{"H"},{"O"},{"H"}};
	List<int[]> h2obonds1 = new List<int[]>{new int[4]{0,0,1,0}, new int[4]{1,0,2,0}};
	Dictionary<string,List<int[]>> h2obonds = new Dictionary<string, List<int[]>>();
	
	string[,] ch4 = new string[,] {{"","H",""}, {"H","C","H"},{"","H",""}};
	List<int[]> ch4bonds1 = new List<int[]>{new int[4]{1,0,1,1}, new int[4]{1,1,2,1}, new int[4]{1,1,1,2}, new int[4]{1,1,0,1}};
	Dictionary<string,List<int[]>> ch4bonds = new Dictionary<string, List<int[]>>();
	
	string[,] h2so4 = new string[,] {{"","H",""},{"","O",""}, {"O","S","O"},{"","O",""},{"","H",""}};
	List<int[]> h2so4bonds1 = new List<int[]>{new int[4]{0,1,1,1}, new int[4]{1,1,2,1}, new int[4]{2,1,3,1}, new int[4]{3,1,4,1}};
	List<int[]> h2so4bonds2 = new List<int[]>{new int[4]{2,1,2,0}, new int[4]{2,1,2,2}};
	Dictionary<string,List<int[]>> h2so4bonds = new Dictionary<string, List<int[]>>();
	
	string[,] ccl2f2v1 = new string[,] {{"","F",""}, {"Cl","C","Cl"},{"","F",""}};
	string[,] ccl2f2v2 = new string[,] {{"","Cl",""}, {"F","C","F"},{"","Cl",""}};
	List<int[]> ccl2f2bonds1 = new List<int[]>{new int[4]{1,0,1,1}, new int[4]{1,1,2,1}, new int[4]{1,1,1,2}, new int[4]{1,1,0,1}};
	Dictionary<string,List<int[]>> ccl2f2v1bonds = new Dictionary<string, List<int[]>>();
	Dictionary<string,List<int[]>> ccl2f2v2bonds = new Dictionary<string, List<int[]>>();
	
	string[,] ethanol = new string[,] {{"","H",""}, {"H","C","H"}, {"H","C","H"}, {"","O",""}, {"","H",""}};
	List<int[]> ethanolbonds1 = new List<int[]>{new int[4]{0,1,1,1}, new int[4]{1,1,2,1}, new int[4]{2,1,3,1}, new int[4]{3,1,4,1}, new int[4]{1,1,1,0}, new int[4]{1,1,2,1}, new int[4]{2,1,2,0}, new int[4]{2,1,2,2}};
	Dictionary<string,List<int[]>> ethanolbonds = new Dictionary<string, List<int[]>>();
	
	Dictionary<string,List<int[]>>[] allbonds;
	string[] names;
	List<string[,]> molecules = new List<string[,]>();
	
	public int progressionIndex;
	//public string[] progression;
	
	GameObject player;
	public Transform textTriggerPrefab;
	
	// Classes used to progress in the game
	
	public Dissolving dissolveClass;
	public MeltWall meltClass;
	List<GameObject> taskTriggers;
	List<string> completedTasks;
	
	// Unicode characters for subscripts used in compound formulas
	char c2 = '\u2082';
	char c3 = '\u2083';
	char c4 = '\u2084';
	
	// Use this for initialization
	void Start () {
		molecules.Add(h2o);
		molecules.Add(ch4);
		molecules.Add (h2so4);
		molecules.Add (ccl2f2v1);
		molecules.Add (ccl2f2v2);
		molecules.Add (ethanol);
		names = new string[]{"h2o","ch4","h2so4","ccl2f2","ccl2f2","ethanol"};
		
		h2obonds.Add ("I",h2obonds1);
		h2obonds.Add ("II", new List<int[]>());
		ch4bonds.Add ("I",ch4bonds1);
		ch4bonds.Add ("II", new List<int[]>());
		h2so4bonds.Add ("I", h2so4bonds1);
		h2so4bonds.Add ("II", h2so4bonds2);
		ccl2f2v1bonds.Add ("I",ccl2f2bonds1);
		ccl2f2v1bonds.Add ("II", new List<int[]>());
		ccl2f2v2bonds.Add ("I",ccl2f2bonds1);
		ccl2f2v2bonds.Add ("II", new List<int[]>());
		ethanolbonds.Add ("I",ethanolbonds1);
		ethanolbonds.Add ("II", new List<int[]>());
																						
		allbonds = new Dictionary<string, List<int[]>>[]{h2obonds,ch4bonds,h2so4bonds,ccl2f2v1bonds,ccl2f2v2bonds,ethanolbonds};
		
		progressionIndex = 0;
		//progression = new string[]{"H"+c2+"O","CH"+c4,"H"+c2+"SO"+c4,"CH"+c4,"CCl"+c2+"F"+c2,"CH"+c3+"CH"+c2+"OH"};
	
		DontDestroyOnLoad(transform.gameObject);
		
		taskTriggers = new List<GameObject>(GameObject.FindGameObjectsWithTag("TaskTrigger"));
		completedTasks = new List<string>();
		dissolveClass = GameObject.Find("debris1").GetComponent<Dissolving>();
		meltClass = GameObject.Find("Task2-Melt").GetComponent<MeltWall>();
		player = GameObject.Find ("Player");
	}
	
	void OnLevelWasLoaded()
	{
		taskTriggers = new List<GameObject>(GameObject.FindGameObjectsWithTag("TaskTrigger"));
		player = GameObject.Find ("Player");
	}
	void CreateNewTrigger(string triggerName)
	{
		Vector3 toSpawn = player.transform.position;
		UnityEngine.Object newTrigger = Instantiate(textTriggerPrefab,toSpawn,Quaternion.identity);
		newTrigger.name = triggerName;
	}
	
	public string Progress(string symbol)
	{
		List<string> triggeredAll = new List<string>();
		foreach (GameObject taskTrigger in taskTriggers)
		{	
			if (player.collider.bounds.Intersects(taskTrigger.collider.bounds))
			{
				triggeredAll.Add (taskTrigger.name);
			}
		}
		
		foreach(string triggered in triggeredAll)
		{
			bool correct = false;
			if (triggered.Equals("WaterTaskTrigger"))
			{
				if (symbol.Equals("H"+c2+"O"))
				{
					GameObject waterwall = GameObject.Find("WaterTask");
					Destroy (waterwall);
					CreateNewTrigger("communic_waterdone1");
					correct = true;
				}
			}
			else if (triggered.Equals("Methane1TaskTrigger"))
			{
				if (symbol.Equals("CH"+c4))
				{
					meltClass.melted1 = true;
					CreateNewTrigger("communic_methanedone1");
					correct = true;
				}
			}
			else if (triggered.Equals("Methane2TaskTrigger"))
			{
				if (symbol.Equals("CH"+c4))
				{
					meltClass.melted2 = true;
					correct = true;
				}
			}
			else if (triggered.Equals("AcidTaskTrigger"))
			{
				if (symbol.Equals("H"+c2+"SO"+c4))
				{
					GameObject acidcube = GameObject.Find ("AcidCube");
					Destroy (acidcube);
					dissolveClass.dissolve = true;
					CreateNewTrigger("communic_aciddone1");
					correct = true;
				}
			}
			else if (triggered.Equals("FreonTaskTrigger"))
			{
				if (symbol.Equals("CCl"+c2+"F"+c2))
				{
					GameObject heatwall = GameObject.Find("HeatTask");
					Destroy (heatwall);
					CreateNewTrigger("communic_heatdone1");
					correct = true;
				}
			}
			else if (triggered.Equals("EthanolTaskTrigger"))
			{
				//if (symbol.Equals("H"+c2+"O"))
				if (symbol.Equals("CH"+c3+"CH"+c2+"OH"))
				{	
					GameObject lander = GameObject.Find ("Lander");
					lander.GetComponent<Launch>().Launching();
					correct = true;
				}
			}
			
			if (correct)
			{
				completedTasks.Add(triggered);
				progressionIndex += 1;
				GameObject toRemove = GameObject.Find (triggered);
				taskTriggers.Remove(toRemove);
				Debug.Log ("Success");
				return "Done";
			}
		}
	
		return "";
	}
	
	public string Combine(string[,] tool, Dictionary<string,List<int[]>> bondsLogic)
	{
		int enumerating = 0;
		foreach (string[,] molecule in molecules)
		{
			int toolx = tool.GetLength(0);
			int tooly = tool.GetLength (1);
			
			int mx = molecule.GetLength(0);
			int my = molecule.GetLength (1);
			//Debug.Log (x+" "+y);
						
			int dx = toolx - mx + 1;
			int dy = tooly - my + 1;
			
			if (dx < 1 || dy < 1) // Such values would mean that the tool is too small for the molecule
			{
				continue;
			}
			
			int trials = dx*dy;
			//Debug.Log ("dx:"+dx);
			//Debug.Log ("dy:"+dy);
			
			// Calculates offsets for the molecule template based on tool window size
			
			int[] offsetx = new int[trials];
			int[] offsety = new int[trials];
			
			IEnumerable<int> offsetRangeX = Enumerable.Range(0,dx);
			IEnumerable<int> offsetRangeY = Enumerable.Range(0,dy);
			
			int offsetindex = 0;
			
			foreach(int offy in offsetRangeY)
			{
				foreach(int offx in offsetRangeX)
				{
					offsetx[offsetindex] = offx;
					offsety[offsetindex] = offy;
					//Debug.Log (offx+" "+offy);
					offsetindex +=1;	
				}
			}			
			
			// Tries to apply the molecule template in the tool window, iterating through possible offsets
			
			for(int i = 0; i < trials; i++)
			{
				bool found = true;
				for(int y = 0; y < my; y++)
				{
					for(int x = 0; x < mx; x++)
					{
						if (!molecule[x,y].Equals(tool[x+offsetx[i],y+offsety[i]]))
						{
							found = false;
							break;
						}
					}
					if (!found)
					{
						break;
					}
				}
				if (found)
				{
					if (compareBonds (bondsLogic, offsetx[i], offsety[i], enumerating))
					{
						return names[enumerating];
					}
				}
			}
			
			enumerating += 1;
		}
		
		return "None";
	}
	
	bool compareBonds(Dictionary<string,List<int[]>> bondsLogic, int startx, int starty, int enumerating)
	// Since the bond configuration assumes the template starts at 0,0, the bonds suggested by the player are offset.
	// If all the correct bonds are found and the number of player-supplied bonds and correct bonds is equal, success is communicated. 
	{
		Dictionary<string, List<int[]>> goodBonds = allbonds[enumerating];
		List<int[]> goodBonds1 = goodBonds["I"];
		List<int[]> goodBonds2 = goodBonds["II"]; 
		
		List<int[]> playerBonds1 = bondsLogic["I"];
		List<int[]> playerBonds2 = bondsLogic["II"];
		
		if (playerBonds1.Count != goodBonds1.Count || playerBonds2.Count != goodBonds2.Count)
		{
			return false;
		}
		
		int bondNumber = goodBonds1.Count+goodBonds2.Count;
		
		int correctBonds = 0; 
		
		foreach(int[] bond in goodBonds1)
		{
			int[] bondPairOpposite = new int[4];
			bondPairOpposite[0] = bond[2];
			bondPairOpposite[1] = bond[3];
			bondPairOpposite[2] = bond[0];
			bondPairOpposite[3] = bond[1];
			
			foreach(int[] playerBondOriginal in playerBonds1)
			{
				int[] playerBond = new int[4];
				playerBond[0] = playerBondOriginal[0]-startx;
				playerBond[1] = playerBondOriginal[1]-starty;
				playerBond[2] = playerBondOriginal[2]-startx;
				playerBond[3] = playerBondOriginal[3]-starty;
				
				if (playerBond.SequenceEqual(bond) || playerBond.SequenceEqual(bondPairOpposite))
				{
					correctBonds+=1;
				}
			}
		}
		// Ugly repetition of code, easy to fix though
		foreach(int[] bond in goodBonds2)
		{
			int[] bondPairOpposite = new int[4];
			bondPairOpposite[0] = bond[2];
			bondPairOpposite[1] = bond[3];
			bondPairOpposite[2] = bond[0];
			bondPairOpposite[3] = bond[1];
			
			foreach(int[] playerBondOriginal in playerBonds2)
			{
				int[] playerBond = new int[4];
				playerBond[0] = playerBondOriginal[0]-startx;
				playerBond[1] = playerBondOriginal[1]-starty;
				playerBond[2] = playerBondOriginal[2]-startx;
				playerBond[3] = playerBondOriginal[3]-starty;
				
				if (playerBond.SequenceEqual(bond) || playerBond.SequenceEqual(bondPairOpposite))
				{
					correctBonds+=1;
				}
			}
		}
		
		if (correctBonds ==  bondNumber)
		{
			return true;
		} else {
			return false;
		}
	}
}
