using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaManX : Character{

    /*PUBLIC VARIABLES*/
    public GameObject projectile;   //projectile prefab of the character, if character can use projectiles
    public GameObject shield;       //shield prefab of the character

    /*PRIVATE VARIABLES*/
    Projectile projectileInfo;  //script component of the projectile

    // Use this for initialization
    protected override void Start ()
    {

        //initializing variables, starting from parent
        base.Start();

        // animator
        anim    = GetComponent<Animator>();

        // animation clip
        clips = anim.runtimeAnimatorController.animationClips;

        // float; getting animation time
        foreach (AnimationClip clip in clips)
        {
            // animation time of retreat position
            if (clip.name.Contains("X_Retreat"))
            {
                retTime = clip.length;
            }
            // animation time of advancing position
            else if (clip.name.Contains("X_Advance"))
            {
                advTime = clip.length;
            }

            // animation time for entrance
            else if (clip.name.Contains("X_Entrance"))
            {
                entTime = clip.length;
            }

            // animation time for victory
            else if (clip.name.Contains("X_Victory"))
            {
                vicTime = clip.length;
            }

            // animation for ultimate attack
            else if (clip.name.Contains("X_Ultimate"))
            {
                ultTime = clip.length;
            }

            // animation for drop time
            else if (clip.name.Contains("X_Drop"))
            {
                dropTime = clip.length;
            }
        }

        // setting up the rigid body 2D
        rb2d = GetComponent<Rigidbody2D>();

        // setting up the transforms for the projectile and the shield
        if (CompareTag("Player"))
        {
            projectileTrans = GameObject.Find("PlayerProjectileSpawn").transform;
            shieldTrans = GameObject.Find("PlayerShieldSpawn").transform;
        }

        else if (CompareTag("Enemy"))
        {
            projectileTrans = GameObject.Find("EnemyProjectileSpawn").transform;
            shieldTrans = GameObject.Find("EnemyShieldSpawn").transform;
        }

        // start the entrance transition
        co = StartCoroutine("AnimationTransition");
        StopCoroutine(co);

    }

    //setting up the magic
    private void Update()
    {
        // sending information to the Controllers
        if (setID)
        {
            if (CompareTag("Player"))
            {
                pc.charPosition = PlayerInfo.POSITION;
                //Debug.Log("Player position: " + pc.charPosition);
                pc.transitionSet = false;
            }
            else if (CompareTag("Enemy"))
            {
                //Debug.Log("Work in progress for enemy...");
            }

            setID = false;
        }

        // receiving information from the Controllers
        if (getID)
        {
            // if the current STATE is in BACK position
            if (currPos.Equals(Position.Back))
            {
                // if the character is moving to the FRONT position
                if (PlayerInfo.POSITION.Equals((int)Position.Advance))
                {
                    MoveForward();
                    anim.SetTrigger("Front");

                }

                // if the character is transitioning to DEFEND position
                else if (PlayerInfo.POSITION.Equals((int)Position.Defend))
                {
                    Defend();
                }
            }

            // if the current STATE is in FRONT position
            else if (currPos.Equals(Position.Front))
            {
                // if the character is moving to the BACK position
                if (PlayerInfo.POSITION.Equals((int)Position.Retreat))
                {
                    MoveBack();
                    anim.SetTrigger("Back");
                }

                // if the character is transitioning to ATTACK position
                else if (PlayerInfo.POSITION.Equals((int)Position.Attack))
                {
                    Attack();
                    anim.SetTrigger("Strike");
                }
            }

            getID = false;
            transition = true;
        }
    }


    //this is where the magic happens, kinda
    private void FixedUpdate()
    {
        if (transition)
        {

            //start the transition coroutine
            co = StartCoroutine("AnimationTransition");

            //end the transition coroutine
            StopCoroutine(co);

        }
    }

    //deals with animation transitions
    IEnumerator AnimationTransition()
    {

        //intro animation start
        if (currPos.Equals(Position.Entrance))
        {

            //set the entrance position
            Vector2 entrancePosition = new Vector2(startingPos.x, startingPos.y + 2.0f);
            while (anim.GetCurrentAnimatorStateInfo(0).IsName("Entrance") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
            {

                //transition from entrance position to starting position
                Entrance(anim.GetCurrentAnimatorStateInfo(0).normalizedTime, entrancePosition);
                yield return null;
            }

            //let's start functionality
            currPos = Position.Back;
        }

        //if ADVANCE transition is happening
        else if (currPos.Equals(Position.Advance))
        {
            //wait for animation to completely end before moving to next transition
            while (anim.GetCurrentAnimatorStateInfo(0).IsName("MoveForward") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
            {
                //move character into target position
                Advance(anim.GetCurrentAnimatorStateInfo(0).normalizedTime, transform.position);

                //wait every frame until animation finishes
                yield return null;

            }

            //once finished, set new state
            currPos = Position.Front;

        }

        //if ATTACK transition is happening
        else if (currPos.Equals(Position.Attack))
        {
            //wait for animation to finish before doing anything else
            while (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
            {
                //SPECIFIC TO MEGAMAN; if position where projectile is fired
                //and projectile has not been fired yet
                projectileFired = Fire(anim.GetCurrentAnimatorStateInfo(0).normalizedTime, projectileFired);

                yield return null;
            }

            //once finished, return to FRONT position and reset projectile fire
            currPos = Position.Front;
            projectileFired = false;
        }

        //if RETREAT transition is happening
        else if (currPos.Equals(Position.Retreat))
        {
            //wait for animation to completely end before moving to next transition
            while (anim.GetCurrentAnimatorStateInfo(0).IsName("Retreat") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
            {

                //move character into target position
                Retreat(anim.GetCurrentAnimatorStateInfo(0).normalizedTime, transform.position);

                //wait every frame until animation finishes
                yield return null;
            }

            //once finished, set new state
            currPos = Position.Back;

        }

        //if DEFEND transition is happening
        else if (currPos.Equals(Position.Defend))
        {
            //if the shild has not been active yet, activate it
            Shield(activateShield);

            //do nothing while shield is active
            while (shieldActive)
            {
                yield return null;
            }

            //reset positionings and reactivate shield permission
            currPos = Position.Back;
            activateShield = false;
        }

        //if the character gets HURT
        else if (currPos.Equals(Position.Hurt))
        {
            //wait until animation ends before continuing on
            if ((CompareTag("Player") && playerDmg >= 100) || (CompareTag("Enemy") && enemyDmg >= 100))
            {
                //play hurt animation only once through
                while (anim.GetCurrentAnimatorStateInfo(0).IsName("HurtMajor") && !anim.GetCurrentAnimatorStateInfo(0).loop)
                {
                    //wait until animation is done
                    yield return null;
                }
            }

            else if ((CompareTag("Player") && playerDmg < 100) || (CompareTag("Enemy") && enemyDmg < 100)){
                
                //play hurt animation only once through
                while(anim.GetCurrentAnimatorStateInfo(0).IsName("HurtMin") && !anim.GetCurrentAnimatorStateInfo(0).loop)
                {
                    //wait until animation is done
                    yield return null;
                }
            }

            //revert back to previous position
            currPos = revertPos;
            anim.SetInteger("RevertPos", (int)currPos);
            revertPos = Position.Rest;
        }

        // setting the state to the characters
        if (CompareTag("Player"))
        {
            PlayerInfo.POSITION = (int)currPos;
        }
        else if (CompareTag("Enemy"))
        {
            EnemyInfo.POSITION = (int)currPos;
        }

        transform.parent.position = transform.position;
        transform.localPosition = Vector2.zero;

        //end transition
        transition = false;
        setID = true;

        //TESTING: Return character's health
        Debug.Log("Player Health: " + PlayerInfo.HP);
        Debug.Log("Enemy Health: " + EnemyInfo.HP);

        yield return null;
    }

    //function that transitions from entrance position to starting position
    protected override void Entrance(float t, Vector2 v)
    {
        base.Entrance(t,v);
        if (t <= 0.44f)
        {
            transform.position = Vector2.Lerp(v, startingPos, t * entTime);

        }
    }

    //function that transitions from starting position to front position
    protected override void Advance(float t, Vector2 v)
    {
        base.Advance(t, v);
        transform.position = Vector2.Lerp(v, targetAtkPos + (startingPos / 2), t / advTime);
    }

    //SPECIFIC TO RANGE TYPES;
    //function that instantiates a projectile from front to attack position
    protected override bool Fire(float t, bool fired)
    {
        
        //instantiate the shot if has not fired yet
        if(t >= 0.5f && !fired)
        {
            //instantiate the projectile and grab its script
            projectileInfo = Instantiate(projectile, projectileTrans.position + transform.position, 
                projectileTrans.rotation).GetComponent<Projectile>();

            //setting the secondary tags of the projectile
            if (CompareTag("Player"))
            {
                projectileInfo.tempTag = "Player";
            }
            else if (CompareTag("Enemy"))
            {
                projectileInfo.tempTag = "Enemy";
            }

            //prevent repeated shots
            fired = true;
            
        }

        return fired;
    }

    //function that transitions from front position to starting position
    protected override void Retreat(float t, Vector2 v)
    {
        base.Retreat(t, v);
        //move character into target position
        //jump back with vertical ascent
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.25f)
        {
            targetDefPos = new Vector2(0, 0.35f);
        }
        //jump back with veritcal descent
        else
        {
            targetDefPos = Vector2.zero;
        }
        transform.position = Vector2.Lerp(v, startingPos + targetDefPos, t / retTime);
        
    }

    //function that instantiates a shield from starting to defend position
    protected override void Shield(bool shieldUp)
    {
        base.Shield(shieldUp);

        //if shield has not been activated yet, activate it
        if (!shieldUp)
        {
            Instantiate(shield, shieldTrans.position + transform.position, shieldTrans.rotation);
            shieldActive = true;
            activateShield = true;
        }
    }

    //determine whether the character was hit by another object
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        base.OnTriggerEnter2D(collider);

        //play the HURT animation
        if((CompareTag("Player") && playerHit) || (CompareTag("Enemy") && enemyHit))
        {
            anim.SetTrigger("Damaged");

            if (playerHit)
            {
                anim.SetFloat("DmgCount", playerDmg);
                playerHit = false;
            }

            else if (enemyHit)
            {
                anim.SetFloat("DmgCount", enemyDmg);
                enemyHit = false;
                
            }
        }
    }
}
