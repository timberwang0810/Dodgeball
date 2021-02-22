using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    // Start is called before the first frame update


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Walls")
        {
            SoundManager.S.WallHitSound();
            if (this.gameObject.tag.Equals("PlayerBall"))
            {
                Destroy(this.gameObject, 5.0f);
                GameManager.S.OnBallDespawned();
            }

            else if (this.gameObject.tag.Equals("EnemyBall"))
            {
                this.gameObject.tag = "Ball";
            }
        }
    }
}
