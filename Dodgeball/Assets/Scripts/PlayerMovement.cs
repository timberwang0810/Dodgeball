using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    
    private float horizontalMove = 0.0f;
    private float verticalMove = 0.0f;
    private Rigidbody2D rb;
    private bool isDashing;

    Vector2 previous;
    public Vector2 velocity = new Vector2(0, 0);
    public float magnitude = 0;

    public float dodgeCooldown;
    private float dodgeTimer;
    public float dodgeForce;

    public Slider dodgeCoolDownBar;
    public Image dodgeCoolDownBarImage;
    private Color lowCoolDownColor = Color.red;
    private Color highCoolDownColor = Color.yellow;

    private bool facingLeft;
    private SpriteRenderer mySpriteRenderer;
    private Animator animator;

    private void Start()
    {
        previous = transform.position;
        rb = GetComponent<Rigidbody2D>();
        isDashing = false;
        dodgeTimer = dodgeCooldown;

        dodgeCoolDownBar.minValue = 0;
        dodgeCoolDownBar.maxValue = dodgeCooldown;
        dodgeCoolDownBar.value = dodgeCooldown;
        dodgeCoolDownBarImage.color = highCoolDownColor;

        facingLeft = false;
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.S.gameState != GameManager.GameState.playing || GameManager.S.IsPaused()) return;
        //horizontalMove = Mathf.Lerp(horizontalMove, Input.GetAxisRaw("Horizontal") * speed, accleration * Time.deltaTime);
        //verticalMove = Mathf.Lerp(verticalMove, Input.GetAxisRaw("Vertical") * speed, accleration * Time.deltaTime);
        dodgeTimer += Time.deltaTime;
        if (dodgeTimer <= dodgeCooldown) UpdateDodgeCoolDownBar();

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

        //Debug.Log("stamina " + stamina);
        //Vector2 newVel = new Vector2(transform.position.x - previous.x, transform.position.y - previous.y);

        //velocity = newVel / Time.deltaTime;

        //magnitude = (newVel.magnitude) / Time.deltaTime;
        //previous = transform.position;

        if (Input.GetKeyDown(KeyCode.LeftShift) && dodgeTimer >= dodgeCooldown)
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
    private void UpdateDodgeCoolDownBar()
    {
        float clampedTimer = Mathf.Clamp(dodgeTimer, 0, dodgeCooldown);
        dodgeCoolDownBar.value = clampedTimer;
        dodgeCoolDownBarImage.color = Color.Lerp(lowCoolDownColor, highCoolDownColor, clampedTimer/dodgeCooldown);
    }
}
