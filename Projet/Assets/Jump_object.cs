using UnityEngine;
using System.Collections;

public class Jump_object : MonoBehaviour {
		
	public GameObject Character;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			other.GetComponent<SledScript>().movSpeed += 50.0f;
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			Debug.Log("Jump Mothafucka"); 
			// Ajouter plancher et gravité!!!!
			//other.rigidbody.AddForce(Vector3.up*200.0f);

           //other.transform.Translate(0, 25, 0);
		}
	}
	
}