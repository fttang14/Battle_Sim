using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    /*PUBLIC Variables*/
    public float speed;


    /*PRIVATE VARIABLES*/

    Rigidbody2D rb2d;   //rigidbody of the projectile

    private void Awake()
    {
        //initialize variables
        rb2d = GetComponent<Rigidbody2D>();

        //move projectile when activated
        rb2d.velocity = Vector2.right * speed;
    }


}
