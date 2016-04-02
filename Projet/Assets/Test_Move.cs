using UnityEngine;
using System.Collections;

public class Test_Move : MonoBehaviour {

	
	public float speed;

	void Start()
	{
		if(speed == 0)
		{
			speed = 10;
		}
	}
	
	void Update()
	{

	}
	
	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		
		rigidbody.velocity = new Vector3 (moveHorizontal*speed, 0.0f, moveVertical*speed);

	}
	
}