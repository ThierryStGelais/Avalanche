using UnityEngine;
using System.Collections;

public class Avalanche_Move : MonoBehaviour {
	float speed = 120.0f;
    public float accel = 1.0f;
	GameObject Player;
	AudioSource Audio;
    float nextSwitchAnim = 0.0f;
    float animLength = 0.5f;
    bool animGoingRight = true;
    int counter = 0;


	// Use this for initialization
	void Start () {
		Audio = transform.GetComponentInChildren<AudioSource> ();
		Player = GameObject.FindGameObjectWithTag ("Player");
		rigidbody.velocity = new Vector3 (0.0f, 0.0f, speed * GlobalSettings.gameSpeed);
	}
	
	// Update is called once per frame
	void Update ()
	{
        accel *= Mathf.Abs(Player.transform.position.z - transform.position.z) / 1000;
		//Audio.volume = 1 - ((Vector3.Distance (Player.transform.position, transform.position)-20)/10);
		//Debug.Log (Vector3.Distance (Player.transform.position, transform.position));
        if (speed < (Player.GetComponent<SledScript>().maxWOBoost+30.0f))
        {
            speed += accel*(speed/Player.GetComponent<SledScript>().maxSpeed);
        }
        else if (speed >= Player.GetComponent<SledScript>().maxWOBoost + 50.0f)
        {
            speed = Player.GetComponent<SledScript>().maxWOBoost + 50.0f;
        }
        rigidbody.velocity = new Vector3(0.0f, 0.0f, speed);
        Animation();
	}

    void Animation()
    {
        
        if (Time.time > nextSwitchAnim)
        {
            nextSwitchAnim = Time.time + animLength;
            if (animGoingRight) // Right
            {
                transform.Translate(10f, 0, 0);
                if (counter >= 5)
                {
                    counter = 0;
                    animGoingRight = false;
                }
            }
            else //Top
            {
                transform.Translate(-10f, 0, 0);
                if (counter >= 5)
                {
                    counter = 0;
                    animGoingRight = true;
                }
            }

            counter++;
        }
    }

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			// Do animation or stuff


			Debug.Log("Bad Mothafucka");
            other.gameObject.SetActive(false);
            Application.LoadLevel("GameOver");

		} 
        else if (other.tag == "Obstacle")
		    other.gameObject.SetActive(false);
	}
}