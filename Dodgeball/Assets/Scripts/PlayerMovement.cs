﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float dodgeForce;

    private float horizontalMove = 0.0f;
    private float verticalMove = 0.0f;
    private Rigidbody2D rb;

    private float dodgeTimer;
    private bool facingLeft;
    private SpriteRenderer mySpriteRenderer;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dodgeTimer = GameManager.S.dodgeCooldown;
        facingLeft = false;
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.S.gameState != GameManager.GameState.playing || GameManager.S.IsPaused()) return;
        dodgeTimer += GameManager.S.isPowerFilled() ? Time.deltaTime * 2 : Time.deltaTime;
        if (dodgeTimer <= GameManager.S.dodgeCooldown) GameManager.S.UpdateDodgeCoolDownBar(dodgeTimer);

        horizontalMove = Input.GetAxisRaw("Horizontal");
        if (horizontalMove < 0)
        {
            facingLeft = true;
            mySpriteRenderer.flipX = true;
        }
        else if (horizontalMove > 0)
        {
            facingLeft = false;
            mySpriteRenderer.flipX = false;
        }

        verticalMove = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector3(horizontalMove, verticalMove).normalized;
        float finalSpeed = speed;
        

        if (dodgeTimer >= 0.0f && dodgeTimer < 0.2f)
        {
            rb.velocity = movement * finalSpeed * dodgeForce;
            StartCoroutine(dodgeTrail());
        }
        else
        {
            rb.velocity = movement * finalSpeed;
        }
        if ((horizontalMove == 0 && verticalMove == 0) || rb.velocity.magnitude < 1)
        {
            animator.SetBool("running", false);
        } else
        {
            animator.SetBool("running", true);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dodgeTimer >= GameManager.S.dodgeCooldown)
        {
            SoundManager.S.DodgeSound();
            Vector2 velocityCopy = rb.velocity;
            velocityCopy.Normalize();
            //Debug.Log(velocityCopy.magnitude);
            if (velocityCopy.magnitude > 0)
            {
                Debug.Log("dodge");
                //rb.AddForce(velocityCopy * dodgeForce, ForceMode2D.Impulse);
                dodgeTimer = 0.0f;
            }
        }

    }

    private IEnumerator dodgeTrail()
    {
        GetComponent<TrailRenderer>().enabled = true;
        yield return new WaitForSeconds(0.2f);
        GetComponent<TrailRenderer>().enabled = false;
    }
}
