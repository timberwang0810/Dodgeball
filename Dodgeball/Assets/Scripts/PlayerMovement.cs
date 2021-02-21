using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public float dodgeCooldown;
    private float dodgeTimer;
    public float dodgeForce;

    public Slider staminaBar;
    public Image staminaBarImage;
    private Color lowStaminaColor = Color.red;
    private Color highStaminaColor = Color.green;

    public Slider dodgeCoolDownBar;
    public Image dodgeCoolDownBarImage;
    private Color lowCoolDownColor = Color.red;
    private Color highCoolDownColor = Color.yellow;

    private void Start()
    {
        previous = transform.position;
        rb = GetComponent<Rigidbody2D>();
        isDashing = false;
        dodgeTimer = dodgeCooldown;
        origStamina = stamina;

        staminaBar.minValue = 0;
        staminaBar.maxValue = origStamina;
        staminaBar.value = stamina;
        staminaBarImage.color = highStaminaColor;

        dodgeCoolDownBar.minValue = 0;
        dodgeCoolDownBar.maxValue = dodgeCooldown;
        dodgeCoolDownBar.value = dodgeCooldown;
        dodgeCoolDownBarImage.color = highCoolDownColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.S.gameState != GameManager.GameState.playing) return;
        //horizontalMove = Mathf.Lerp(horizontalMove, Input.GetAxisRaw("Horizontal") * speed, accleration * Time.deltaTime);
        //verticalMove = Mathf.Lerp(verticalMove, Input.GetAxisRaw("Vertical") * speed, accleration * Time.deltaTime);
        dodgeTimer += Time.deltaTime;
        if (dodgeTimer <= dodgeCooldown) UpdateDodgeCoolDownBar();

        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector3(horizontalMove, verticalMove).normalized;
        float finalSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift) && !(dodgeTimer >= 0.0f && dodgeTimer < 0.2f))
        {
            isDashing = true;
            regenDelayTimer = 0;
            if (stamina > 0)
            {
                finalSpeed += dashBoost;
                stamina = Mathf.Clamp(stamina - (staminaSpendingRate * Time.deltaTime), 0, origStamina);
                UpdateStaminaBar();
            }
        }

        else
        {
            isDashing = false;
            if (stamina < origStamina)
            {
                if (regenDelayTimer >= regenDelay)
                {
                    stamina = Mathf.Clamp(stamina + (staminaRegeningRate * Time.deltaTime), 0, origStamina);
                    UpdateStaminaBar();
                }
                else regenDelayTimer += Time.deltaTime;
            }
            
        }
        rb.velocity = movement * finalSpeed;

        if (dodgeTimer >= 0.0f && dodgeTimer < 0.2f)
        {
            rb.velocity = rb.velocity * dodgeForce;
        }
        //Debug.Log("stamina " + stamina);
        //Vector2 newVel = new Vector2(transform.position.x - previous.x, transform.position.y - previous.y);

        //velocity = newVel / Time.deltaTime;

        //magnitude = (newVel.magnitude) / Time.deltaTime;
        //previous = transform.position;

        if (Input.GetKeyDown("c") && dodgeTimer >= dodgeCooldown)
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

    private void UpdateStaminaBar()
    {
        staminaBar.value = stamina;
        staminaBarImage.color = Color.Lerp(lowStaminaColor, highStaminaColor, stamina / origStamina);
    }

    private void UpdateDodgeCoolDownBar()
    {
        float clampedTimer = Mathf.Clamp(dodgeTimer, 0, dodgeCooldown);
        dodgeCoolDownBar.value = clampedTimer;
        dodgeCoolDownBarImage.color = Color.Lerp(lowCoolDownColor, highCoolDownColor, clampedTimer/dodgeCooldown);
    }
}
