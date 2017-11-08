using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaManX : MonoBehaviour {

    /*PUBLIC Variables*/
    public GameObject projectile;                                   //projectile of the character
    public Transform projectileTransform;                           //position of where the projectile will spawn
    public GameObject shield;                                       //shield of the character
    public Transform shieldTransform;                               //position of where the projectile will spawn

    /*STATIC VARIABLES*/
    public static bool shieldActive;                                //time for how much the shield is active for

    /*PRIVATE VARIABLES*/
    Animator anim;                                                  //animator of the character

    bool transition;                                                //determining whether character is transitioning
    bool canAttack;                                                 //determining whether character is attacking
    bool canDefend;                                                 //determining whether character is defending
    bool projectileFired;                                           //if the projectile has been fired or not
    bool isDefending;                                               //if the character is defending or not
    bool activateShield;                                            //(de)activates the shield

    enum Position { Defend, Back, Retreat, Advance, Front, Attack}; //states of the character

    float advTime;                                                  //animation time to advance
    float retTime;                                                  //animation time to retreat

    IEnumerator co;                                                 //coroutine for the animations

    Position currPos;                                               //current state of the character

    Rigidbody2D rb2d;

    //TEST VARIABLES TO DELETE IN LATER UPDATES
    Vector2 targetAtkPos;
    Vector2 targetDefPos;

    // Use this for initialization
    void Awake ()
    {

        //initializing variables
        anim                = GetComponent<Animator>();
        currPos             = Position.Back;
        targetAtkPos        = Vector2.right;
        targetDefPos        = Vector2.zero;
        rb2d                = GetComponent<Rigidbody2D>();
        transition          = false;
        canAttack           = false;
        canDefend           = true;
        projectileFired     = false;
        isDefending         = false;
        shieldActive        = false;
        activateShield      = false;

        //records length (time) of each animation
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in clips)
        {
            //animation time of retreat position
            if (clip.name.Contains("Retreat"))
            {
                retTime = clip.length;
            }
            //animation time of advancing position
            else if (clip.name.Contains("Advance"))
            {
                advTime = clip.length;
            }
        }
    }

    //setting up the magic
    private void Update()
    {
        //if pressing the RIGHT direction
        if (Input.GetKeyDown(KeyCode.D))
        {
            //if current state is in BACK position and has not transitioned yet
            //Move forward
            if (currPos.Equals(Position.Back) && !transition)
            {
                //start animation, change state, and begin transition
                transition = true;
                currPos = Position.Advance;
                anim.SetTrigger("Front");
                canAttack = true;
                canDefend = false;
            }

            //if current state is in FRONT position and is able to attack
            //Attack
            else if (currPos.Equals(Position.Front) && !transition && canAttack)
            {
                transition = true;
                currPos = Position.Attack;
                anim.SetTrigger("Strike");
            }
        }

        //if pressing the LEFT direction
        if (Input.GetKeyDown(KeyCode.A))
        {
            //if current state is in FRONT position and has not transitioned yet
            //Move back
            if (currPos.Equals(Position.Front) && !transition)
            {
                //start animation, change state, and begin transition
                transition = true;
                currPos = Position.Retreat;
                anim.SetTrigger("Back");
                canAttack = false;
                canDefend = true;

            }

            //if current state is in BACK position and has not transitioned yet
            //Defend
            else if (currPos.Equals(Position.Back) && !transition && canDefend)
            {
                transition = true;
                currPos = Position.Defend;
            }
        }
    }

    //this is where the magic happens, kinda
    private void FixedUpdate()
    {
        if (transition)
        {
            //start the transition coroutine
            Coroutine co = StartCoroutine("AnimationTransition");

            //end the transition coroutine
            StopCoroutine(co);

        }
    }

    //deals with animation transitions
    IEnumerator AnimationTransition()
    {

        //if ADVANCE transition is happening
        if (currPos.Equals(Position.Advance))
        {
            //wait for animation to completely end before moving to next transition
            while(anim.GetCurrentAnimatorStateInfo(0).IsName("MoveForward") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
            {
                //move character into target position
                transform.position = Vector2.Lerp(transform.position, targetAtkPos, anim.GetCurrentAnimatorStateInfo(0).normalizedTime / advTime);
                //wait every frame until animation finishes
                yield return null;
                
            }

            //set parent position to child position
            transform.parent.position = transform.position;
            transform.localPosition = Vector2.zero;

            //once finished, set new state
            currPos = Position.Front;

            //end transition
            transition = false;
        }

        //if ATTACK is happening
        else if (currPos.Equals(Position.Attack))
        {
            //wait for attack animation to finish before doing anything else
            while (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
            {
                //if position where projectile is fired and a projectile has not been fired yet
                if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f && !projectileFired)
                {
                    //instantiate the shot
                    Instantiate(projectile, projectileTransform.position, projectileTransform.rotation);

                    //prevent repeated shots
                    projectileFired = true;
                }

                //wait for every frame until animation ends
                yield return null;
            }

            //once finished, return to FRONT position
            currPos = Position.Front;
            transition = false;
            projectileFired = false;

        }

        //if DEFEND is happening
        else if (currPos.Equals(Position.Defend))
        {

            //if the shield has not been activated yet, activate it
            if (!activateShield)
            {
                Instantiate(shield, shieldTransform.position, shieldTransform.rotation);
                shieldActive = true;
                activateShield = true;
            }

            //wait until the shield is no longer active to continue
            while (shieldActive)
            {

                yield return null;
            }

            currPos = Position.Back;
            transition = false;
            activateShield = false;
        }

        //if RETREAT transition is happening
        else if (currPos.Equals(Position.Retreat))
        {

            //wait for animation to completely end before moving to next transition
            while (anim.GetCurrentAnimatorStateInfo(0).IsName("Retreat") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
            {
                //move character into target position
                //jump back with vertical ascent
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.25f)
                {
                    targetDefPos = new Vector2(0, 0.25f);
                }
                //jump back with vertical descent
                else
                {
                    targetDefPos = Vector2.zero;
                }
                transform.position = Vector2.Lerp(transform.position, targetDefPos, anim.GetCurrentAnimatorStateInfo(0).normalizedTime / retTime);

                //wait every frame until animation finishes
                yield return null;

            }

            //set parent position to child position
            transform.parent.position = transform.position;
            transform.localPosition = Vector2.zero;

            //once finished, set new state
            currPos = Position.Back;

            //end transition
            transition = false;
        }

        yield return null;
    }
}
