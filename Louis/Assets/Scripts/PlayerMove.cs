using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {
	public float speed;
	public float moregravity;
	public float jumpSpeed;
	bool grounded = false;

	// Use this for initialization
	void Start () {
		//Physics.gravity = new Vector3 (0, -moregravity,0);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		rigidbody.isKinematic = false;
		Vector3 targetVelocity = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));

		if (targetVelocity.magnitude > targetVelocity.normalized.magnitude) 
		{
			targetVelocity = targetVelocity.normalized; // Prevents a faster movement when two keys are pressed
		} 

		float modifiedspeed = speed;

		if (Input.GetButton ("Run")) // Running, the key must be held down
		{
			modifiedspeed *= 1.5f;
		}

		targetVelocity = transform.TransformDirection (targetVelocity);
		targetVelocity *= modifiedspeed*Time.deltaTime;

		rigidbody.MovePosition(rigidbody.position+targetVelocity); // Alternative movement 
		//rigidbody.AddForce (targetVelocity, ForceMode.VelocityChange);

		if (Input.GetButton("Jump") && grounded) // Jumping, the key must be pressed once, won't trigger again until released
		{
			rigidbody.AddForce (new Vector3(0,jumpSpeed,0),ForceMode.VelocityChange);
		}

		if (!grounded)
		{
			rigidbody.AddForce (new Vector3(0,-moregravity,0), ForceMode.VelocityChange);
		}

		grounded = false;
	}

	void OnCollisionStay()
	{
		grounded = true;
	}

}
