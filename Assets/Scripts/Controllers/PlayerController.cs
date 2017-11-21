using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    /*PUBLIC VARIABLES*/
    [HideInInspector]
    public bool transitionSet; // determining whether to change positions yet or not
    [HideInInspector]
    public int charPosition;    // current Position from Character script

    /*PRIVATE VARIABLES*/
    Character playerCharacter;  // the player's character

    // JUST A REFERENCE; WILL REMOVE LATER ON
    enum SetPosition
    {
        Entrance, Defend, Back,
        Retreat, Advance, Front, Attack, Hurt,
        Ultimate, Drop, Victory, Defeat, Rest
    };       // states of the Character

    private void OnEnable()
    {
        playerCharacter = GetComponent<Character>();
        transitionSet = true;
    }

    // Update is called once per frame
    void Update()
    {
        // the RIGHT direction
        if (Input.GetKeyDown(KeyCode.D))
        {
            //if the player has not transitioned yet
            if (!transitionSet)
            {

                // Moving forward or attacking
                // Back -> Advance, Front -> Attack
                // Back = 2,    Advance = 4
                // Front = 5,   Attack = 6
                switch (charPosition)
                {
                    case 2:
                        playerCharacter.PlayerInfo.POSITION = 4;
                        //Debug.Log("I am advancing!");

                        break;
                    case 5:
                        playerCharacter.PlayerInfo.POSITION = 6;
                        //Debug.Log("I am attacking!");

                        break;
                    default:
                        break;
                }

                //once the position is set, let the animations run in Character script
                playerCharacter.getID = true;

                //prevent player from continually pressing
                //down the same key...
                transitionSet = true;
            }
        }

        // the LEFT direction
        else if (Input.GetKeyDown(KeyCode.A))
        {
            //if the player has not transitioned yet
            if (!transitionSet)
            {
                // Moving back or retreating
                // Front -> Retreat, Back -> Defend
                // Defend = 1,  Back = 2,
                // Retreat = 3, Front = 5
                switch (charPosition)
                {
                    case 2:
                        playerCharacter.PlayerInfo.POSITION = 1;
                        //Debug.Log("I am defending!");
                        break;
                    case 5:
                        playerCharacter.PlayerInfo.POSITION = 3;
                        //Debug.Log("I am retreating!");
                        break;
                    default:
                        break;
                }

                //once the position is set, let the animations run in Character script
                playerCharacter.getID = true;

                //prevent player from continually pressing
                //down the same key...
                transitionSet = true;

            }
        }
    }
}
