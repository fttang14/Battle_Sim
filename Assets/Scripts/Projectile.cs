using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    /*STATIC VARIABLES*/
    public static bool playerShot;

    /*PUBLIC VARIABLES*/
    public float speed;
    public float dmgOut;


    /*PRIVATE VARIABLES*/

    Rigidbody2D rb2d;   //rigidbody of the projectile

    private void Awake()
    {
        //initialize variables
        rb2d = GetComponent<Rigidbody2D>();

        //move projectile when activated
        rb2d.velocity = Vector2.right * speed;

        //WORK IN PROGRESS
        if (this.gameObject.CompareTag("Enemy"))
        {
            playerShot = false;
        }
        else
        {
            playerShot = true;
        }
        

        //FIXED DAMAGE OUTPUTS
        MegaManX.storeDmg = dmgOut;
        Debug.Log(dmgOut);
    }


}
