using UnityEngine;
using System.Collections;

public class Gravel_Obstacle : MonoBehaviour {

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
            Debug.Log("On Gravel");
            /*other.rigidbody.drag = 0.0f;
            other.rigidbody.angularDrag = 0.0f;*/
			GetComponent<AudioSource>().Play();
            //other.rigidbody.AddForce(0, 0, -2500f);            
            //other.rigidbody.velocity = transform.forward  * Mathf.Lerp(other.rigidbody.velocity.magnitude, other.rigidbody.velocity.magnitude * (0.66f), 0.5f);

            /*other.GetComponent<Basic_Controller>().force -= 0.33f; ;*/
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("On Gravel");
            if (other.GetComponent<SledScript>().movSpeed > other.GetComponent<SledScript>().baseSpeed)
            other.GetComponent<SledScript>().movSpeed -= 10.0f;
           /*if (other.rigidbody.drag < 10)
           {
               other.rigidbody.drag += 0.25f;

           }*/
                //other.GetComponent<SledScript>().force -= 0.33f; ;
        }
    }

    /*void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("not on gravel");
           // other.GetComponent<Basic_Controller>().force = 5.0f;
            other.rigidbody.drag = 3.0f;
        }
    }*/

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Not On Gravel");
            /*other.rigidbody.drag = 0.0f;
            other.rigidbody.angularDrag = 0.0f;*/
            //other.rigidbody.AddForce(0, 0, 2500f);

            /*other.GetComponent<Basic_Controller>().force -= 0.33f; ;*/
        }
    }

}
