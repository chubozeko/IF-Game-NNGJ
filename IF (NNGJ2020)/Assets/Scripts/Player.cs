using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator an;

    private Texture2D mColorSwapTex;

    private Color[] mSpriteColors;
    
    public float speed = 5f;
    public bool isFacingRight = true;
    private float wHeight;
    private float health = 100f;

    public float jumpSpeed = 7f;
    public bool isJumping = false;
    private float jumpButtonPressTime = 0f;
    private float maxJumpTime = 0.2f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private float rayCastLength = 0.005f;

    [Header("Collision")]
    public bool onGround = false;
    public LayerMask groundLayer;
    public Vector2 direction;

    void Start()
    {
        health = 100f;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        an = GetComponent<Animator>();

        // wHeight: the bottom part of the Player
        wHeight = GetComponent<Collider2D>().bounds.extents.y + 0.1f;

        sr.material.SetColor("_RepCol", new Color(1.0f, 0.0f, 0.0f, 0.5f));
    }


    void FixedUpdate()
    {
        /*** HORIZONTAL MOVEMENT ***/
        // Get Horizontal Movement (Walking)
        float horzMove = Input.GetAxisRaw("Horizontal");

        // Move Player according to direction
        Vector2 vect = rb.velocity;
        rb.velocity = new Vector2(horzMove * speed, vect.y);

        // Check movement direction and make player face towards direction
        FlipPlayer(-horzMove);

        /*** VERTICAL MOVEMENT ***/
        // Get Vertical Movement (Jumping)
        float vertMove = Input.GetAxisRaw("Vertical");
        if (IsOnGround() && isJumping == false && (Input.GetButtonDown("Jump") || vertMove > 0))
        {
            if (Input.GetButton("Jump") || vertMove > 0)
            {
                isJumping = true;
                an.SetBool("IsIdle", false);
                an.SetBool("IsJumping", true);
                an.Play("JumpUBegin");
                Jump();

            } else
            {
                isJumping = false;
                an.SetBool("IsIdle", true);
                an.SetBool("IsJumping", false);
            }
        }

        // Check if Player is jumping & jump time hasn't elapsed yet
        /*
        if (isJumping && jumpButtonPressTime < maxJumpTime)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
        */
        // Check if Jumping time period has elapsed
        if (jumpButtonPressTime > maxJumpTime)
        {
            vertMove = 0f;
            isJumping = false;
            an.SetBool("IsJumping", false);
        }

        // Check if Player is still busy jumping
        if (vertMove >= 1f)
        {
            jumpButtonPressTime += Time.deltaTime;
        }
        else
        {
            isJumping = false;
            jumpButtonPressTime = 0f;
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier / 2) * Time.deltaTime;
            an.SetBool("IsJumping", false);
        }

        if (!onGround && vertMove == 0)
        {
            an.SetBool("IsIdle", false);
        } else
        {
            an.SetBool("IsIdle", true);
        }

        an.SetFloat("horzMove", horzMove);
        an.SetFloat("vertMove", vertMove);
    }

    private void Update()
    {
        onGround = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y - wHeight),
            Vector2.down, 0.6f, groundLayer);

        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        an.SetBool("IsJumping", isJumping);
        
    }

    private void Jump()
    {
        isJumping = true;
        an.SetBool("IsJumping", true);
        an.Play("JumpUBegin");
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        isJumping = false;
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

        if (groundCheck) return true;
        else return false;
    }

    public void GetHit(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // wHeight = ;
        Gizmos.DrawLine(new Vector2(transform.position.x, GetComponent<Collider2D>().bounds.extents.y + 0.1f), transform.position + (Vector3.down * rayCastLength));
    }
}
