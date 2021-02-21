using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject ballPrefab;
    public float throwSpeed;
    private bool buffed = false;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   
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
        if (Input.GetMouseButtonDown(0))
        {
            Throw();
        }
    }

    private void Throw()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 dir = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
        dir.Normalize();
        //Debug.Log(dir);

        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
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
            b.velocity = dir * throwSpeed;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "EnemyBall")
        {
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
            }
        }
    }
}
