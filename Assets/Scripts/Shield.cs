using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

    /*PUBLIC VARIABLES*/
    public float startTime = 3.0f;  //maximum time for the shield to be active


    /*PRIVATE VARIABLES*/
    bool deactivate;  //determines when to deactivate the shield

    float timeLeft;   //maximum time for the shield to be active
    float clipTime; //time for how long the clip will be played
    float timeExit; //time for the shield to be completely destroyed
    Animator anim;  //animator of the shield
    Rigidbody2D rb2d;   //rigidbody2d of the shield


    //initializing variables
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();

        //records length (time) of each animation
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            //animation time of shield
            if (clip.name.Contains("Inactive"))
            {
                clipTime = clip.length;
            }
        }

    }

    //start the animation
    private void Start()
    {
        anim.SetBool("DefenseActive", true);

        //the time for the shield to be up before destroying the shield
        timeLeft = startTime;

        //time after the shield's timer reaches 0; how long it'll take to completely destroy shield
        timeExit = clipTime;

        //setup deactivate boolean
        deactivate = false;
    }

    //start the countdown
    private void Update()
    {
        //if ready to desstroy shield
        if (deactivate)
        {
            //decrement timer
            timeExit -= Time.deltaTime;

            //wait until time is up before destroying shield
            if (timeExit <= 0.0f)
            {
                MegaManX.shieldActive = false;
                Destroy(gameObject);
            }

        }

        //if shield is still up
        else
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
                deactivate = true;
            }
        }
    }

    //if a weapon hits the shield, reset the shield's timer
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon"))
        {
            timeLeft = startTime;

            //destroy all projectiles that hit shield
            if (collision.gameObject.name.Contains("Projectile"))
            {
                Destroy(collision.gameObject);
            }
        }


    }

    
}
