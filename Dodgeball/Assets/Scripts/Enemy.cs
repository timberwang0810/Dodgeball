using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject player;
    public GameObject ballPrefab;
    public float HP;
    public float damage;
    public float throwSpeed;

    public float startDelay;
    public float timeBetweenAttacks;

    private void Start()
    {
        GetReady();
    }

    private void GetReady()
    {
        StartCoroutine(ReadyDelay());
    }

    private IEnumerator ReadyDelay()
    {
        yield return new WaitForSeconds(startDelay);
        StartAttack();
    }

    private void StartAttack()
    {
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        Throw();
        yield return new WaitForSeconds(startDelay);
        StartAttack();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlayerBall")
        {
            Destroy(this.gameObject);
        }
    }

    private void Throw()
    {
        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
        ball.tag = "EnemyBall";
        ball.layer = 9;
        Rigidbody2D b = ball.GetComponent<Rigidbody2D>();
        Vector2 dir = player.transform.position - transform.position;
        dir.Normalize();
        b.velocity = dir * throwSpeed;
    }
}
