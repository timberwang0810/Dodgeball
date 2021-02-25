using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nerd : MonoBehaviour
{
    public enum MoveState {moving, idle};

    public float missRangeX;
    public float missRangeY;

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

    public float moveTime;
    public float idleTime;
    public float moveTimer;
    private MoveState status = MoveState.idle;

    [Header("Enemy Movement AI")]
    public float speed;
    public float timeBetweenDirectionChange;
    private float directionChangeTimer;
    private Vector2 currentDirection;

    private Animator animator;
    private Rigidbody2D rb;

    private float previous;

    private bool throwing = false;

    private void Start()
    {
        //GetReady();
        animator = GetComponent<Animator>();
        attackTimer = timeBetweenAttacks;
        rb = GetComponent<Rigidbody2D>();
        GetComponent<SpriteRenderer>().flipX = false;
        previous = transform.position.x;
        GenerateRandomDirection();
    }

    private void GetReady()
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
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
            throwing = false;
            GameManager.S.OnScoreAdded(score);
            GameManager.S.OnEnemyDestroyed();
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
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
        
        Vector3 badPlayerPos = nerdRandomMiss();

        Vector2 dir = badPlayerPos - transform.position;
        dir.Normalize();
        b.velocity = dir * throwSpeed;
    }

    private Vector3 nerdRandomMiss()
    {
        float missX = Random.Range(player.transform.position.x - missRangeX, player.transform.position.x + missRangeX);
        float missY = Random.Range(player.transform.position.y - missRangeY, player.transform.position.y + missRangeY);
        return new Vector3(missX, missY, 0);
    }

    private void Roam()
    {
        rb.velocity = new Vector2(speed * currentDirection.x, speed * currentDirection.y);
        //Debug.Log(gameObject.GetComponent<Rigidbody2D>().velocity);
    }

    void Update()
    {
        if (GameManager.S.gameState != GameManager.GameState.playing)
        {
            //Debug.Log("cant play");
            ready = false;
            animator.SetBool("moving", false);
            status = MoveState.idle;
            rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
            return;
        }
        else
        {
            GetReady();
        }

        if (!throwing)
        {
            if (directionChangeTimer >= timeBetweenDirectionChange)
            {
                GenerateRandomDirection();
            }

            if (status == MoveState.idle)
            {
                if (moveTimer < idleTime)
                {
                    //do nothing
                    rb.velocity = new Vector2(0, 0);
                    moveTimer += Time.deltaTime;
                    animator.SetBool("moving", false);
                }
                else
                {
                    moveTimer = 0;
                    status = MoveState.moving;
                }

            }
            else if (status == MoveState.moving)
            {
                if (moveTimer < moveTime)
                {
                    Roam();
                    moveTimer += Time.deltaTime;
                    animator.SetBool("moving", true);
                }
                else
                {
                    moveTimer = 0;
                    status = MoveState.idle;

                }
            }
            flipSprite();
        } else
        {
            rb.velocity = new Vector2(0, 0);
            GetComponent<SpriteRenderer>().flipX = false;
        }

        if (ready)
        {
            directionChangeTimer += Time.deltaTime;

            if (attackTimer >= timeBetweenAttacks)
            {
                animator.SetBool("moving", false);
                StartCoroutine(throwFreezePos());
                attackTimer = 0.0f;
            }
            else
            {
                attackTimer += Time.deltaTime;
            }
        }

    }

    private void flipSprite()
    {
        float facing = (transform.position.x - previous) / Time.deltaTime;
        if (facing < 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (facing > 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        previous = transform.position.x;
    }

    private IEnumerator throwFreezePos()
    {
        throwing = true;
        Throw();
        yield return new WaitForSeconds(1);
        throwing = false;
    }

    private void GenerateRandomDirection()
    {
        currentDirection.x = Random.Range(-1.0f, 1.0f);
        currentDirection.y = Random.Range(-1.0f, 1.0f);
        currentDirection.Normalize();
        directionChangeTimer = 0;
    }
}
