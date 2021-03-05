using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    // Move State
    public enum MoveState { moving, idle };

    [Header("Enemy Attributes")]
    public GameObject ballPrefab;
    public float HP;
    public float damage;
    public float throwSpeed;
    public int score;

    public float startDelay;
    public float timeBetweenAttacks;
    private float actualAttackDelay;

    private bool ready = false;
    private float attackTimer;

    [Header("Enemy Movement Animation")]
    public float moveTime;
    public float idleTime;
    public float moveTimer;
    private MoveState status = MoveState.moving;

    [Header("Enemy Movement AI")]
    public float speed;
    public float timeBetweenDirectionChange;
    private float directionChangeTimer;
    private Vector2 currentDirection;

    protected Animator animator;
    private Rigidbody2D rb;
    protected GameObject player;

    private float previous;

    private bool throwing = false;
    private bool onCourt;
    private bool died = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("moving", true);
        attackTimer = timeBetweenAttacks;
        actualAttackDelay = timeBetweenAttacks;
        rb = GetComponent<Rigidbody2D>();
        GetComponent<SpriteRenderer>().flipX = false;
        previous = transform.position.x;
        player = GameObject.FindGameObjectWithTag("Player");
        gameObject.layer = 13;
        GenerateRandomDirection();
    }

    private void GetReady()
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        StartCoroutine(ReadyDelay());
    }

    // Delay sequence upon enemy entering the court
    private IEnumerator ReadyDelay()
    {
        //Debug.Log("getting ready");
        yield return new WaitForSeconds(startDelay);
        ready = true;
    }

    //// Start attack function
    //private void StartAttack()
    //{
    //    StartCoroutine(Attack());
    //}

    //// Attack Sequence
    //private IEnumerator Attack()
    //{
    //    //Throw();
    //    gameObject.GetComponent<Animator>().SetTrigger("throw");
    //    yield return new WaitForSeconds(startDelay);
    //    StartAttack();
    //}

    // Sound played when the enemy is hit
    protected abstract void OnHitSound();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Dies if hit by player ball
        if (collision.gameObject.tag == "PlayerBall")
        {
            SoundManager.S.HitSound();
            OnHitSound();
            GetComponent<CapsuleCollider2D>().enabled = false;
            throwing = false;
            GameManager.S.OnScoreAdded(score);
            GameManager.S.OnEnemyDestroyed();
            rb.velocity = new Vector2(0, 0);
            animator.SetTrigger("die");
            GetComponent<SpriteRenderer>().flipX = false;
            //change this later
            died = true;
            Destroy(this.gameObject, 1);
            Destroy(collision.gameObject);
        }
        // Move in a new direction if hit a wall or enemy
        else if (collision.gameObject.tag == "Walls" || collision.gameObject.tag == "Enemy")
        {
            //Debug.Log("Hit Wall");
            GenerateRandomDirection();
        }
    }

    // Throw mechanics
    protected abstract void Throw();

    // Get the rotation of the ball
    protected float getBallRotation(Vector3 position2)
    {
        Vector2 position1 = transform.position;
        return AngleBetweenTwoPoints(position1, position2);
    }

    // Helper to determine the angle between two points
    private float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    // Roaming function (automatic movement)
    private void Roam()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * currentDirection.x, speed * currentDirection.y);
        //Debug.Log(gameObject.GetComponent<Rigidbody2D>().velocity);
    }

    void Update()
    {
        // Pause enemy if on-court or keep enemy running if off-court
        if (GameManager.S.gameState != GameManager.GameState.playing)
        {
            if (onCourt)
            {
                ready = false;
                animator.SetBool("moving", false);
                status = MoveState.idle;
                rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
                return;
            } else
            {
                return;
            }

        }
        // Get ready to attack if on-court or keep enemy running if off-court
        else
        {
            if (onCourt)
            {
                GetReady();
            }
            else
            {
                RunEntrance();
                return;
            }
        }

        if (died) return;

     
        if (!throwing)
        {
            // After a certain time, generate a new random direction to go towards
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
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
            GetComponent<SpriteRenderer>().flipX = false;
        }

        if (ready)
        {
            directionChangeTimer += Time.deltaTime;

            // Attack the player after a certain time
            if (attackTimer >= actualAttackDelay)
            {
                Debug.Log(actualAttackDelay);
                animator.SetBool("moving", false);
                StartCoroutine(throwFreezePos());
                attackTimer = 0.0f;
                actualAttackDelay = timeBetweenAttacks + Random.Range(-2.0f, 2.0f);
            }
            else
            {
                attackTimer += Time.deltaTime;
            }
        }

    }

    // Throwing sequence
    private IEnumerator throwFreezePos()
    {
        throwing = true;
        gameObject.GetComponent<Animator>().SetTrigger("throw");
        yield return new WaitForSeconds(1);
        throwing = false;
    }

    // Helper to generate a random direction
    private void GenerateRandomDirection()
    {
        currentDirection.x = Random.Range(-1.0f, 1.0f);
        currentDirection.y = Random.Range(-1.0f, 1.0f);
        currentDirection.Normalize();
        directionChangeTimer = 0;
    }

    // Spawned enemy run in from entrance into the court
    // Assume enemies enter from right side
    private void RunEntrance()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = speed * Vector2.left;
    }

    // After enemy exit enemy entrance, it activates its roam and attack mechanics
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemyEntrance")
        {
            onCourt = true;
            gameObject.layer = 11;
        }
    }

    // Helper to flip the sprite
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
}
