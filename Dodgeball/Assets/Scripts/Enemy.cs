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
    public int score;

    public float startDelay;
    public float timeBetweenAttacks;

    private bool ready = false;
    private float attackTimer;

    private Animator animator;

    private void Start()
    {
        //GetReady();
        animator = GetComponent<Animator>();
        attackTimer = timeBetweenAttacks;
    }

    private void GetReady()
    {
        StartCoroutine(ReadyDelay());
    }

    private IEnumerator ReadyDelay()
    {
        //Debug.Log("getting ready");
        yield return new WaitForSeconds(startDelay);
        ready = true;
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
            SoundManager.S.HitSound();
            GetComponent<CapsuleCollider2D>().enabled = false;
            GameManager.S.OnScoreAdded(score);
            GameManager.S.OnEnemyDestroyed();
            Destroy(this.gameObject, 1.0f);
        }
    }

    private void Throw()
    {
        animator.SetTrigger("throw");
        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
        GameManager.S.OnBallSpawned();
        ball.tag = "EnemyBall";
        ball.layer = 9;
        Rigidbody2D b = ball.GetComponent<Rigidbody2D>();
        Vector2 dir = player.transform.position - transform.position;
        dir.Normalize();
        b.velocity = dir * throwSpeed;
    }

    void Update()
    {
        if (GameManager.S.gameState != GameManager.GameState.playing)
        {
            //Debug.Log("cant play");
            ready = false;
            return;
        } else
        {
            GetReady();
        }

        if (ready)
        {
            if (attackTimer >= timeBetweenAttacks)
            {
                Throw();
                attackTimer = 0.0f;
            }
            else
            {
                attackTimer += Time.deltaTime;
            }
        }

    }
}
