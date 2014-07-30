using UnityEngine;
using System.Collections;

public class BoxTextTrigger : MonoBehaviour {

	TextTriggering receiver;
	
	void Start()
	{
		GameObject receiverObject = GameObject.Find ("TextManager");
		receiver = receiverObject.GetComponent<TextTriggering>();
	}
	
	void OnTriggerEnter(Collider other) 
	{
		if (other.name.Equals("Player"))
		{
			receiver.GetTrigger(name);
			Destroy (gameObject);
		}
	}
}
