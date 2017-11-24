using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    /*STATIC VARIABLES*/
    public static bool playerShot;  //if the player shot the projectile
    public static bool enemyShot;   //if the enemy shot the projectile

    /*PUBLIC VARIABLES*/
    public float speed;
    //public float dmgOut;

    [HideInInspector]
    public string tempTag;    //secondary of the projectile; dependent on who fired

    /*PRIVATE VARIABLES*/
    Rigidbody2D rb2d;   //rigidbody of the projectile

    private void Awake()
    {
        //initialize variables
        rb2d = GetComponent<Rigidbody2D>();

    }

    private void Start()
    {
        //move projectile when activated
        if (tempTag.Equals("Player"))
        {
            Debug.Log("Player has fired!");
            rb2d.velocity = Vector2.right * speed;
            playerShot = true;
        }

        else if (tempTag.Equals("Enemy"))
        {
            Debug.Log("Enemy has fired!");
            rb2d.velocity = Vector2.left * speed;
            enemyShot = true;
        }
    }
}
