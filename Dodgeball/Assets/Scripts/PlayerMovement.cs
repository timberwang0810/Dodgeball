using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float dodgeForce;
    public float dodgeCooldown;

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
        dodgeTimer = dodgeCooldown;
        facingLeft = false;
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.S.gameState != GameManager.GameState.playing) return;
        dodgeTimer += GameManager.S.isPowerFilled() ? Time.deltaTime * 2 : Time.deltaTime;

        horizontalMove = Input.GetAxisRaw("Horizontal");
        if (horizontalMove < 0)
        {
            facingLeft = true;
            mySpriteRenderer.flipX = true;
            GetComponent<BoxCollider2D>().offset = new Vector2(-2.98f, 0);
        }
        else if (horizontalMove > 0)
        {
            facingLeft = false;
            mySpriteRenderer.flipX = false;
            GetComponent<BoxCollider2D>().offset = new Vector2(2.98f, 0);
        }

        verticalMove = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector3(horizontalMove, verticalMove).normalized;
        float finalSpeed = speed;
        

        if (dodgeTimer >= 0.0f && dodgeTimer < 0.2f)
        {
            rb.velocity = movement * finalSpeed * dodgeForce;
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

        if (Input.GetKeyDown(KeyCode.LeftShift) && dodgeTimer >= dodgeCooldown)
        {
            SoundManager.S.DodgeSound();
            Vector2 velocityCopy = rb.velocity;
            velocityCopy.Normalize();
            StartCoroutine(dodgeTrail());
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

        // this let's player still be obstructed by the border
        this.gameObject.layer = 11;
        //Debug.Log("it's gone");
        yield return new WaitForSeconds(0.2f);
        this.gameObject.layer = 10;
        //Debug.Log("its back");
        yield return new WaitForSeconds(0.3f);
        GetComponent<TrailRenderer>().enabled = false;
    }
}
