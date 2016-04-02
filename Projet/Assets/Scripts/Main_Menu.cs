using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Main_Menu : MonoBehaviour {

    public Canvas HowToPlay;

	void Start () 
    {
        HowToPlay.enabled = false;
	}

    public void quit()
    {
        Application.Quit();
    }

    public void newGame()
    {
        Application.LoadLevel("Main");
    }

    public void howToPlay()
    {
        HowToPlay.enabled = true;
    }

    public void BackToMenu()
    {
        HowToPlay.enabled = false;
    }
}
