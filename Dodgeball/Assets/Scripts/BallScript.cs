using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public float takeoffVeloctiy;
    private bool inAir;

    private void Start()
    {
        inAir = true;
    }
    private void Update()
    {
        if (inAir)
        {
            float currVelocity = gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            if (currVelocity <= takeoffVeloctiy) OnBallGrounded();
        }
        //Debug.Log(gameObject.GetComponent<Rigidbody2D>().velocity.magnitude);
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Walls")
        {
            GetComponent<TrailRenderer>().enabled = false;
            SoundManager.S.WallHitSound();
            Destroy(this.gameObject);
            //OnBallGrounded();
        }
        if (collision.gameObject.tag == "EnemyBall" || collision.gameObject.tag == "PlayerBall")
        {
            SoundManager.S.WallHitSound();
            SoundManager.S.WallHitSound();
            GetComponent<TrailRenderer>().enabled = false;
            Destroy(this.gameObject);
        }
    }

    private void OnBallGrounded()
    {
        inAir = false;
        if (this.gameObject.tag.Equals("PlayerBall"))
        {
            this.gameObject.tag = "Ball";
            Destroy(this.gameObject, 5.0f);
            GameManager.S.OnBallDespawned();
        }

        else if (this.gameObject.tag.Equals("EnemyBall"))
        {
            this.gameObject.tag = "Ball";
        }
    }
}
