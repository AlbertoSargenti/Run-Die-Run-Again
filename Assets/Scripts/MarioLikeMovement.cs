using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioLikeMovement : MonoBehaviour
{
    [SerializeField]
    public bool isMovementEnabled = true;
    [SerializeField]
    public bool isMovingRight = true; // Controls the right or left movement
    [SerializeField]
    public bool isPlayerControlEnabled = true; // Controls whether the player can control the character
    [SerializeField]
    public bool isRunningEnabled = true; // Controls whether the character can run

    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 10f;
    public float maxJumpTime = 0.5f; // Maximum time the jump button can be held
    public float groundDamping = 20f; // Ground damping to simulate inertia
    public float airDamping = 5f; // Air damping to simulate inertia
    public float ceilingCheckRadius = 0.2f; // Radius to check for ceiling
    public Transform[] groundChecks; // Array of ground check transforms
    public Transform ceilingCheck; // Transform for the ceiling check
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isCeilingHit;
    private bool isJumping;
    private float jumpTimeCounter;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpTimeCounter = maxJumpTime;
    }

    private void Update()
    {
        if (!isMovementEnabled)
        {
            return; // Exit the function if movement is disabled
        }

        // Check if at least one of the ground checks detects ground
        isGrounded = IsAnyGrounded();

        // Check if the ceiling check detects a ceiling hit
        isCeilingHit = IsCeilingHit();

        // Character movement
        float horizontalInput = isPlayerControlEnabled ? Input.GetAxis("Horizontal") : (isMovingRight ? 1f : 0f);
        bool isRunning = isRunningEnabled && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        float speed = isRunning ? runSpeed : walkSpeed;

        // Calculate damping based on whether the character is grounded or in the air
        float damping = isGrounded ? groundDamping : airDamping;

        // Smoothly accelerate or decelerate the character
        rb.velocity = new Vector2(
            Mathf.Lerp(rb.velocity.x, horizontalInput * speed, Time.deltaTime * damping),
            rb.velocity.y
        );

        // Jumping
        if (isGrounded)
        {
            jumpTimeCounter = maxJumpTime;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetButtonUp("Jump") && isJumping)
        {
            isJumping = false;
        }
    }

    private bool IsAnyGrounded()
    {
        foreach (Transform groundCheck in groundChecks)
        {
            float raycastLength = 0.2f;
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, raycastLength, groundLayer);

            if (hit.collider != null && !hit.collider.isTrigger)
            {
                return true; // At least one ground check detected ground
            }
        }

        return false; // None of the ground checks detected ground
    }

    private bool IsCeilingHit()
    {
        Collider2D hit = Physics2D.OverlapCircle(ceilingCheck.position, ceilingCheckRadius, groundLayer);

        return hit != null && !hit.isTrigger;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isJumping && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isJumping = false;

            // Apply a slight downward force to simulate stopping the jump
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.down * 2f, ForceMode2D.Impulse);
        }
    }

    // You can use this method to toggle player control from other parts of your code
    public void TogglePlayerControl(bool enablePlayerControl)
    {
        isPlayerControlEnabled = enablePlayerControl;
    }

    // You can use this method to toggle running control from other parts of your code
    public void ToggleRunningControl(bool enableRunningControl)
    {
        isRunningEnabled = enableRunningControl;
    }
}
