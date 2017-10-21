using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaManX : MonoBehaviour {

    /*PRIVATE VARIABLES*/
    Animator anim;                                                  //animator of the character

    bool transition;                                                //determining whether character is transitioning

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
            if (currPos.Equals(Position.Back) && !transition)
            {
                //start animation, change state, and begin transition
                transition = true;
                currPos = Position.Advance;
                anim.SetTrigger("Front");

            }

            //if current state is in FRONT position and has not transitioned yet
            else if (currPos.Equals(Position.Front) && !transition)
            {
                Debug.Log("Attack!!!");
            }
        }

        //if pressing the LEFT direction
        if (Input.GetKeyDown(KeyCode.A))
        {
            //if current state is in FRONT position and has not transitioned yet
            if (currPos.Equals(Position.Front) && !transition)
            {
                //start animation, change state, and begin transition
                transition = true;
                currPos = Position.Retreat;
                anim.SetTrigger("Back");
                
            }

            //if current state is in BACK position and has not transitioned yet
            else if (currPos.Equals(Position.Back) && !transition)
            {
                Debug.Log("Defend!!!");
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

        /*
        else if (Pos.Equals(Position.Attack))
        {

        }
        else if (Pos.Equals(Position.Defend))
        {

        }
        */

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

                /*
                //jump back with vertical ascent
                if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.75f)
                {
                    transform.position = Vector2.Lerp(transform.position, targetDefPosUp, anim.GetCurrentAnimatorStateInfo(0).normalizedTime / retTime);
                }

                //jump back with vertical descent
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.75f)
                {
                    transform.position = Vector2.Lerp(transform.position, targetDefPosDown, anim.GetCurrentAnimatorStateInfo(0).normalizedTime / retTime);
                }

                Debug.Log("Y position: " + transform.position.y);
                */
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
