using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    
    public float speed = 5f;
    public bool isFacingRight = true;
    private float wHeight;
    private float wWidth;
    private float health = 100f;

    [Header("Jumping")]
    public float jumpSpeed = 7f;
    public bool isJumping = false;
    private float jumpButtonPressTime = 0f;
    public float maxJumpTime = 0.2f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float wallJumpY = 10f;
    public bool isWallJumping = false;

    private float rayCastLength = 0.005f;

    [Header("Dashing")]
    private float dashTime;
    public float startDashTime;
    public float dashForce;
    public float startDashTimer;
    float currentDashTimer;
    bool isDashing;
    float dashDir;

    public bool onGround;
    public bool onWall;

    public float wallSlideSpeed;

    void Start()
    {
        health = 100f;

        dashTime = startDashTime;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // wHeight: the bottom part of the Player
        wHeight = GetComponent<Collider2D>().bounds.extents.y + 0.05f;
        // wWidth: the left or right part of the Player
        wWidth = GetComponent<Collider2D>().bounds.extents.x + 0.05f;
    }


    void FixedUpdate()
    {
        onGround = IsOnGround();
        onWall = IsOnWall();

        /*** HORIZONTAL MOVEMENT ***/
        // Get Horizontal Movement (Walking)
        float horzMove = Input.GetAxisRaw("Horizontal");

        // Move Player according to direction
        Vector2 vect = rb.velocity;
        rb.velocity = new Vector2(horzMove * speed, vect.y);

        // Check movement direction and make player face towards direction
        FlipPlayer(horzMove);

        /*** VERTICAL MOVEMENT ***/
        // Get Vertical Movement (Jumping)
        float vertMove = Input.GetAxisRaw("Vertical");
        
        if (Input.GetButtonDown("Jump") || vertMove > 0f)
        {
            isJumping = true;
            rb.velocity = Vector2.up * jumpSpeed;
        }
        
        /*
        if (IsOnGround() && isJumping == false)
        {
            if (vertMove > 0f)
            {
                isJumping = true;
            }
        }
        */

        // Check if Player is busy jumping & jump time hasn't elapsed yet
        if (isJumping && jumpButtonPressTime < maxJumpTime)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        // Check if Player is still busy jumping
        if (Input.GetButton("Jump")) // || (vertMove > 0f))
        {
            jumpButtonPressTime += Time.deltaTime;
        }
        else
        {
            isJumping = false;
            jumpButtonPressTime = 0f;
        }

        // Check if Jumping time period has elapsed
        if (jumpButtonPressTime > maxJumpTime)
        {
            rb.velocity = Vector2.zero;
            // vertMove = 0f;
        }

        /* DASHING */
        if (Input.GetButtonDown("Dash") && !IsOnGround() && horzMove != 0)
        {
            isDashing = true;
            currentDashTimer = startDashTimer;
            rb.velocity = Vector2.zero;
            dashDir = (int)horzMove;
        }

        if (isDashing)
        {
            rb.velocity = transform.right * dashDir * dashForce;
            currentDashTimer -= Time.deltaTime;

            if (currentDashTimer <= 0)
            {
                isDashing = false;
            }
        }

        /* On WALLS */
        if (onWall && !onGround)
        {
            // Slide on Walls
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }
        if (onWall && Input.GetButton("WallGrab"))
        {
            rb.velocity = new Vector2(rb.velocity.x, vertMove * speed);
        }
        
        // Wall Jumps
        if (rb.velocity.x > 0 && onWall && !onGround)
        {
            isWallJumping = true;
            Invoke("SetWallJumpingToFalse", 0.05f);
        }
        if (isWallJumping)
        {
            rb.velocity = new Vector2(-GetWallDirection() * speed, wallJumpY);
        }
    }

    private void Update()
    {
        float vertMove = Input.GetAxisRaw("Vertical");
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }   
    }

    private void FlipPlayer(float horzMove)
    {
        // Face RIGHT
        if (horzMove > 0 && !isFacingRight)
        {
            isFacingRight = !isFacingRight;
            sr.flipX = !sr.flipX;
        }
        // Face LEFT
        if (horzMove < 0 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            sr.flipX = !sr.flipX;
        }
    }

    public bool IsOnGround()
    {
        bool groundCheck = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y - wHeight),
            -Vector2.up,
            rayCastLength);

        // onGround = groundCheck;
        if (groundCheck) return true;
        else return false;
    }

    public bool IsOnWall()
    {
        bool wallCheck1 = Physics2D.Raycast(
            new Vector2(transform.position.x - wWidth, transform.position.y),
            Vector2.left,
            rayCastLength);
        bool wallCheck2 = Physics2D.Raycast(
            new Vector2(transform.position.x + wWidth, transform.position.y),
            -Vector2.left,
            rayCastLength);

        // onWall = wallCheck1 || wallCheck2;
        if (wallCheck1 || wallCheck2) return true;
        else return false;
    }

    public int GetWallDirection()
    {
        if (Physics2D.Raycast(
            new Vector2(transform.position.x - wWidth, transform.position.y),
            Vector2.left,
            rayCastLength))
        {
            return -1;
        } else if (Physics2D.Raycast(
            new Vector2(transform.position.x + wWidth, transform.position.y),
            -Vector2.left,
            rayCastLength))
        {
            return 1;
        } else
        {
            return 0;
        }
    }

    public void SetWallJumpingToFalse() { isWallJumping = false; }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y - wHeight, 0), 0.1f);
        Gizmos.DrawWireSphere(new Vector3(transform.position.x + wWidth, transform.position.y, 0), 0.1f);
        Gizmos.DrawWireSphere(new Vector3(transform.position.x - wWidth, transform.position.y, 0), 0.1f);
        // Gizmos.DrawWireSphere((Vector2)transform.position - wWidth, 0.25f);
    }

    public void GetHit(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
