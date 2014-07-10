using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TaskManagement : MonoBehaviour {
	string[,] h2o = new string[,] {{"H"},{"O"},{"H"}};
	string[,] ch4 = new string[,] {{"","H",""}, {"H","C","H"},{"","H",""}};
	
	string[] names;
	List<string[,]> molecules = new List<string[,]>();
	  
	// Use this for initialization
	void Start () {
		molecules.Add(h2o);
		molecules.Add(ch4);
		names = new string[]{"h2o","ch4"};
	}
	
	public string Combine(string[,] tool)
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
					return names[enumerating];
				}
			}
			
			enumerating += 1;
		}
		
		return "None";
	}
}
