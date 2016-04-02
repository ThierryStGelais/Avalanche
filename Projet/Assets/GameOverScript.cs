using UnityEngine;
using System.Collections;

public class GameOverScript : MonoBehaviour {

    void Update()
    {
        if (Input.GetButtonDown("Ending"))
        {
            Application.LoadLevel("ThierryMenu");
        }
    }
}
