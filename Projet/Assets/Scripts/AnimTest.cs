using UnityEngine;
using System.Collections;

public class AnimTest : MonoBehaviour {

    Animator anims;
    bool charging = false;
    bool red = true;

    void Awake()
    {
        anims = GetComponent<Animator>();
    }

    public void clickCharge()
    {
        if (charging)
        {
            anims.SetTrigger("Pousser");
            charging = false;
        }
        else
        {
            anims.SetTrigger("Charger");
            charging = true;
        }
    }

    public void clickCancelCharge()
    {
        if (charging)
        {
            anims.SetTrigger("AnnulerCharge");
            charging = false;
        }
    }

    public void clickSwitch()
    {
        anims.SetTrigger(red ? "SwitchToGreen" : "SwitchToRed");
        red = !red;
    }

    public void clickLeft()
    {
        anims.SetTrigger("LeftSteer");
    }

    public void clickCentre()
    {
        anims.SetTrigger("StopSteer");
    }

    public void clickRight()
    {
        anims.SetTrigger("RightSteer");
    }
}
