using UnityEngine;
using System.Collections;

public class SpriteRender : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Renderer>().sortingOrder = Mathf.FloorToInt((-1) * Mathf.Abs(transform.position.z - Camera.main.transform.position.z));
	}
	
	// Update is called once per frame
	void Update () {
        
        //renderer.sortingOrder =Mathf.FloorToInt((-1) * Mathf.Abs(transform.position.z - Camera.main.transform.position.z));
	}
}
