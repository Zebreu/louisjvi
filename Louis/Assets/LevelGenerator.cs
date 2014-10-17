using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {
	GameObject proceduralbit1;
	GameObject player;
	Vector3 oldposition;
	Mesh meshToDeform;
	
	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		proceduralbit1 = GameObject.Find ("proceduralbit1");
		oldposition = player.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance(player.transform.position,oldposition) > 60)
		{
			Object.Instantiate(proceduralbit1, player.transform.position, Quaternion.FromToRotation(Vector3.up, Vector3.back));
			oldposition = player.transform.position;
		}
		meshToDeform = proceduralbit1.GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = meshToDeform.vertices;
		
		int chosenvertex = 0;
		int i = 0;
		float minDistance = 99999;
		float distance = 0;
		while (i < vertices.Length)
		{
			Vector3 worldposition = proceduralbit1.transform.TransformPoint(vertices[i]);
			distance = Vector3.Distance(worldposition, player.transform.position);
			if (distance < minDistance)
			{
				chosenvertex = i;
				minDistance = distance;
			}
			i += 1;
		}
		Debug.Log (minDistance);
		Debug.Log (chosenvertex);
		Debug.Log (vertices[chosenvertex].x);
		Debug.Log (vertices[chosenvertex].y);
		Debug.Log (vertices[chosenvertex].z);
		Debug.Log (player.transform.position.x);
		Debug.Log (player.transform.position.y);
		Debug.Log (player.transform.position.z);		
		vertices[chosenvertex].z += -0.1f;

		meshToDeform.vertices = vertices;
		meshToDeform.RecalculateBounds();
		meshToDeform.RecalculateNormals();
		proceduralbit1.GetComponent<MeshCollider>().sharedMesh = null;
		proceduralbit1.GetComponent<MeshCollider>().sharedMesh = meshToDeform;
		
		
		
	}
	
	
}
