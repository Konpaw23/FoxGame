using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityStandardAssets._2D;

public class Player : MonoBehaviour
{
    [Header("Movement parameters")]
    [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 0.1f; // moving speed of the player
    [Range(0.01f, 100.0f)][SerializeField] private float jumpForce = 6.0f; // jump force of the player
    [Range(0.01f, 20.0f)][SerializeField] private float mainGravityScale = 3.0f; // gravity strength for the player
    [Range(0.01f, 20.0f)][SerializeField] private float climbingSpeed = 0.1f; // climbing speed of the player

    [SerializeField] AudioClip bonusSound;
    [SerializeField] AudioClip keySound;
    [SerializeField] AudioClip enemyKilledSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip heartSound;
    [SerializeField] GameObject playerCamera;

    private BoxCollider2D boxCollider;
    private AudioSource source;
    private Rigidbody2D rigidBody;
    public LayerMask groundLayer;
    private Animator animator;
    private bool isWalking = false;
    private bool isFacingRight = true;
    //private int score = 0;
    private bool isLadder = false;
    private bool isOnLadder = false;
    private bool isClimbing = false;
    private bool isAlive = true;
    private float currentLadderXPos = 0;
    float vertical = 0;
    private Vector2 StartPosition;

    private CheckpointController checkPoint = null;

    const float rayLength = 1.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rigidBody.gravityScale = mainGravityScale;
        StartPosition = transform.position;
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if(GameManager.instance.currentGameState == GameState.GS_GAME)
        {
            isWalking = false;
            if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                isOnLadder = false;
                isClimbing = false;
                //transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
                Vector2 movePosition = new Vector2(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y);
                rigidBody.MovePosition(movePosition);
                isWalking = true;
                if (isFacingRight == false)
                    Flip();
            }
            
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                isOnLadder = false;
                isClimbing = false;
                transform.Translate(moveSpeed * Time.deltaTime * -1, 0.0f, 0.0f, Space.World);
                isWalking = true;
                if (isFacingRight == true)
                    Flip();
            }

            vertical = Input.GetAxisRaw("Vertical");
            if (isLadder && (vertical != 0))
            {
                if(!(IsGrounded() && vertical < 0))
                {
                    // transform.position = Vector3(currentLadderXPos, transform.position.y, transform.position.z);
                    rigidBody.velocity = Vector2.zero;
                    isOnLadder = true;
                    isClimbing = true;
                }
                else
                {
                    isOnLadder = false; 
                    isClimbing = false;
                }
                
            }
            else if (isOnLadder == true)
            {
                isClimbing = false;
            }*/

        /*if (isOnLadder && IsGrounded() && vertical < 0)
        {
            isOnLadder = false;
            isClimbing = false;
        }*/

        /*if (isOnLadder)
        {
            rigidBody.gravityScale = 0;
            if (isClimbing)
            {
                transform.Translate(0.0f , climbingSpeed * Time.deltaTime * vertical, 0.0f, Space.World);
            }
        }
        else
        {
            rigidBody.gravityScale = mainGravityScale;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (IsGrounded() || isOnLadder)
            {
                Jump();
            }
            isOnLadder = false;
            isClimbing = false;
        }

        animator.SetBool("isGrounded", IsGrounded());
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isClimbing", isClimbing);
        animator.SetBool("isFalling", IsFalling());
        animator.SetBool("isOnLadder", isOnLadder);
    }*/



        if (GameManager.instance.currentGameState == GameState.GS_GAME && isAlive)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                if (IsGrounded() || isOnLadder)
                {
                    Jump();
                }
                isOnLadder = false;
                isClimbing = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (GameManager.instance.currentGameState == GameState.GS_GAME)
        {
            if(isAlive)
            {
                isWalking = false;
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                {
                    isOnLadder = false;
                    isClimbing = false;
                    transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
                    //Vector2 movePosition = new Vector2(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y);
                    //rigidBody.MovePosition(movePosition);
                    isWalking = true;
                    if (isFacingRight == false)
                        Flip();
                }

                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                {
                    isOnLadder = false;
                    isClimbing = false;
                    transform.Translate(moveSpeed * Time.deltaTime * -1, 0.0f, 0.0f, Space.World);
                    isWalking = true;
                    if (isFacingRight == true)
                        Flip();
                }

                vertical = Input.GetAxisRaw("Vertical");
                if (isLadder && IsLadderAccessible() && (vertical != 0))
                {
                    if (!isOnLadder)
                    {
                        Vector3Int cellPositionGrid = GameManager.instance.grid.WorldToCell(transform.position);
                        Vector2 realCellPosition = GameManager.instance.grid.GetCellCenterWorld(cellPositionGrid);
                        transform.position = new Vector2(realCellPosition.x, transform.position.y);
                    }
                    if (!(IsGrounded() && vertical < 0))
                    {
                        // transform.position = Vector3(currentLadderXPos, transform.position.y, transform.position.z);
                        rigidBody.velocity = Vector2.zero;
                        isOnLadder = true;
                        isClimbing = true;
                    }
                    else
                    {
                        isOnLadder = false;
                        isClimbing = false;
                    }

                }
                else if (isOnLadder == true)
                {
                    if (!IsLadderAccessible() && vertical < 0)
                    {
                        isClimbing = true;
                    }
                    else
                    {
                        isClimbing = false;
                    }
                }

                /*if (isOnLadder && IsGrounded() && vertical < 0)
                {
                    isOnLadder = false;
                    isClimbing = false;
                }*/

                if (isOnLadder)
                {
                    rigidBody.gravityScale = 0;
                    if (isClimbing)
                    {
                        transform.Translate(0.0f, climbingSpeed * Time.deltaTime * vertical, 0.0f, Space.World);
                    }
                }
                else
                {
                    rigidBody.gravityScale = mainGravityScale;
                }

                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    if (IsGrounded() || isOnLadder)
                    {
                        Jump();
                    }
                    isOnLadder = false;
                    isClimbing = false;
                }
            }
            animator.SetBool("isGrounded", IsGrounded());
            animator.SetBool("isWalking", isWalking);
            animator.SetBool("isClimbing", isClimbing);
            animator.SetBool("isFalling", IsFalling());
            animator.SetBool("isOnLadder", isOnLadder);
            animator.SetBool("isDead", !isAlive);
        }
        /*if(isOnLadder)
        {
            rigidBody.gravityScale = 0;
            if(isClimbing)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, vertical* moveSpeed);
            }
        }
        else
        {
            rigidBody.gravityScale = mainGravityScale;
        }*/
    }

    void Jump()
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        //Debug.Log("Jump!");
    }

    bool IsGrounded()
    {
        int offsetMul = (isFacingRight ? 1 : -1);
        Vector3 vectorL = new Vector3(-1 * (boxCollider.size.x) / 2 + boxCollider.offset.x * offsetMul, 0, 0);
        Vector3 vectorR = new Vector3((boxCollider.size.x) / 2 + boxCollider.offset.x * offsetMul, 0, 0);

        Debug.DrawRay(this.transform.position + vectorL, rayLength * Vector3.down, Color.white, 1, false);
        Debug.DrawRay(this.transform.position + vectorR, rayLength * Vector3.down, Color.white, 1, false);

        return Physics2D.Raycast(this.transform.position + vectorL, Vector2.down, rayLength, groundLayer.value) ||
            Physics2D.Raycast(this.transform.position + vectorR, Vector2.down, rayLength, groundLayer.value);
    }

    public bool IsFalling()
    {
        if((!IsGrounded()) && (rigidBody.velocity.y < 0))
        {
            return true;
        }
        return false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "FallLevel")
        {
            //Debug.Log("Player fell off level");
            StartCoroutine(KillWithoutAnimation());
        }
            
        if(other.CompareTag("Bonus"))
        {
            source.PlayOneShot(bonusSound, AudioListener.volume);
            GameManager.instance.AddPoints(1);
            other.gameObject.SetActive(false);
        }
        if (other.CompareTag("Ladder"))
        {
            isLadder = true;
            currentLadderXPos = other.transform.position.x;
        }
        if(other.CompareTag("Key"))
        {
            source.PlayOneShot(keySound, AudioListener.volume);
            GameManager.instance.AddKey();
            other.gameObject.SetActive(false);
        }
        if(other.CompareTag("Heart"))
        {
            source.PlayOneShot(heartSound, AudioListener.volume);
            GameManager.instance.GiveLive();
            other.gameObject.SetActive(false);
        }
        if(other.CompareTag("Exit"))
        {
            GameManager.instance.ExitInfo();
        }
        if(other.CompareTag("MovingPlatform"))
        {
            transform.SetParent(other.transform);
        }
        if(other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy.IsAlive())
            {
                if(transform.position.y > other.gameObject.transform.position.y && !IsGrounded())
                {
                    source.PlayOneShot(enemyKilledSound, AudioListener.volume);
                    GameManager.instance.AddPoints(3);
                    GameManager.instance.EnemiesKilledNumberInc();
                    enemy.Kill();
                    enemy.IgnoreCollision(boxCollider);
                    Jump();
                    //Debug.Log("Killed an enemy");
                }
                else
                {
                    Kill();
                }
            }
        }
        if(other.CompareTag("Checkpoint"))
        {
            if(checkPoint != null)
            {
                checkPoint.Deactivate();
            }
            checkPoint = other.GetComponent<CheckpointController>();
            checkPoint.Activate();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
            isOnLadder = false;
        }
        if(other.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null);
        }
        if(other.CompareTag("Exit"))
        {
            GameManager.instance.DisableExitInfo();
            GameManager.instance.DisableCanEndLevel();
        }
    }

    //is ladder directly on player position
    bool IsLadderAccessible()
    {
        Vector3 playerLowerPosition = new Vector3(transform.position.x, transform.position.y - 0.75f, 0.0f);

        Collider2D playerCollision = Physics2D.OverlapPoint(playerLowerPosition, LayerMask.GetMask("Ladder"));

        if (playerCollision != null && playerCollision.CompareTag("Ladder"))
        {
            return true;
        }
        return false;
    }

    void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        isFacingRight = !isFacingRight;
    }

    public IEnumerator KillWithoutAnimation()
    {
        if (isAlive)
        {
            GameManager.instance.DecreaseLive();
            CameraFollow cameraFollow = playerCamera.GetComponent<CameraFollow>();
            cameraFollow.isFollowing = false;
            source.PlayOneShot(deathSound, AudioListener.volume);

            isAlive = false;
            yield return new WaitForSeconds(1.5f);

            if (checkPoint == null)
            {
                transform.position = StartPosition;
            }
            else
            {
                transform.position = checkPoint.GetPosition();
            }

            isAlive = true;
            cameraFollow.isFollowing = true;
        }
    }

    public void Kill()
    {
        if (isAlive)
        {
            GameManager.instance.DecreaseLive();
            StartCoroutine(KillAnimation());
        }
    }

    IEnumerator KillAnimation()
    {
        Quaternion rotation = transform.rotation;

        CameraFollow cameraFollow = playerCamera.GetComponent<CameraFollow>();
        cameraFollow.isFollowing = false;

        source.PlayOneShot(deathSound, AudioListener.volume);
        boxCollider.enabled = false;
        rigidBody.freezeRotation = false;
        BounceBackDeath();
        isAlive = false;
        
        yield return new WaitForSeconds(1.5f);

        if (checkPoint == null)
        {
            transform.position = StartPosition;
        }
        else
        {
            transform.position = checkPoint.GetPosition();
        }

        isAlive = true;
        cameraFollow.isFollowing = true;

        transform.rotation = rotation;
        boxCollider.enabled = true;
        rigidBody.freezeRotation = true;
    }

    void BounceBackDeath()
    {
        Jump();
        if (isFacingRight)
        {
            rigidBody.AddForce(Vector2.left * 3, ForceMode2D.Impulse);
            rigidBody.AddTorque(0.6f * rigidBody.mass, ForceMode2D.Impulse);
        }
        else
        {
            rigidBody.AddForce(Vector2.right * 3, ForceMode2D.Impulse);
            rigidBody.AddTorque(-0.6f * rigidBody.mass, ForceMode2D.Impulse);
        }
    }
}
