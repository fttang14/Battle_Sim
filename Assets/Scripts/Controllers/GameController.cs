using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    /*PUBLIC VARIABLES*/
    public Roster roster;   //the roster

    [HideInInspector]
    public CharacterStats gamePlayer;   //information about Player character
    [HideInInspector]
    public CharacterStats gameEnemy;    //information about Enemy character
    /*STATIC VARIABLES*/

    /*PRIVATE VARIABLES*/
    bool characterSet;  //determine when both characters have been instantiated

    bool playerSet; //when the player has been instantiated
    bool enemySet;  //when the enemy has been instantiated

    GameObject characterP;   // instantiating the Player character
    GameObject characterE;  // instantiating the Enemy character

    // Use this for initialization
    void Awake()
    {
        characterSet = false;
        playerSet = false;
        enemySet = false;

    }

    // Update is called once per frame
    void Update()
    {

        //once the characters have been chosen, instantiate those characters
        if (Roster.rosterSet && !characterSet)
        {

            gamePlayer = Roster.Player;
            gameEnemy = Roster.Enemy;

            //Debug.Log(gamePlayer.TAG);
            //Debug.Log(gameEnemy.TAG);

            // first, search for the characters in GameObject List
            foreach (GameObject g in roster.characterList)
            {
                //then, instantiate character when game object has been found
                if (gamePlayer.IDENTITY.ToUpper().Equals(g.name.ToUpper()))
                {
                    characterP = SetupCharacters(g, gamePlayer.TAG);
                    playerSet = true;
                    break;
                }

                else
                {
                    break;
                }
            }

            // now, search for the characters in GameObject List for Enemy
            foreach (GameObject g in roster.characterList)
            {
                // instantiate Enemy GameObject
                if (gameEnemy.IDENTITY.ToUpper().Equals(g.name.ToUpper()))
                {
                    characterE = SetupCharacters(g, gameEnemy.TAG);
                    enemySet = true;
                    break;
                }

                else
                {
                    break;
                }
            }

            if (playerSet && enemySet)
            {
                // prevent script from constantly instantiating game objects
                characterSet = true;
                roster.enabled = false;

                // BATTLE CONTROLLER IS NEEDED
            }
        }
    }

    //Instantiating characters into the correct starting positions
    GameObject SetupCharacters(GameObject g, string type)
    {
        GameObject playerObject;    // setting up the instantiation of the Player Object
        GameObject enemyObject;     // setting up the instantiation of the Enemy Object

        // if the character selected is MegaMan X set him to active
        if (g.name.ToUpper().Equals("CHARACTER_MMX"))
        {
            if (type.ToUpper().Contains("PLAYER"))
            {
                playerObject = Instantiate(g);
                playerObject.tag = "Player";
                playerObject.GetComponent<PlayerController>().enabled = false;
                playerObject.GetComponent<EnemyController>().enabled = false;
                playerObject.transform.parent = GameObject.Find("PlayerObject").transform;
                return playerObject;
            }

            else if (type.ToUpper().Contains("ENEMY"))
            {
                enemyObject = Instantiate(g);
                enemyObject.tag = "Enemy";
                enemyObject.GetComponent<PlayerController>().enabled = false;
                enemyObject.GetComponent<EnemyController>().enabled = false;

                Vector2 flipScale = enemyObject.transform.localScale;
                flipScale.x *= -1;
                enemyObject.transform.localScale = flipScale;

                enemyObject.transform.parent = GameObject.Find("EnemyObject").transform;
                return enemyObject;
            }
        }

        return null;
    }
}
