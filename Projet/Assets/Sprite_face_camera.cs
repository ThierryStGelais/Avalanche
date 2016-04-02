using UnityEngine;
using System.Collections;

public class Sprite_face_camera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float camY = Camera.main.transform.eulerAngles.y;

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, camY, transform.eulerAngles.z);

        //transform.LookAt(Camera.main.transform, Vector3.up);
	}
}
