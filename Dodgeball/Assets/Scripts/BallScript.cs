using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Walls")
        {
            GetComponent<TrailRenderer>().enabled = false;
            SoundManager.S.WallHitSound();
            Destroy(this.gameObject);
        }

        if (collision.gameObject.tag == "EnemyBall" || collision.gameObject.tag == "PlayerBall")
        {
            SoundManager.S.WallHitSound();
            SoundManager.S.WallHitSound();
            GetComponent<TrailRenderer>().enabled = false;
            Destroy(this.gameObject);
        }
    }
}
