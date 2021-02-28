using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nerd : Enemy
{
    public float missRangeX;
    public float missRangeY;

    protected override void Throw()
    {
        gameObject.GetComponent<Animator>().SetTrigger("throw");
        
        Vector3 badPlayerPos = nerdRandomMiss();

        Vector2 dir = badPlayerPos - transform.position;

        float ballAngle = getBallRotation(badPlayerPos);
        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.Euler(new Vector3(0f, 0f, ballAngle)));
        ball.GetComponent<TrailRenderer>().enabled = false;
        ball.tag = "EnemyBall";
        ball.layer = 9;
        Rigidbody2D b = ball.GetComponent<Rigidbody2D>();
        ball.GetComponent<ParticleSystem>().Stop();
        if ((ballAngle > 90 && ballAngle <= 180) || (ballAngle < -90 && ballAngle >= -180))
        {
            ball.GetComponent<SpriteRenderer>().flipX = false;
            ball.GetComponent<SpriteRenderer>().flipY = true;
        }

        dir.Normalize();
        b.velocity = dir * throwSpeed;
    }

    private Vector3 nerdRandomMiss()
    {
        float missX = Random.Range(player.transform.position.x - missRangeX, player.transform.position.x + missRangeX);
        float missY = Random.Range(player.transform.position.y - missRangeY, player.transform.position.y + missRangeY);
        return new Vector3(missX, missY, 0);
    }

    protected override void OnHitSound()
    {
        SoundManager.S.HitSoundNerds();
    }
}
