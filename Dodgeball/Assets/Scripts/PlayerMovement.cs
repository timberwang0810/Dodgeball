using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float accleration;

    private float horizontalMove = 0.0f;
    private float verticalMove = 0.0f;
    private Rigidbody2D rb;

    Vector2 previous;
    public Vector2 velocity = new Vector2(0, 0);
    public float magnitude = 0;

    private void Start()
    {
        previous = transform.position;
        rb = GetComponent<Rigidbody2D>();
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
        rb.velocity = movement * speed;

        //Vector2 newVel = new Vector2(transform.position.x - previous.x, transform.position.y - previous.y);

        //velocity = newVel / Time.deltaTime;

        //magnitude = (newVel.magnitude) / Time.deltaTime;
        //previous = transform.position;
    }
}
