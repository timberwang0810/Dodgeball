using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float dashBoost;
    //public float accleration;

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

    private void Start()
    {
        previous = transform.position;
        rb = GetComponent<Rigidbody2D>();
        isDashing = false;
        dodgeTimer = dodgeCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        //if (GameManager.S.gameState != GameManager.GameState.playing) return;
        //horizontalMove = Mathf.Lerp(horizontalMove, Input.GetAxisRaw("Horizontal") * speed, accleration * Time.deltaTime);
        //verticalMove = Mathf.Lerp(verticalMove, Input.GetAxisRaw("Vertical") * speed, accleration * Time.deltaTime);
        dodgeTimer += Time.deltaTime;

        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector3(horizontalMove, verticalMove).normalized;
        float finalSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift) && !(dodgeTimer >= 0.0f && dodgeTimer < 0.2f))
        {
            finalSpeed += dashBoost;
            isDashing = true;
        }
        else isDashing = false;

        rb.velocity = movement * finalSpeed;

        if (dodgeTimer >= 0.0f && dodgeTimer < 0.2f)
        {
            rb.velocity = rb.velocity * dodgeForce;
        }
        //Vector2 newVel = new Vector2(transform.position.x - previous.x, transform.position.y - previous.y);

        //velocity = newVel / Time.deltaTime;

        //magnitude = (newVel.magnitude) / Time.deltaTime;
        //previous = transform.position;

        if (Input.GetKeyDown("c") && dodgeTimer >= dodgeCooldown)
        {
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
}
