using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    /*****DESCRIPTION*****/

    /* This script is for Player setup and input. */

    /*PUBLIC VARIABLES*/
    [HideInInspector]
    public bool getID;   //if the character is ready to change positions
    [HideInInspector]
    public CharacterStats PlayerInfo;    //CharacterStats of the Player
    [HideInInspector]
    public CharacterStats EnemyInfo;     //CharacterStats of the Enemy

    public Transform playerStartPos;    // starting position of Player
    public Transform enemyStartPos;     // starting position of Enemy

    /*STATIC VARIABLES*/
    public static bool keyPressed;  //determine if a key has been pressed down
    public static bool shieldActive;    //time for how much the shield is active for 
    public static bool playerHit;   //if the player has been hit or not
    public static bool enemyHit;    //if the enemy has been hit or not

    public static int playerDmg;  //store damage from attack to Player
    public static int enemyDmg;   //store damage from attack to Enemy

    /*PRIVATE VARIABLES*/

    /*PROTECTED VARIABLES*/
    protected AnimationClip[] clips;    //animation clips of the player

    protected Animator anim;        //animator of the Player

    protected BattleController battleController;   //battle controller

    protected bool transition;    //determining whether character is transitioning
    protected bool canAttack;     //determining whether character is attacking
    protected bool canDefend;     //determining whether character is defending
    protected bool projectileFired;       //if the projectile has been fired or not
    protected bool activateShield;        //(de)activates the shield
    protected bool HPCrit;        //determining if character is in HP Critical State
    protected bool win;       //if player has won
    protected bool lose;      //if player has lost
    protected bool ultReady;      //if ultimate attack is ready
    protected bool setID;   //if the character has changed positions

    protected Coroutine co;     //coroutine for the animations

    protected EnemyController ec;   // enemy controls

    protected enum Position
    {
        Entrance, Defend, Back,
        Retreat, Advance, Front, Attack, Hurt,
        Ultimate, Drop, Victory, Defeat, Rest
    };       // states of the Character

    protected float advTime;    //animation time to advance
    protected float retTime;    //animation time to retreat
    protected float entTime;    //animation time for entrance
    protected float vicTime;    //animation time for victory
    protected float ultTime;    //animation time for ultimate attack
    protected float dropTime;   //animation time for dropping after ultimate attack

    protected GameObject gameController;   //the game controller object
    protected GameController gc;    //the game controller component

    protected PlayerController pc;  // player controls

    protected Position currPos;     //current state of the character
    protected Position revertPos;   //revert back to original state when triggered from ANY state

    protected Rigidbody2D rb2d;     //rigidbody2D of the character

    protected Transform projectileTrans;    //projectile's position
    protected Transform shieldTrans;        //shield's position

    protected Vector2 startingPos;      //actual starting position of Player

    //TEST VARIABLES TO DELETE IN LATER UPDATES
    protected Vector2 targetAtkPos;
    protected Vector2 targetDefPos;

    // Use this for initialization
    protected virtual void Awake()
    {
        battleController = GameObject.Find("BattleController").GetComponent<BattleController>();

        keyPressed = false;
        shieldActive = false;
        playerHit = false;
        enemyHit = false;

        transition = true;
        canDefend = true;
        canAttack = false;
        projectileFired = false;
        activateShield = false;
        HPCrit = false;
        win = false;
        lose = false;
        ultReady = false;
        setID = false;
        getID = false;

        gameController = GameObject.Find("GameController");
        gc = gameController.GetComponent<GameController>();
        PlayerInfo = gc.gamePlayer;
        EnemyInfo = gc.gameEnemy;

        currPos = Position.Entrance;
        revertPos = Position.Rest;

    }

    protected virtual void Start () {

        //setting up the positions based on Tag
        if (CompareTag("Player"))
        {
            startingPos = playerStartPos.position;

            // TEST POSITIONS; WILL BE MORE ON POINT LATER
            targetAtkPos = Vector2.right;
            targetDefPos = Vector2.zero;

            // enabling the player controller functionality
            pc = GetComponent<PlayerController>();
            pc.enabled = true;

        }

        else if (CompareTag("Enemy"))
        {
            startingPos = enemyStartPos.position;

            // TEST POSITIONS; WILL BE MORE ON POINT LATER
            targetAtkPos = Vector2.left;
            targetDefPos = Vector2.zero;
            ec = GetComponent<EnemyController>();
            ec.enabled = true;
        }
	}

    /***** ALL IN COROUTINE *****/

    // Entrance animation function
    protected virtual void Entrance(float t, Vector2 v)
    {
        //Debug.Log("Start 'ENTRANCE' animation...");
    }

    // Advance animation function
    protected virtual void Advance(float t, Vector2 v)
    {
        //Debug.Log("Start 'ADVANCE' animation...");
    }

    // SPECIFIC TO RANGE TYPES;
    // Fire animation function
    protected virtual bool Fire(float t, bool fired)
    {
        //Debug.Log("Start 'ATTACK' animation...");
        return fired;
    }

    // Retreat animation function
    protected virtual void Retreat(float t, Vector2 v)
    {
        //Debug.Log("Start 'RETREAT' animation...");
    }

    // Shield animation function
    protected virtual void Shield(bool shieldUp)
    {
        //Debug.Log("Start 'DEFEND' animation...");
    }
    
    //Move forward if in back position
    protected void MoveForward()
    {
        currPos     = Position.Advance;
        canAttack   = true;
        canDefend   = false;
    }

    //attack if in front position
    protected void Attack()
    {
        currPos     = Position.Attack;
    }

    //Move back if in front position
    protected void MoveBack()
    {
        currPos     = Position.Retreat;
        canAttack   = false;
        canDefend   = true;
    }

    //defend if in back position
    protected void Defend()
    {
        currPos     = Position.Defend;
    }

    //get hurt from any position
    protected void Hurt()
    {
        if ((CompareTag("Player") && playerHit) || (CompareTag("Enemy") && enemyHit))
        {
            //change the character's position to HURT
            if (currPos != Position.Hurt)
            {
                //store current position to a temporary position
                revertPos = currPos;

                //change current position
                currPos = Position.Hurt;
            }
        }
    }

    //ultimate attack if ready, in any position
    protected virtual void Ultimate()
    {
        transition  = true;
        currPos     = Position.Ultimate;
    }

    //end game when you have won or lost
    protected virtual void BattleResult(bool victory, bool defeat)
    {

        transition = true;
        canAttack = false;
        canDefend = false;

        // if the Player has won
        if (victory)
        {
            currPos = Position.Victory;
            win = !victory;
        }

        // if the Player has lost
        else if (defeat)
        {
            currPos = Position.Defeat;
            lose = !defeat;
        }
    }

    //whenever collision occurs between two GameObjects
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log("Character damaged! Calculation start!");

        //send information to the Battle Controller
        battleController.BattleInProgress(gameObject, collider.gameObject);

        //set trigger for DAMAGED animator state
        Hurt();

        //set the transition for animation
        transition = true;
    }

}
