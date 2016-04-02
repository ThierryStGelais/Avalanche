using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SledScript : MonoBehaviour
{
    struct Character
    {
        float stamina;
        float maxStamina;
        public string controller;

        public Character(float stamina, string controller)
        {
            this.stamina = stamina;
            this.maxStamina = stamina;
            this.controller = controller;
        }

        //Get
        public float GetStamina() { return stamina; }

        //Useful functions
        public void RemoveStam(float staminaRemoved)
        {
            if (stamina - staminaRemoved >= 0) stamina -= staminaRemoved; //can't remove past 0
            else Debug.Log("Not Enough Stamina");
        }

        public void AddStam(float addedStam)
        {
            if (stamina + addedStam < maxStamina) stamina += addedStam; //can't add past max Stamina
            else stamina = maxStamina;
        }
    };

    //Characters
    private float maxStamina = 100;
    public float stamRecoveryRate = 0.5f;
    private float nextStaminaRecovery = 0.0f;
    private float nextSwitch = 0.0f;
    private float switchRate = 3.0f;
    private bool canSwitch = true;
    private Character charInFront;
    private Character charInBack;
    private GameObject avalanche;
    private float normalHeight;

    //Acceleration Stats
    public float maxSpeed;
    public float acceleration = 0.5f;
    public float speedAdded = 7.5f;
    public float maxSpeedAdded = 8f;
    private float accStamRemoved = 0.25f; //Changes the closer the players are to the avalanche
    private float accStamRemovedPush = 20.0f;
    private float accelerationPrepAnimTime = 0.5f;
    private float accelerationDownAnimTime = 1f;
    private float endDownAnim = 0.0f;
    private float endPrepAnim = 0.0f;
    private float nextPowerPushTick = 0.0f;
    private float maxPowerPushTick = 0.5f;
    private bool AnimPreped = false;
    private bool charging = false, released = false, turning = false;
    private bool switched = false, switching = false;

    //Horizontal Speed Stats
    public float horizontalSpeedForce = 2f;
    public float maxHorizontalSpeed;

    //Horizontal Speed Help (Drift Help)
    public float TurnRateLeft = 0.5f;
    public float TurnRateRight = 0.5f;

    private float startingRotation;

    private Animator anims;


    //UI
    public Slider Slider_Char1;
    public Slider Slider_Char2;

    //New Acceleration
    float timeCharged = 0.0f;
    float timePressed = 0.0f;
    float timeForBoost = 1.0f;
    float boostModifier = 0.0f;
    public float baseSpeed = 10.0f;
    public float movSpeed = 0.0f;
    float baseDist;
    public float maxWOBoost = 200.0f;
    float jumpedAt = 0.0f;
    public bool finish = false;

    //Turn
    bool frontTurning = false, backTurning = false;
    float frontR, backR;
    float backRStamTime = 0;
    int i = 1;

    private BoxCollider boxC;
    private Vector3 boxCCenter, boxCJumpCenter;


    void Start()
    {
        charInFront = new Character(maxStamina, "Controller1_");
        charInBack = new Character(maxStamina, "Controller2_");
        avalanche = GameObject.FindGameObjectWithTag("Avalanche");
        startingRotation = transform.rotation.y;
        //rigidbody.velocity = new Vector3(0, 0, baseSpeed);
        movSpeed = baseSpeed;
        baseDist = Mathf.Abs(avalanche.transform.position.z - transform.position.z);
        anims = GetComponentInChildren<Animator>();
        normalHeight = transform.position.y;
        Slider_Char1 = GameObject.Find("SliderP1").GetComponent<Slider>();
        Slider_Char2 = GameObject.Find("SliderP2").GetComponent<Slider>();

        boxC = GetComponent<BoxCollider>();
        boxCCenter = boxC.center;
        boxCJumpCenter = new Vector3(boxC.center.x, boxC.center.y + 5, boxC.center.z);
    }

    void Update()
    {
        if (rigidbody.isKinematic) return;

        //Debug.Log(anims.GetBool("Boost"));
        //Switch character when both player press the button
        if (!charging & !turning & canSwitch & Input.GetButton("Controller1_Switch") & Input.GetButton("Controller2_Switch"))
        {
            SwitchCharPosition();
            switching = true;
            endPrepAnim = Time.time;
        }
        if (Time.time >= endPrepAnim + 1.0f)
        {
            switching = false;
        }
        if (Time.time >= nextSwitch)
        {
            nextSwitch = Time.time + switchRate;
            canSwitch = true;

        }

        if (Time.time >= jumpedAt + 1.00f)
        {
            anims.SetBool("jump", false);
        }
        //Accelerate();

        if (!turning & !switching & Input.GetButtonDown(charInBack.controller + "Acceleration") & !charging)
        {
            charging = true;
            timePressed = Time.time;
        }
        if (!turning & !switching & Input.GetButtonUp(charInBack.controller + "Acceleration"))
        {
            charging = false;
            timeCharged = Time.time - timePressed;
            Boost(timeCharged);
            timeCharged = 0.0f;
        }

        anims.SetBool("Charge", charging);

        if (!charging & !switching & Input.GetAxis(charInBack.controller + "Turn") < -0.5f & charInBack.GetStamina() >= accStamRemoved)
        {
            anims.SetBool("Boost", true);
            turning = true;
            TurnRateLeft = 1.0f;
            TurnRateRight = 0.195f;
            charInBack.RemoveStam(accStamRemoved);
            Debug.Log(charInBack.GetStamina());

        }
        else if (!charging & !switching & Input.GetAxis(charInBack.controller + "Turn") > 0.5f & charInBack.GetStamina() >= accStamRemoved)
        {
            anims.SetBool("Boost", true);
            turning = true;
            TurnRateRight = 1.0f;
            TurnRateLeft = 0.195f;
            charInBack.RemoveStam(accStamRemoved);
            Debug.Log(charInBack.GetStamina());


        }
        else
        {
            anims.SetBool("Boost", false);
            turning = false;
            if (charInBack.GetStamina() < accStamRemoved)
                Debug.Log("Not enough stamina!" + charInBack.GetStamina());
            TurnRateLeft = 0.25f;
            TurnRateRight = 0.25f;

        }

        if (!switched)
        {
            Slider_Char1.value = maxStamina - charInBack.GetStamina();
            Slider_Char2.value = maxStamina - charInFront.GetStamina();
        }
        else
        {
            Slider_Char2.value = maxStamina - charInBack.GetStamina();
            Slider_Char1.value = maxStamina - charInFront.GetStamina();
        }
    }

    void FixedUpdate()
    {
        if (rigidbody.isKinematic) return;

        if (anims.GetCurrentAnimationClipState(1).Length > 0 && anims.GetCurrentAnimationClipState(1)[0].clip.name == "jump")
            boxC.center = boxCJumpCenter;
        else
            boxC.center = boxCCenter;


        //Character in front recovers stamina on interval
        if (Time.time >= nextStaminaRecovery)
        {
            nextStaminaRecovery = Time.time + stamRecoveryRate;
            charInFront.AddStam(0.05f);
            nextStaminaRecovery = 0;
        }



        /*if (charging)
        {
            if (speedAdded < maxSpeedAdded)
            {
                nextPowerPushTick += Time.fixedDeltaTime;

                if (nextPowerPushTick >= maxPowerPushTick)
                {
                    speedAdded += 3.2f; //More speed when released
                    charInBack.RemoveStam(accStamRemoved); //Remove more the longer it takes
                    nextPowerPushTick = 0;
                }
            }
        }
        else if (released && charInBack.GetStamina() - accStamRemoved > 0 && AnimPreped && endPrepAnim < Time.time) //Acceleration if character has enough stamina
        {
            nextPowerPushTick = 0;
            charging = false;
            endDownAnim = Time.time + accelerationDownAnimTime; //End of the anim to prepare again
            
            rigidbody.velocity = new Vector3(rigidbody.velocity.x,
                                                 rigidbody.velocity.y,
                                                 rigidbody.velocity.z + speedAdded);

            charInBack.RemoveStam(accStamRemovedPush); //Bigger amount when pushing
            speedAdded = 3.2f;
            AnimPreped = false; //THe animation isnt ready
            released = false;
        }*/
        /*else if (currentSpeed > normalSpeed) //Deceleration if nothing
        {
            currentSpeed -= speedDecay;
        }
        else
        {
            currentSpeed = normalSpeed;
        }*/



        if (!finish & movSpeed < maxWOBoost)
        {
            movSpeed += acceleration * (maxSpeed / movSpeed);
        }
        /*else if (rigidbody.velocity.z > maxSpeed)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x,
                                             rigidbody.velocity.y,
                                             rigidbody.velocity.z + Time.fixedDeltaTime * -acceleration);
        }*/
        else if (!finish & movSpeed > maxSpeed)
        {
            movSpeed -= 5 * acceleration;
        }
        else if (!finish)
        {
            movSpeed -= 1.5f* acceleration;
        }
        else if(finish && movSpeed>= 0+5*acceleration)
        {
            movSpeed -= 5 * acceleration;
        }
        else
        {
            movSpeed = 0;
        }


        if (transform.position.y > normalHeight)
        {
            transform.Translate(0, -0.75f, 0);
            if (transform.position.y < normalHeight)
                transform.position = new Vector3(transform.position.x, normalHeight, transform.position.z);
        }

        //rigidbody.AddForce(transform.forward * currentSpeed);
        rigidbody.velocity = transform.forward * movSpeed;
        // rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, rigidbody.velocity.z + acceleration);
        Mouvement();
        //STurn();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Jump")
        {
            anims.SetBool("jump", true);
            jumpedAt = Time.time;
        }

    }
    void jumpEnd()
    {
        anims.SetBool("jump", false);
    }


    //-------------------------------------------------------------------------------------------------
    //  Mouvement of the sled, the character in front controls the mouvement
    //------------------------------------------------------------------------------------------------
    void Mouvement()
    {
        float h = Input.GetAxis(charInFront.controller + "Horizontal");

        anims.SetFloat("Steer", h);
        //Debug.Log(h);
        //if(transform.eulerAngles.y<=120.0f | transform.eulerAngles.y>=240)
        if (h > 0)
            transform.Rotate(transform.up, TurnRateLeft * h);
        else if (h < 0)
            transform.Rotate(transform.up, TurnRateRight * h);

        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, ClampAngle(transform.eulerAngles.y, 240.0f, 120.0f), transform.eulerAngles.z);
        ClampRotation(-120.0f, 120.0f, 0);
    }

    //-------------------------------------------------------------------------------------------------
    //  Sled descent speed modifier
    //------------------------------------------------------------------------------------------------
    /*  void Accelerate()
      {
          if (Input.GetButtonDown(charInBack.controller + "Acceleration") && endDownAnim < Time.time && !AnimPreped) //Animation going up to charge the push
          {
              endPrepAnim = Time.time + accelerationPrepAnimTime; // Set the animation time to get boost
              AnimPreped = true;
              charging = true;
          }
          else
          {
              charging = false;
              if (Input.GetButtonUp(charInBack.controller + "Acceleration"))
                  released = true;
          }
      }*/


    //-------------------------------------------------------------------------------------------------
    //  Switch character position on sled
    //------------------------------------------------------------------------------------------------
    void SwitchCharPosition()
    {
        Debug.Log("Front:" + charInFront.controller + " Back:" + charInBack.controller);
        anims.SetTrigger("Switch");
        Character temp = charInFront;
        charInFront = charInBack;
        charInBack = temp;
        canSwitch = false;
        switched = !switched;
        Debug.Log("Front:" + charInFront.controller + " Back:" + charInBack.controller);
    }

    //-------------------------------------------------------------------------------------------------
    //  Switch character position on sled
    //------------------------------------------------------------------------------------------------
    /* void STurn()
     {
         float rotationY = transform.eulerAngles.y + 180;

         // FRONT
         float frontH = Input.GetAxis(charInFront.controller + "Turn");

         if (!frontTurning)
         {
             if (frontH > 0) //Turn Right
             {
                 frontR = ClampAngle(transform.rotation.y - TurnRate, 270, 90);
                 transform.Rotate(transform.rotation.x, frontR, transform.rotation.z);
                 frontTurning = true;
             }
             else if (frontH < 0) //Turn Left
             {
                 frontR = ClampAngle(transform.rotation.y + TurnRate, 270, 90);
                 transform.Rotate(transform.rotation.x, frontR, transform.rotation.z);
                 frontTurning = true;
             }
         }
         else if (frontH == 0)
         {
             transform.Rotate(0, -frontR, 0);
             frontTurning = false;
         }

         // BACK

         float backH = Input.GetAxis(charInBack.controller + "Turn");

         if (!backTurning)
         {
             if (backH > 0) //Turn Right
             {
                 backR = ClampAngle(transform.rotation.y - (TurnRate/2), 315, 45);
                 transform.Rotate(transform.rotation.x, backR, transform.rotation.z);
                 backTurning = true;
             }
             else if (backH < 0) //Turn Left
             {
                 backR = ClampAngle(transform.rotation.y + (TurnRate/2), 315, 45);
                 transform.Rotate(transform.rotation.x, backR, transform.rotation.z);
                 backTurning = true;
             }
         }
         else
         {
             backRStamTime += Time.deltaTime;

             if (backRStamTime >= 0.1f)
             {
                 charInBack.RemoveStam(1);
                 backRStamTime = 0;
             }

             if (backH == 0)
             {
                 transform.Rotate(0, -backR, 0);
                 backTurning = false;
             }
         }
     }*/

    void ClampRotation(float minAngle, float maxAngle, float clampAroundAngle = 0)
    {
        //clampAroundAngle is the angle you want the clamp to originate from
        //For example a value of 90, with a min=-45 and max=45, will let the angle go 45 degrees away from 90

        //Adjust to make 0 be right side up
        clampAroundAngle += 180;

        //Get the angle of the z axis and rotate it up side down
        float y = transform.rotation.eulerAngles.y - clampAroundAngle;

        y = WrapAngle(y);

        //Move range to [-180, 180]
        y -= 180;

        //Clamp to desired range
        y = Mathf.Clamp(y, minAngle, maxAngle);

        //Move range back to [0, 360]
        y += 180;

        //Set the angle back to the transform and rotate it back to right side up
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, y + clampAroundAngle, transform.rotation.eulerAngles.z);
    }

    float WrapAngle(float angle)
    {
        //If its negative rotate until its positive
        while (angle < 0)
            angle += 360;

        //If its to positive rotate until within range
        return Mathf.Repeat(angle, 360);
    }

    void Boost(float charged)
    {

        if (charged >= timeForBoost)
        {
            boostModifier = 5.0f;
        }
        else
        {
            boostModifier = Mathf.Max(charged*2.5f, 1.0f);
        }
        if (charInBack.GetStamina() >= Mathf.CeilToInt(accStamRemovedPush * Mathf.Pow(boostModifier, 2) * (Mathf.Sqrt(Mathf.Abs(avalanche.transform.position.z - transform.position.z)) / (baseDist))))
        {
            anims.SetTrigger("Pousser");
            Debug.Log(charInBack.GetStamina());
            movSpeed += speedAdded * boostModifier * 5.0f;
            charInBack.RemoveStam(Mathf.CeilToInt(accStamRemovedPush * Mathf.Pow(boostModifier, 2) * (Mathf.Sqrt(Mathf.Abs(avalanche.transform.position.z - transform.position.z)) / (baseDist))));
        }
        else
        {
            Debug.Log("Not enough Stamina!" + charInBack.GetStamina());
        }


        //Debug.Log("Before:" + transform.forward*movSpeed);

        //Debug.Log("After:" + transform.forward*movSpeed);
        //Debug.Log("Stamina removed:" + Mathf.CeilToInt(accStamRemovedPush * Mathf.Pow(boostModifier, 2)));
    }

    public void toEnd()
    {
        finish = true;
        acceleration = 0.4f;
    }
}
