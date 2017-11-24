using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour {

    /*PUBLIC VARIABLES*/
    public GameController game; //the game controller

    /*PRIVATE VARIABLES*/
    CharacterStats PlayerStats; //set up the instantiated Player's stats
    CharacterStats EnemyStats;  //set up the instantiated Enemy's stats

    GameObject Player;  //set up the instantiated Player GameObject
    GameObject Enemy;   //set up the instantiated Enemy GameObject


	// Once battle controller has awaken, prepare variables
	void OnEnable () {

        //set up stats from the Game Controller
        PlayerStats = game.gamePlayer;
        EnemyStats = game.gameEnemy;

        //turn off the Game Controller
        game.enabled = false;

	}

    //setting up the combatants; will be called from GameController
    public void CharacterSetup(GameObject p, GameObject e)
    {
        Player = p;
        Enemy = e;
    }

    //determining the interaction between two objects
    public void BattleInProgress(GameObject thisOne, GameObject otherOne)
    {
        //if the receiving object is the Player
        if (thisOne.tag.Equals("Player"))
        {

            //if the other object is a weapon, and the enemy landed a hit, deal damage accordingly
            if (otherOne.tag.Equals("Projectile") && Projectile.enemyShot)
            {
                Debug.Log("Player has been hit!");
                Character.enemyDmg = (EnemyStats.ATK + Random.Range(-2, 2)) - PlayerStats.DEF;
                PlayerStats.HP -= Character.enemyDmg;
                Projectile.enemyShot = false;
                Character.playerHit = true;
                Debug.Log("The enemy dealt " + Character.enemyDmg + " damage!");
                Destroy(otherOne);
            }
        }

        //if the receiving object is the enemy
        else if (thisOne.tag.Equals("Enemy"))
        {

            //if the other object is a weapon, and the player landed a hit, deal damage accordingly
            if (otherOne.tag.Equals("Projectile") && Projectile.playerShot)
            {
                Debug.Log("Enemy has been hit!");
                Character.playerDmg = (PlayerStats.ATK + Random.Range(-2, 2)) - EnemyStats.DEF;
                EnemyStats.HP -= Character.playerDmg;
                Projectile.playerShot = false;
                Character.enemyHit = true;
                Debug.Log("The player dealt " + Character.playerDmg + " damage!");
                Destroy(otherOne);
            }
        }

        //if the receiving object is the shield
        else if (thisOne.tag.Equals("Shield") && otherOne.tag.Equals("Projectile"))
        {
            //if the enemy's shield was hit by the player's projectile, deal reduced damage
            if (Projectile.playerShot)
            {
                Character.playerDmg = ((PlayerStats.ATK + Random.Range(-2, 2)) - EnemyStats.DEF) / 2;
                EnemyStats.HP -= Character.playerDmg;
                Debug.Log("The player dealt " + Character.playerDmg + " damage!");
                Projectile.playerShot = false;
            }

            //if the player's shield was hit by the enemy's projectile, deal reduced damage
            else if (Projectile.enemyShot)
            {
                Character.enemyDmg = (((EnemyStats.ATK) + Random.Range(-2, 2)) - PlayerStats.DEF) / 2;
                PlayerStats.HP -= Character.enemyDmg;
                Debug.Log("The enemy dealt " + Character.enemyDmg + " damage!");
                Projectile.enemyShot = false;
            }

            //Destroy the Projectile
            Destroy(otherOne);
        }
    }
}
