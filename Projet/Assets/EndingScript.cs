using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndingScript : MonoBehaviour {

    private Canvas UI;
    public Canvas EndMessage;
    private bool canEnd = false;
    private GameObject avalanche;
    private bool endMode = false;

    void Start()
    {
        UI = GameObject.FindGameObjectWithTag("UI").GetComponent<Canvas>();
        avalanche = GameObject.FindWithTag("Avalanche");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            avalanche.SetActive(false);
            //other.rigidbody.isKinematic = true;
           // if(other.GetComponent<SledScript>().movSpeed >=0)
            //{
                other.GetComponent<SledScript>().toEnd();
            //}

            Instantiate(EndMessage);
            canEnd = true;
        }
    }

    void Update()
    {
        if (canEnd && Input.GetButtonDown("Ending"))
        {
            Application.LoadLevel("ThierryMenu");
        }
    }
}
