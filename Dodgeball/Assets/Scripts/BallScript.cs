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
            SoundManager.S.WallHitSound();
            OnBallGrounded();
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
