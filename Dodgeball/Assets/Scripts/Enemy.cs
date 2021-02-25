using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject player;
    public GameObject ballPrefab;

    [Header("Enemy Attributes")]
    public float HP;
    public float damage;
    public float throwSpeed;
    public int score;
    public float startDelay;
    public float timeBetweenAttacks;

    private bool ready = false;
    private float attackTimer;

    [Header("Enemy Movement AI")]
    public float speed;
    public float timeBetweenDirectionChange;
    private float directionChangeTimer;
    private Vector2 currentDirection;

    private Animator animator;

    private void Start()
    {
        //GetReady();
        animator = GetComponent<Animator>();
        attackTimer = timeBetweenAttacks;
        GenerateRandomDirection();
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
            Destroy(collision.gameObject);
        }

        else if (collision.gameObject.tag == "Walls" || collision.gameObject.tag == "Enemy")
        {
            //Debug.Log("Hit Wall");
            GenerateRandomDirection();
        }
    }

    private void Throw()
    {
        animator.SetTrigger("throw");
        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
        ball.GetComponent<TrailRenderer>().enabled = false;
        GameManager.S.OnBallSpawned();
        ball.tag = "EnemyBall";
        ball.layer = 9;
        Rigidbody2D b = ball.GetComponent<Rigidbody2D>();
        ball.GetComponent<ParticleSystem>().Stop();
        Vector2 dir = player.transform.position - transform.position;
        dir.Normalize();
        b.velocity = dir * throwSpeed;
    }

    private void Roam()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * currentDirection.x, speed * currentDirection.y);
        //Debug.Log(gameObject.GetComponent<Rigidbody2D>().velocity);
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

        if (directionChangeTimer >= timeBetweenDirectionChange)
        {
            GenerateRandomDirection();
        }
        Roam();

        if (ready)
        {
            directionChangeTimer += Time.deltaTime;

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

    private void GenerateRandomDirection()
    {
        currentDirection.x = Random.Range(-1.0f, 1.0f);
        currentDirection.y = Random.Range(-1.0f, 1.0f);
        currentDirection.Normalize();
        directionChangeTimer = 0;
    }
}
