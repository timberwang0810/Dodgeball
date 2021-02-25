using System.Collections;
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
    private ParticleSystem particles;
    private SpriteRenderer mySpriteRenderer;
    private bool particlesPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        particles = GetComponent<ParticleSystem>();
        particles.Stop();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
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
            rb.velocity = new Vector2(0, 0);
        }
    }

    private void Throw()
    {
        animator.SetTrigger("throw");
        rb.velocity = new Vector2(0, 0);

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
        if (IsBuffed())
        {
            b.velocity = dir * throwSpeed * 2;
        }
        else
        {
            ball.GetComponent<TrailRenderer>().enabled = false;
            b.velocity = dir * throwSpeed;
        }

        if (dir.x <= 0)
        {
            mySpriteRenderer.flipX = true;
            ball.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (dir.x > 0)
        {
            mySpriteRenderer.flipX = false;
            ball.GetComponent<SpriteRenderer>().flipX = true;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "EnemyBall")
        {
            //this.transform.DetachChildren();
            Destroy(collision.gameObject);
            rb.velocity = new Vector2(0, 0);
            particles.Stop();
            particlesPlaying = false;
            buffed = false;
            holding = false;
            animator.SetTrigger("hit");
            
            if (GameManager.S.gameState != GameManager.GameState.oops)
            {
                Debug.Log("making sound");
                SoundManager.S.HitSound();
            }
            GameManager.S.playerDied();
            animator.SetBool("holding", false);
            animator.SetBool("running", false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemyBall") //parry
        {
            Destroy(collision.gameObject);
            holding = true;             
            animator.SetBool("holding", true);
            rb.velocity = new Vector2(0, 0);
            GameManager.S.OnSuccessfulParry();
        }
    }

    private bool IsBuffed()
    {
        return GameManager.S.isPowerFilled();
    }
}
