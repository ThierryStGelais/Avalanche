using UnityEngine;
using System.Collections;

public class Basic_Controller : MonoBehaviour {


    public float force;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        rigidbody.AddForce(transform.forward * Input.GetAxis("Vertical") * 100*force);
        //rigidbody.AddForce(transform.right * Input.GetAxis("Horizontal") * 100*force);
        transform.RotateAround(transform.up, (0.05f * Input.GetAxis("Horizontal")));
	}
}
