using System.Collections;
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

    private bool iframes = false;

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
            return;
        }

        // Throw a ball if it has a ball or is buffed 
        if (Input.GetKeyDown(ControlManager.S.currThrowKeyCode) && (holding || IsBuffed()))
        {
            animator.SetTrigger("throw");
            //Throw();
            if (!IsBuffed())
            {
                holding = false;
                animator.SetBool("holding", false);
            }
        }

        // Parry
        if (Input.GetKeyDown(ControlManager.S.currParryKeyCode))
        {
            animator.SetTrigger("parry");
            StartCoroutine(parryFrames());
        }


    }

    // Parry frames sequence
    private IEnumerator parryFrames()
    {
        iframes = true;
        yield return new WaitForSeconds(0.2f);
        iframes = false;
    }

    // Throw mechanics
    private void Throw()
    {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 dir = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
        dir.Normalize();

        float ballAngle = getBallRotation(Input.mousePosition);
        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.Euler(new Vector3(0f, 0f, ballAngle)));
        if ((ballAngle > 90 && ballAngle <= 180) || (ballAngle < -90 && ballAngle >= -180))
        {
            ball.GetComponent<SpriteRenderer>().flipX = false;
            ball.GetComponent<SpriteRenderer>().flipY = true;
        }

        SoundManager.S.ThrowSound();
        ball.tag = "PlayerBall";
        ball.layer = 8; // player layer
        Rigidbody2D b = ball.GetComponent<Rigidbody2D>();

        // Extra effects for buffed throw
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

        // Sprite flipping
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

    // Get the rotation of the ball
    private float getBallRotation(Vector3 mousePos)
    {
        Vector2 position1 = transform.position;
        Vector2 position2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return AngleBetweenTwoPoints(position1, position2);
    }

    // Helper to determine the angle between two points
    private float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Dies if hit by enemy ball
        if (collision.gameObject.tag == "EnemyBall")
        {
            Destroy(collision.gameObject);
            if (!iframes)
            {
                rb.velocity = new Vector2(0, 0);
                particles.Stop();
                holding = false;
                animator.SetTrigger("hit");
                mySpriteRenderer.flipX = false;
                GetComponent<BoxCollider2D>().offset = new Vector2(2.98f, 0);
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
            animator.SetTrigger("parried");
            Destroy(collision.gameObject);
            SoundManager.S.WallHitSound();
            holding = true;             
            animator.SetBool("holding", true);
            rb.velocity = new Vector2(0, 0);
            SoundManager.S.ParrySound();
            GameManager.S.OnSuccessfulParry();
        }
    }

    // Helper to determine if player is buffed
    private bool IsBuffed()
    {
        return GameManager.S.isPowerFilled();
    }
}
