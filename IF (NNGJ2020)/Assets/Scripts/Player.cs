using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public GameObject playerObject;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator an;

    private Camera cam;

    private ParticleSystem ps;

    private Texture2D mColorSwapTex;

    private Color[] mSpriteColors;

    private Vector2 lastVelocity;

    public float speed = 5f;
    public bool isFacingRight = true;
    private float wHeight;
    private float wWidth;
    private float health = 100f;

    [Header ("Jumping")]
    public float jumpSpeed = 7f;
    public bool isJumping = false;
    public float wallJumpY = 10f;
    private float jumpButtonPressTime = 0f;
    private float maxJumpTime = 0.2f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private float rayCastLength = 0.005f;

    [Header ("Collision")]
    public bool onGround = false;
    public LayerMask groundLayer;
    public Vector2 direction;

    [Header ("Dashing")]
    // public float dashSpeed;
    public float dashTime = 0.25f;
    // public float startDashTime;
    public bool isDashing = false;
    public float dashForce = 8f;
    public float startDashTimer = 0.25f;
    float currentDashTimer;
    int dashDir;
    [Header ("check point")]
    public Transform lastCheckPoint;

    void Start () {
        health = 100f;

        currentDashTimer = startDashTimer;
    }

    private void Awake () {
        sr = GetComponent<SpriteRenderer> ();
        rb = GetComponent<Rigidbody2D> ();
        an = GetComponent<Animator> ();
        ps = GetComponent<ParticleSystem> ();

        cam = Camera.main;

        // wHeight: the bottom part of the Player
        wHeight = GetComponent<Collider2D> ().bounds.extents.y + 0.1f;
        // wWidth: the right part of the Player
        wWidth = GetComponent<Collider2D> ().bounds.extents.x + 0.1f;

        sr.material.SetColor ("_RepCol", new Color (1.0f, 0.0f, 0.0f, 0.5f));
    }

    void FixedUpdate () {
        /*** HORIZONTAL MOVEMENT ***/
        // Get Horizontal Movement (Walking)
        float horzMove = Input.GetAxisRaw ("Horizontal");

        // Move Player according to direction
        Vector2 vect = rb.velocity;
        rb.velocity = new Vector2 (horzMove * speed, vect.y);

        // Check movement direction and make player face towards direction
        FlipPlayer (-horzMove);

        /*** VERTICAL MOVEMENT ***/
        // Get Vertical Movement (Jumping)
        float vertMove = Input.GetAxisRaw ("Vertical");
        if (IsOnGround () && isJumping == false) // && (Input.GetButtonDown("Jump") || vertMove > 0))
        {
            if (Input.GetButtonDown ("Jump") || vertMove > 0) {
                isJumping = true;
                an.SetBool ("IsIdle", false);
                an.SetBool ("IsJumping", true);
                an.Play ("JumpUBegin");
                Jump ();

            } else {
                isJumping = false;
                an.SetBool ("IsIdle", true);
                an.SetBool ("IsJumping", false);
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
        if (jumpButtonPressTime > maxJumpTime) {
            vertMove = 0f;
            isJumping = false;
            an.SetBool ("IsJumping", false);
        }

        // Check if Player is still busy jumping
        if (vertMove >= 1f) {
            jumpButtonPressTime += Time.deltaTime;
        } else {
            isJumping = false;
            jumpButtonPressTime = 0f;

            an.SetBool ("IsJumping", false);
        }

        if (!onGround && vertMove == 0) {
            an.SetBool ("IsIdle", false);
        } else {
            an.SetBool ("IsIdle", true);
        }

        /* WALL JUMP */
        if (IsOnWall () && !IsOnGround () && horzMove == 1) {
            rb.velocity = new Vector2 (-GetWallDirection () * speed * -.75f, wallJumpY);
        }

        /* DASHING */
        if (Input.GetButtonDown ("Dash") && direction.x != 0) {
            isDashing = true;
            currentDashTimer = startDashTimer;
            dashDir = (int) horzMove;
            rb.velocity = Vector2.zero;
        }

        if (isDashing) {
            rb.velocity = transform.right * dashDir * dashForce;
            currentDashTimer -= Time.deltaTime;

            if (currentDashTimer <= 0) {
                isDashing = false;
            }
        }

        lastVelocity = rb.velocity;
        an.SetFloat ("horzMove", horzMove);
        an.SetFloat ("vertMove", vertMove);
    }

    private void Update () {
        onGround = Physics2D.Raycast (
            new Vector2 (transform.position.x, transform.position.y - wHeight),
            Vector2.down, rayCastLength, groundLayer);

        direction = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

        if (!onGround) {
            if (rb.velocity.y < 0) {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            } else if (rb.velocity.y > 0 && !Input.GetButton ("Jump")) {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }

        /*
        else if (rb.velocity.y > 0 && Input.GetButton("Jump") && !onGround)
        {
            rb.velocity = Vector2.zero;
        }
       */

        an.SetBool ("IsJumping", isJumping);
    }

    private void Jump () {
        isJumping = true;
        an.SetBool ("IsJumping", true);
        an.Play ("JumpUBegin");
        rb.velocity = new Vector2 (rb.velocity.x, 0);
        rb.AddForce (Vector2.up * jumpSpeed, ForceMode2D.Impulse);

        isJumping = false;
    }
    private void FlipPlayer (float horzMove) {
        // Face RIGHT
        if (horzMove > 0 && !isFacingRight) {
            isFacingRight = !isFacingRight;
            sr.flipX = !sr.flipX;
        }
        // Face LEFT
        if (horzMove < 0 && isFacingRight) {
            isFacingRight = !isFacingRight;
            sr.flipX = !sr.flipX;
        }
    }

    public bool IsOnGround () {
        bool groundCheck = Physics2D.Raycast (
            new Vector2 (transform.position.x, transform.position.y - wHeight), -Vector2.up,
            rayCastLength, groundLayer);

        if (groundCheck) return true;
        else return false;
    }
    public bool IsOnWall () {
        bool wallCheck1 = Physics2D.Raycast (
            new Vector2 (transform.position.x - wWidth, transform.position.y),
            Vector2.left,
            rayCastLength, groundLayer);
        bool wallCheck2 = Physics2D.Raycast (
            new Vector2 (transform.position.x + wWidth, transform.position.y),
            Vector2.right,
            rayCastLength, groundLayer);

        // onWall = wallCheck1 || wallCheck2;
        if (wallCheck1 || wallCheck2) return true;
        else return false;
    }

    public int GetWallDirection () {
        if (Physics2D.Raycast (new Vector2 (transform.position.x - wWidth,
                    transform.position.y),
                Vector2.left,
                rayCastLength, groundLayer)) {
            return -1;
        } else if (Physics2D.Raycast (new Vector2 (transform.position.x + wWidth,
                    transform.position.y),
                Vector2.right,
                rayCastLength, groundLayer)) {
            return 1;
        } else {
            return 0;
        }
    }

    public void GetHit (float damage) {
        health -= damage;
        ShakeBehaviour shk;
        if (cam.TryGetComponent<ShakeBehaviour> (out shk)) {
            shk.TriggerShake ();
        }

        if (health <= 0f) {

            PlayerDies ();
            //Destroy(gameObject);
        }
    }

    public void PlayerDies () {

        var nv = lastVelocity.normalized * 60;

        var shp = ps.shape;

        if (nv != Vector2.zero) {
            float angle = Mathf.Atan2 (nv.x, nv.y) * Mathf.Rad2Deg;
            shp.rotation = new Vector3 (angle - 90f, 90.0f, 0.0f);
        } else {
            shp.rotation = new Vector3 (-90f, -90f, 0f);
        }

        ps.Emit (80);
        //Attempts to load scene at 0 or reset position 
        playerObject.transform.position = lastCheckPoint.transform.position;

    }

    void OnCollisionEnter2D (Collision2D collision) {
        //    Debug.Log("Collided with " + collision.gameObject.name);
        if (collision.gameObject.CompareTag ("Trap")) {
            SceneManager.LoadScene (0);
            GetHit (1000f);
        }
    }

    private void OnDrawGizmos () {
        Gizmos.color = Color.red;
        Gizmos.DrawLine (
            new Vector2 (transform.position.x, transform.position.y - wHeight),
            new Vector2 (transform.position.x, transform.position.y - wHeight - rayCastLength));
    }
}