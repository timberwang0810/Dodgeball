﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject ballPrefab;
    public float throwSpeed;

    private Rigidbody2D rb;
    private bool holding = false;
    private Animator animator;
    private ParticleSystem particles;
    private SpriteRenderer mySpriteRenderer;

    private float idleTimer;

    private bool iframes = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        particles = GetComponent<ParticleSystem>();
        particles.Stop();
        mySpriteRenderer = GetComponent<SpriteRenderer>();

        idleTimer = 15.0f;

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.S.gameState != GameManager.GameState.playing)
        {
            //Debug.Log("cant play");
            return;
        }

        if (Input.GetMouseButtonDown(0) && (holding || IsBuffed()))
        {
            Throw();
            if (!IsBuffed())
            {
                holding = false;
                animator.SetBool("holding", false);
            }
        }

        if (Input.GetKeyDown("space"))
        {
            animator.SetTrigger("parry");
            StartCoroutine(parryFrames());
        }


    }

    private IEnumerator parryFrames()
    {
        iframes = true;
        yield return new WaitForSeconds(0.2f);
        iframes = false;
    }

    private void Throw()
    {
        animator.SetTrigger("throw");

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 dir = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
        dir.Normalize();
        //Debug.Log(dir);

        float ballAngle = getBallRotation(Input.mousePosition);
        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.Euler(new Vector3(0f, 0f, ballAngle)));
        if ((ballAngle > 90 && ballAngle <= 180) || (ballAngle < -90 && ballAngle >= -180))
        {
            ball.GetComponent<SpriteRenderer>().flipX = false;
            ball.GetComponent<SpriteRenderer>().flipY = true;
        }

        idleTimer = 15.0f;
        SoundManager.S.ThrowSound();
        ball.tag = "PlayerBall";
        ball.layer = 8; // player layer
        Rigidbody2D b = ball.GetComponent<Rigidbody2D>();
        if (IsBuffed())
        {
            ball.GetComponent<ParticleSystem>().Play();
            b.velocity = dir * throwSpeed * 2;
        }
        else
        {
            ball.GetComponent<TrailRenderer>().enabled = false;
            ball.GetComponent<ParticleSystem>().Stop();
            b.velocity = dir * throwSpeed;
        }

        if (dir.x <= 0)
        {
            mySpriteRenderer.flipX = true;
            GetComponent<BoxCollider2D>().offset = new Vector2(-2.98f, 0);
        }
        else if (dir.x > 0)
        {
            mySpriteRenderer.flipX = false;
            GetComponent<BoxCollider2D>().offset = new Vector2(2.98f, 0);
        }

    }

    private float getBallRotation(Vector3 mousePos)
    {
        Vector2 position1 = transform.position;
        Vector2 position2 = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return AngleBetweenTwoPoints(position1, position2);
    }

    private float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "EnemyBall")
        {
            //this.transform.DetachChildren();
            Destroy(collision.gameObject);
            if (!iframes)
            {
                rb.velocity = new Vector2(0, 0);
                particles.Stop();
                holding = false;
                animator.SetTrigger("hit");
                mySpriteRenderer.flipX = false;
                GetComponent<BoxCollider2D>().offset = new Vector2(2.98f, 0);
                if (GameManager.S.gameState != GameManager.GameState.oops)
                {
                    Debug.Log("making sound");
                    SoundManager.S.PlayerHitSound();
                }
                GameManager.S.playerDied();
                animator.SetBool("holding", false);
                animator.SetBool("running", false);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemyBall") //parry
        {
            Destroy(collision.gameObject);
            SoundManager.S.WallHitSound();
            holding = true;             
            animator.SetBool("holding", true);
            rb.velocity = new Vector2(0, 0);
            SoundManager.S.ParrySound();
            GameManager.S.OnSuccessfulParry();
        }
    }

    private bool IsBuffed()
    {
        return GameManager.S.isPowerFilled();
    }
}
