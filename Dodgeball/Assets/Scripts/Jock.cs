using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jock : Enemy
{
    protected override void Throw()
    {
        gameObject.GetComponent<Animator>().SetTrigger("throw");
        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
        ball.GetComponent<TrailRenderer>().enabled = false;
        GameManager.S.OnBallSpawned();
        ball.tag = "EnemyBall";
        ball.layer = 9;
        Rigidbody2D b = ball.GetComponent<Rigidbody2D>();
        ball.GetComponent<ParticleSystem>().Stop();
        Vector2 dir = player.transform.position - transform.position;
        dir.Normalize();
        b.velocity = dir * throwSpeed;
    }
}
