using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaManX : MonoBehaviour {

    /*PUBLIC Variables*/
    public Transform startingPositionPlayer;                        //starting position of character as Player
    public Transform startingPositionEnemy;                         //starting position of character as Enemy
    public GameObject projectile;                                   //projectile of the character
    public Transform projectileTransform;                           //position of where the projectile will spawn
    public GameObject shield;                                       //shield of the character
    public Transform shieldTransform;                               //position of where the projectile will spawn

    /*STATIC VARIABLES*/
    public static bool shieldActive;                                //time for how much the shield is active for  
    public static float storeDmg;                                   //store damage from attack

    /*PRIVATE VARIABLES*/
    Animator anim;                                                  //animator of the character

    bool transition;                                                //determining whether character is transitioning
    bool canAttack;                                                 //determining whether character is attacking
    bool canDefend;                                                 //determining whether character is defending
    bool projectileFired;                                           //if the projectile has been fired or not
    bool activateShield;                                            //(de)activates the shield
    bool HPCrit;                                                    //determining if character is in HP Critical State
    bool win;                                                       //if player has won
    bool lose;                                                      //if player has lost
    bool ultReady;                                                  //if ultimate attack is ready

    enum Position { Defend, Back, Retreat,
        Advance, Front, Attack, Hurt, Ultimate, Drop,
        Entrance, Victory, Defeat, Rest };                          //states of the character; REST state is just temp value

    float advTime;                                                  //animation time to advance
    float retTime;                                                  //animation time to retreat
    float entTime;                                                  //animation time for entrance
    float vicTime;                                                  //animation time for victory
    float ultTime;                                                  //animation time for ultimate attack
    float dropTime;                                                 //animation time for dropping after ultimate attack

    IEnumerator co;                                                 //coroutine for the animations

    Position currPos;                                               //current state of the character
    Position revertPos;                                             //revert back to original state when triggered from ANY state

    Rigidbody2D rb2d;

    Vector2 startingPosition;                                       //actual position of player

    //TEST VARIABLES TO DELETE IN LATER UPDATES
    Vector2 targetAtkPos;
    Vector2 targetDefPos;

    // Use this for initialization
    void Awake ()
    {

        //initializing variables
        anim                = GetComponent<Animator>();
        currPos             = Position.Entrance;
        revertPos           = Position.Rest;
        targetAtkPos        = Vector2.right;
        targetDefPos        = Vector2.zero;
        //rb2d                = GetComponent<Rigidbody2D>();
        transition          = true; //start at beginning for ENTRANCE animation
        canAttack           = false;
        canDefend           = true;
        projectileFired     = false;
        shieldActive        = false;
        activateShield      = false;
        HPCrit              = false;
        win                 = false;
        lose                = false;
        ultReady            = false;
        
        //NOTE: THERE SHOULD BE AN IF STATEMENT HERE
        //CHARACTER SELECT HAS NOT BEEN IMPLEMENTED YET
        //THEREFORE, CHARACTER IS DEFAULT AS PLAYER FOR NOW
        startingPosition  = new Vector2(startingPositionPlayer.position.x, startingPositionPlayer.position.y);

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

            //animation time for entrance
            else if (clip.name.Contains("Entrance"))
            {
                entTime = clip.length;
            }

            //animation time for victory
            else if (clip.name.Contains("Victory"))
            {
                vicTime = clip.length;
            }

            //animation for ultimate attack
            else if (clip.name.Contains("Ultimate"))
            {
                ultTime = clip.length;
            }

            //animation for drop time
            else if (clip.name.Contains("Drop"))
            {
                dropTime = clip.length;
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

        //if pressing for ULTIMATE attack
        if(Input.GetKeyDown(KeyCode.Q) && !transition /*&& ultReady*/)
        {
            //start animation, change state, and begin transition
            transition = true;
            currPos = Position.Ultimate;
            anim.SetTrigger("Ult");
        }

        //if the player has won the match
        //if (win)
        if(Input.GetKeyDown(KeyCode.K) && !transition)
        {
            transition = true;
            currPos = Position.Victory;
            anim.SetTrigger("Win");
            canAttack = false;
            canDefend = false;
            win = false;
        }

        //if player has lost the match
        //if (lose)
        if(Input.GetKeyDown(KeyCode.L) && !transition)
        {
            transition = true;
            currPos = Position.Defeat;
            anim.SetTrigger("Lose");
            canAttack = false;
            canDefend = false;
            lose = false;
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

        //play winning animation if victory has been achieved
        if (currPos.Equals(Position.Victory))
        {
            //set exit position
            Vector2 exitPosition = new Vector2(transform.position.x, transform.position.y + 2.0f);
            while (anim.GetCurrentAnimatorStateInfo(0).IsName("Victory") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
            {
                Debug.Log(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
                //rising up sequence (exclusive to X)
                if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.62f)
                {
                    transform.position = Vector2.Lerp(transform.position, exitPosition, anim.GetCurrentAnimatorStateInfo(0).normalizedTime / vicTime);
                }

                yield return null;
            }

        }

        //play losing animation if defeat has been achieved
        else if (currPos.Equals(Position.Defeat))
        {
            Debug.Log("I lost!");
            //let animation play, and then destroy character
            Destroy(this.gameObject, 5.0f);
        }

        //intro animation start
        else if (currPos.Equals(Position.Entrance))
        {

            //set the entrance position
            Vector2 entrancePosition = new Vector2(startingPosition.x, startingPosition.y + 2.0f);
            while(anim.GetCurrentAnimatorStateInfo(0).IsName("Entrance") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f){

                //transition from entrance position to starting position
                if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.44f)
                {
                    transform.position = Vector2.Lerp(entrancePosition, startingPosition, anim.GetCurrentAnimatorStateInfo(0).normalizedTime * entTime);
                    //Debug.Log(anim.GetCurrentAnimatorStateInfo(0).normalizedTime * entTime);
                }
                
                yield return null;
            }

            //let's start functionality
            transition = false;
            currPos = Position.Back;
        }

        //if the player gets HURT
        else if (currPos.Equals(Position.Hurt))
        {

            //wait until animation ends before continuing on
            if(storeDmg >= 100.0f)
            {
                while (anim.GetCurrentAnimatorStateInfo(0).IsName("HurtMajor") && !anim.GetCurrentAnimatorStateInfo(0).loop)
                {
                    Debug.Log(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    //wait until animation is done
                    yield return null;
                }
            }

            else
            {
                while (anim.GetCurrentAnimatorStateInfo(0).IsName("HurtMin") && !anim.GetCurrentAnimatorStateInfo(0).loop)
                {
                    //wait until animation is done
                    yield return null;
                }
            }

            //reset values so that game can continue
            transition = false;

            //set position back to previous position
            currPos = revertPos;
            switch ((int)currPos)
            {
                case (int)Position.Back:
                    anim.SetInteger("RevertPos", (int)currPos);
                    break;
                case (int)Position.Front:
                    anim.SetInteger("RevertPos", (int)currPos);
                    break;
                default:
                    break;
            }

            //reset damage count
            //anim.SetFloat("DmgCount", 0.0f);
            revertPos = Position.Rest;

            //IF STATEMENT REGARDING IF CHARACTER HAS REACHED HP CRITICAL STATE
            HPCrit = true;
            anim.SetBool("CriticalHP", HPCrit);
        }

        //if ADVANCE transition is happening
        else if (currPos.Equals(Position.Advance))
        {
            //wait for animation to completely end before moving to next transition
            while(anim.GetCurrentAnimatorStateInfo(0).IsName("MoveForward") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
            {
                //move character into target position
                transform.position = Vector2.Lerp(transform.position, targetAtkPos + (startingPosition / 2), anim.GetCurrentAnimatorStateInfo(0).normalizedTime / advTime);
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
                    Instantiate(projectile, projectileTransform.position + transform.position, projectileTransform.rotation);

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

            //FOR TESTING PURPOSES ONLY; WILL CHANGE LATER
            //win = true;
            //ultReady = true;
        }

        //if DEFEND is happening
        else if (currPos.Equals(Position.Defend))
        {

            //if the shield has not been activated yet, activate it
            if (!activateShield)
            {
                Instantiate(shield, shieldTransform.position + transform.position, shieldTransform.rotation);
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

            //TESTING PURPOSES ONLY; WILL CHANGE LATER
            //lose = true;
            //ultReady = true;
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
                    targetDefPos = new Vector2(0, 0.35f);
                }
                //jump back with vertical descent
                else
                {
                    targetDefPos = Vector2.zero;
                }
                transform.position = Vector2.Lerp(transform.position, targetDefPos + startingPosition, anim.GetCurrentAnimatorStateInfo(0).normalizedTime / retTime);

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

        //if ultimate is happening
        else if (currPos.Equals(Position.Ultimate))
        {
            //NOTE: THIS IS EXCLUSIVE TO EACH CHARACTER
            //EVERY ULTIMATE ATTACK WILL BE DIFFERENT
            Vector2 offset1 = new Vector2(transform.position.x + 0.1f, 0.25f);
            Vector2 offset2 = new Vector2(targetAtkPos.x + (startingPosition.x / 2) + 0.25f, 0.25f);
            while (anim.GetCurrentAnimatorStateInfo(0).IsName("Ultimate") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
            {
                //offset 1 transform
                if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.3f)
                {
                    transform.position = Vector2.Lerp(transform.position, offset1, anim.GetCurrentAnimatorStateInfo(0).normalizedTime * ultTime);
                }

                //offset 2 transform
                else
                {
                    transform.position = Vector2.Lerp(transform.position, offset2, anim.GetCurrentAnimatorStateInfo(0).normalizedTime * ultTime);
                }

                //wait while ultimate completes
                yield return null;
            }
            
            //prepare back to starting position
            currPos = Position.Drop;
            
        }

        //dropping from ULTIMATE animation
        if (currPos.Equals(Position.Drop))
        {
            //make sure character lands back on floor
            Vector2 offset3 = new Vector2(transform.position.x, 0.0f);
            while(anim.GetCurrentAnimatorStateInfo(0).IsName("Drop") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
            {
                transform.position = Vector2.Lerp(transform.position, offset3, anim.GetCurrentAnimatorStateInfo(0).normalizedTime / dropTime);
                yield return null;
            }

            //revert back to starting position
            currPos = Position.Retreat;
        }

        yield return null;
    }

    //if character gets damaged
    private void OnTriggerEnter2D(Collider2D collision)
    {

        //set trigger for DAMAGED animator state
        if (collision.gameObject.CompareTag("Enemy") && !Projectile.playerShot)
        {

            //revert position should be previous state before HURT state
            if (currPos != Position.Hurt)
            {

                revertPos = currPos;

                //change current position and set transition
                currPos = Position.Hurt;
                transition = true;
            }

            anim.SetTrigger("Damaged");

            //determine if damage was small or large
            //implemented later on
            anim.SetFloat("DmgCount", storeDmg);

            //TESTING: DESTROYING OTHER GAME OBJECT
            Destroy(collision.gameObject);

        }
        
    }
}
