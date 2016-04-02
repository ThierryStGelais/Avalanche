using UnityEngine;
using System.Collections;

public class Ice_Obstacle : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("On Ice");
            /*other.rigidbody.drag = 0.0f;
            other.rigidbody.angularDrag = 0.0f;*/
            //other.rigidbody.AddForce(0, 0, 5000f);
            if (other.GetComponent<SledScript>().movSpeed < other.GetComponent<SledScript>().maxSpeed)
            other.GetComponent<SledScript>().movSpeed += 50.0f;
            /*other.GetComponent<Basic_Controller>().force -= 0.33f; ;*/
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("not on ice");
            //other.rigidbody.AddForce(0, 0, -5000f);
            // other.GetComponent<Basic_Controller>().force = 5.0f;
            //other.rigidbody.drag = 3.0f;
            //other.rigidbody.angularDrag = 0.05f;
        }
    }


}
