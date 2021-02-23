﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject ballPrefab;
    public float throwSpeed;
    private bool buffed = false;
    private Rigidbody2D rb;
    private bool holding = false;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.S.gameState != GameManager.GameState.playing)
        {
            //Debug.Log("cant play");
            return;
        }
        //Debug.Log("pplay");
        if (Input.GetMouseButtonDown(0) && holding)
        {
            Throw();
            holding = false;
            animator.SetBool("holding", false);
        }
    }

    private void Throw()
    {
        animator.SetTrigger("throw");

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 dir = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
        dir.Normalize();
        //Debug.Log(dir);

        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
        SoundManager.S.ThrowSound();
        ball.tag = "PlayerBall";
        ball.layer = 8; // player layer
        Rigidbody2D b = ball.GetComponent<Rigidbody2D>();
        if (buffed)
        {
            b.velocity = dir * throwSpeed * 2;
            buffed = false; 
        }
        else
        {
            ball.GetComponent<TrailRenderer>().enabled = false;
            b.velocity = dir * throwSpeed;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "EnemyBall")
        {
            SoundManager.S.HitSound();
            //this.transform.DetachChildren();
            //Destroy(this.gameObject);
            rb.velocity = new Vector2(0, 0);
            GameManager.S.playerDied();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemyBall")
        {
            if (Input.GetKeyDown("space"))
            {
                Debug.Log("caught!");
                Destroy(collision.gameObject);
                buffed = true;
                holding = true;
                animator.SetBool("holding", true);
            }
        }
        else if (collision.gameObject.tag == "Ball")
        {
            if (Input.GetKeyDown("space") && !holding)
            {
                Debug.Log("picked up");
                Destroy(collision.gameObject);
                holding = true;
                animator.SetBool("holding", true);
            }
        }
    }
}
