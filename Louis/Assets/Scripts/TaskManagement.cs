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
	
	Dictionary<string,List<int[]>>[] allbonds;
	string[] names;
	List<string[,]> molecules = new List<string[,]>();
	
	public int progressionIndex;
	public string[] progression;
	
	// Unicode characters for subscripts used in compound formulas
	char c2 = '\u2082';
	//char c3 = '\u2083';
	char c4 = '\u2084';
	
	// Use this for initialization
	void Start () {
		molecules.Add(h2o);
		molecules.Add(ch4);
		names = new string[]{"h2o","ch4"};
		
		h2obonds.Add ("I",h2obonds1);
		h2obonds.Add ("II", new List<int[]>());
		ch4bonds.Add ("I",ch4bonds1);
		ch4bonds.Add ("II", new List<int[]>());
				
		allbonds = new Dictionary<string, List<int[]>>[]{h2obonds,ch4bonds};
		
		progressionIndex = 0;
		progression = new string[2]{"H"+c2+"O","CH"+c4};
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
