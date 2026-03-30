using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 5f;
    [Range(0.01f, 50.0f)][SerializeField] private float moveRange = 8.0f;
    [Range(0.01f, 50.0f)][SerializeField] private float rangeOfSight = 10.0f;
    [Range(1.0f, 10.0f)][SerializeField] private float attackSpeedMultiplier = 2.0f;

    private Animator animator;
    private bool isFacingRight = false;
    private float startPositionX;
    private float startPositionY;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool bodyFading = false;

    private Rigidbody2D rigidBody;
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;

    private bool isMovingRight = false;


    // Start is called before the first frame update
    void Start()
    {
        ;
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        rigidBody.isKinematic = true;
        animator = GetComponent<Animator>();
        startPositionX = this.transform.position.x;
        startPositionY = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsAlive() && !isAttacking)
        {
            if(isMovingRight)
            {
                if(this.transform.position.x < startPositionX + moveRange)
                {
                    MoveRight();
                }
                else
                {
                    Flip();
                    isMovingRight = false;
                    MoveLeft();
                }
            }
            else
            {
                if(this.transform.position.x > startPositionX - moveRange)
                {
                    MoveLeft();
                }
                else
                {
                    Flip();
                    isMovingRight = true;
                    MoveRight();
                }
            }
            if(transform.position.y < startPositionY)
            {
                MoveUp();
                if(transform.position.y > startPositionY)
                {
                    transform.position = new Vector2(transform.position.x, startPositionY);
                }
            }
        }
        else
        {
            if(bodyFading)
            {
                Color tmp = spriteRenderer.color;
                tmp.a -= Time.deltaTime * 2;
                spriteRenderer.color = tmp;
            }
        }
        
        animator.SetBool("isDead", !IsAlive());
        animator.SetBool("isAttacking", isAttacking);
    }

    void FixedUpdate()
    {
        if (IsAlive() && !isAttacking && IsPlayerSpotted() && (transform.position.y == startPositionY))
        {
            Attack();
            Debug.Log("EAGLE ATTACK!");
        }
        else if (IsAlive() && isAttacking)
        {
            Debug.Log("Pos Y: " + transform.position.y);
            Debug.Log("Min pos Y: " + (startPositionY + Mathf.Cos(Mathf.PI / 4 * 3) * rangeOfSight));
            if (transform.position.y > (startPositionY + Mathf.Cos(Mathf.PI / 4 * 3) * rangeOfSight))
            {
                Debug.Log("Moving down");
                if(isMovingRight)
                {
                    MoveRightAttack();
                }
                else
                {
                    MoveLeftAttack();
                }
                MoveDownAttack();
            }
            else
            {
                isAttacking = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        /*
        if(other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            if (other.gameObject.transform.position.y > transform.position.y || player.IsFalling())
            {
                //isDead = true;
                //get killed by player using function Kill()

                StartCoroutine(KillOnAnimationEnd());

                rigidBody.isKinematic = false;
                circleCollider.isTrigger = false;

                if (isMovingRight)
                {
                    rigidBody.AddForce(Vector2.right * moveSpeed, ForceMode2D.Impulse);
                }
                else
                {
                    rigidBody.AddForce(Vector2.left * moveSpeed, ForceMode2D.Impulse);
                }

                isAttacking = false;
                //Kill();
            }
        }
        */
        if((other.CompareTag("Ground") || other.CompareTag("MovingPlatform")) && isAttacking)
        {
            isAttacking = false;
        }
    }

    IEnumerator KillOnAnimationEnd()
    {
        yield return new WaitForSeconds(10.0f);

        bodyFading = true;

        yield return new WaitForSeconds(2.0f);

        gameObject.SetActive(false);
    }

    void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        isFacingRight = !isFacingRight;
    }

    void MoveRight()
    {
        transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
    }

    void MoveLeft()
    {
        transform.Translate(moveSpeed * Time.deltaTime * -1, 0.0f, 0.0f, Space.World);
    }

    void MoveRightAttack()
    {
        transform.Translate(moveSpeed * Time.deltaTime * attackSpeedMultiplier, 0.0f, 0.0f, Space.World);
    }

    void MoveLeftAttack()
    {
        transform.Translate(moveSpeed * Time.deltaTime * -1 * attackSpeedMultiplier, 0.0f, 0.0f, Space.World);
    }

    void MoveDownAttack()
    {
        transform.Translate(0.0f, moveSpeed * Time.deltaTime * -1 * attackSpeedMultiplier, 0.0f, Space.World);
    }

    void MoveUp()
    {
        transform.Translate(0.0f, moveSpeed * Time.deltaTime, 0.0f, Space.World);
    }

    bool IsPlayerSpotted()
    {
        Vector3 eyeLine = new Vector3((isMovingRight ? 1 : -1) * Mathf.Sin(Mathf.PI/4 * 3) * rangeOfSight, Mathf.Cos(Mathf.PI / 4 * 3) * rangeOfSight, 0);

        Debug.DrawRay(this.transform.position, eyeLine, Color.red, 0.1f, false);

        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, eyeLine, rangeOfSight, LayerMask.GetMask("Player", "Ground"));
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    void Attack()
    {
        isAttacking = true;
    }

    public void Kill()
    {
        if(!isDead)
        {
            StartCoroutine(KillOnAnimationEnd());

            rigidBody.isKinematic = false;
            circleCollider.isTrigger = false;

            if (isMovingRight)
            {
                rigidBody.AddForce(Vector2.right * moveSpeed, ForceMode2D.Impulse);
                rigidBody.AddTorque(-0.2f * rigidBody.mass, ForceMode2D.Impulse);
            }
            else
            {
                rigidBody.AddForce(Vector2.left * moveSpeed, ForceMode2D.Impulse);
                rigidBody.AddTorque(0.2f * rigidBody.mass, ForceMode2D.Impulse);
            }

            isAttacking = false;
            isDead = true;
        }
        
    }

    public bool IsAlive()
    {
        return !isDead;
    }

    public void IgnoreCollision(Collider2D otherCollider)
    {
        Physics2D.IgnoreCollision(circleCollider, otherCollider, true);
    }
}
