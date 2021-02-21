using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float dashBoost;
    public float stamina;
    private float origStamina;
    //public float accleration;

    private float regenDelayTimer = 0.0f;
    public float staminaSpendingRate;
    public float staminaRegeningRate;
    public float regenDelay;

    private float horizontalMove = 0.0f;
    private float verticalMove = 0.0f;
    private Rigidbody2D rb;
    private bool isDashing;

    Vector2 previous;
    public Vector2 velocity = new Vector2(0, 0);
    public float magnitude = 0;

    private void Start()
    {
        previous = transform.position;
        rb = GetComponent<Rigidbody2D>();
        isDashing = false;
        origStamina = stamina;
    }

    // Update is called once per frame
    void Update()
    {
        //if (GameManager.S.gameState != GameManager.GameState.playing) return;
        //horizontalMove = Mathf.Lerp(horizontalMove, Input.GetAxisRaw("Horizontal") * speed, accleration * Time.deltaTime);
        //verticalMove = Mathf.Lerp(verticalMove, Input.GetAxisRaw("Vertical") * speed, accleration * Time.deltaTime);
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector3(horizontalMove, verticalMove).normalized;
        float finalSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            finalSpeed += dashBoost;
            isDashing = true;
            stamina = Mathf.Clamp(stamina - (staminaSpendingRate * Time.deltaTime), 0, origStamina);
            regenDelayTimer = 0;
        }

        else
        {
            isDashing = false;
            if (stamina < origStamina)
            {
                if (regenDelayTimer >= regenDelay)
                {
                    stamina = Mathf.Clamp(stamina + (staminaRegeningRate * Time.deltaTime), 0, origStamina);
                }
                else regenDelayTimer += Time.deltaTime;
            }
            
        }
        rb.velocity = movement * finalSpeed;
        Debug.Log("stamina " + stamina);
        //Vector2 newVel = new Vector2(transform.position.x - previous.x, transform.position.y - previous.y);

        //velocity = newVel / Time.deltaTime;

        //magnitude = (newVel.magnitude) / Time.deltaTime;
        //previous = transform.position;
    }
}
