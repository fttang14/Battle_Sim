using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

    /*PUBLIC VARIABLES*/
    public float startTime = 3.0f;  //maximum time for the shield to be active


    /*PRIVATE VARIABLES*/

    float timeLeft;   //maximum time for the shield to be active
    float clipTime; //time for how long the clip will be played
    Animator anim;  //animator of the shield
    Rigidbody2D rb2d;   //rigidbody2d of the shield


    //initializing variables
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();

    }

    //start the animation
    private void Start()
    {
        anim.SetBool("DefenseActive", true);

        //the time for the shield to be up before destroying the shield
        timeLeft = startTime;

    }

    //start the countdown
    private void Update()
    {
        //decrease time that shield stays active
        if (timeLeft > 0.0f)
        {
            timeLeft -= Time.deltaTime;
            Debug.Log("Time left: " + timeLeft + "seconds.");
        }

        //deactivate the shield
        else if (timeLeft <= 0.0f)
        {
            anim.SetBool("DefenseActive", false);
            MegaManX.shieldActive = false;
            Destroy(gameObject);
        }
    }

    //if a weapon hits the shield, reset the shield's timer
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon"))
        {
            timeLeft = startTime;
        }
    }

    
}
