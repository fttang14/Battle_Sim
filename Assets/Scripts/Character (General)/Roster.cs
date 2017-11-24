using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roster : MonoBehaviour {

    /***** DESCRIPTION *****/

    /*
     * This script contains the roster (list) of all characters.
     * From Unity Editor, input character Game Objects / Prefabs
     * to populate the characterList. From there, we will have
     * both the Player and CPU choose their characters from
     * within the characterList. Afterwards, set the Player
     * and Enemy objects to the characters they've chosen,
     * and instantiate those characters in game.
     */

    /*PUBLIC VARIABLES*/

    public GameController gameController;   // the game controller

    public GameObject PlayerObject; // reference to the player
    public GameObject EnemyObject;  // reference to the enemy

    public List<GameObject> characterList = new List<GameObject>(); //holds all the characters available

    /*PUBLIC STATIC VARIABLES*/

    public static bool rosterSet;   //determine whether all parties have decided on their characters
    
    public static CharacterStats Player;   //the Player
    public static CharacterStats Enemy;    //the Enemy

    /*PRIVATE VARIABLES*/

    bool playerSelect; // if player has chosen character
    bool enemySelect; // if enemy has chosen character

    List<CharacterStats> charDetail;      //holds all the details of player characters

    string charID;  //ID of the character selected

	// Use this for initialization
    // Initialize roster of characters
	void Awake () {
        charDetail = new List<CharacterStats>();

        //Adding characters to the character details list
        //Organized by HP, ATK, DEF, SPD, AP, CAP, METER, and ID

        CharacterStats MegaManX = new CharacterStats(1000, 20, 10, 
            2, 0, 4, 0, 0, "CHARACTER_MMX", transform.tag);

        charDetail.Add(MegaManX);

       /* 
        CharacterStats Zero = new CharacterStats(1200, 11, 4, 
            3, 0, 3, 0, "ZERO", transform.tag.ToUpper());
        
        PlayerDetail.Add(Zero);
        EnemyDetail.Add(Zero);
         */

        playerSelect = false;
        enemySelect = false;
        rosterSet = false;

        //don't start the game controller until characters have been selected
        gameController.enabled = false;
    }


    // Update is called once per frame
    private void Update()
    {
        // Let enemy chose a character
        if (!enemySelect && playerSelect)
        {
            int select = Random.Range(0, charDetail.Count);
            CharacterStats ET = charDetail[select];
            Enemy = new CharacterStats(ET.HP, ET.ATK, ET.DEF, ET.SPD, ET.AP, ET.CAP,
                ET.METER, ET.POSITION, ET.IDENTITY, EnemyObject.tag);
            //Debug.Log("Your Enemy has chosen: " + Enemy.IDENTITY + " with tag: " + Enemy.TAG);
            enemySelect = true;
        }

        // Let the user chose a character
        if (!playerSelect)
        {
            // TESTING AT THE MOMENT; WILL CHANGE LATER
            // character selection

            if (Input.GetKeyDown(KeyCode.M))
            {
                charID = "CHARACTER_MMX";
            }
            
            /*
            if (Input.GetKeyDown(KeyCode.Z))
            {
                charID = "ZERO";
            }
            */
            
            // find character in list that has same ID as charID
            foreach(CharacterStats p in charDetail)
            {
                if (p.IDENTITY.ToUpper().Equals(charID))
                {
                    Player = new CharacterStats(p.HP, p.ATK, p.DEF, p.SPD, p.AP, p.CAP, 
                        p.METER, p.POSITION, p.IDENTITY, PlayerObject.tag);

                    //Debug.Log("You have chosen: " + Player.IDENTITY + " with tag: " + Player.TAG);

                    //player has selected a character
                    //now, let the enemy select a character
                    playerSelect = true;

                    break;
                }
            }
        }

        //if both players have chosen their characters, ready the game
        //by turning on the game controller
        else if(playerSelect && enemySelect && !rosterSet)
        {

            // JUST TESTING
            //Debug.Log("Player Tag: " + Player.TAG);

            // JUST TESTING
            //Debug.Log("Enemy Tag: " + Enemy.TAG);

            rosterSet = true;
            gameController.enabled = true;
        }
    }
}
