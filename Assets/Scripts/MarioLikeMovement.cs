using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioLikeMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 10f;
    public float maxJumpTime = 0.5f; // Maximum time the jump button can be held
    public float groundDamping = 20f; // Ground damping to simulate inertia
    public float airDamping = 5f; // Air damping to simulate inertia
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isJumping;
    private float jumpTimeCounter;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpTimeCounter = maxJumpTime;
    }

    private void Update()
    {
        // Check if the character is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // Character movement
        float horizontalInput = Input.GetAxis("Horizontal");
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
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

        if (Input.GetButtonDown("Jump") && isGrounded && !isJumping)
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

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }
    }
}
