using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jock : Enemy
{
    protected override void Throw()
    {
        gameObject.GetComponent<Animator>().SetTrigger("throw");

        float ballAngle = getBallRotation(player.transform.position);
        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.Euler(new Vector3(0f, 0f, ballAngle)));
        ball.GetComponent<TrailRenderer>().enabled = false;
        ball.tag = "EnemyBall";
        ball.layer = 9;
        Rigidbody2D b = ball.GetComponent<Rigidbody2D>();
        ball.GetComponent<ParticleSystem>().Stop();

        Vector2 dir = player.transform.position - transform.position;
        dir.Normalize();
        b.velocity = dir * throwSpeed;
    }

    protected override void OnHitSound()
    {
        SoundManager.S.HitSound();
    }
}
