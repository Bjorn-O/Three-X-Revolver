using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float acceleration = 7;
    [SerializeField] private float decceleration = 7;
    [SerializeField] private float velPower = 0.9f;
    [Space]
    [SerializeField] private float frictionAmount = 0.2f;
    private float moveX;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 15;
    [SerializeField] private float fallGravityMultiplier = 1.9f;
    [Range(0, 1)] [SerializeField] private float jumpCutMultiplier = 0.1f;
    private float gravityScale;

    [Header("Checks")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded = false;
    private bool isJumping = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravityScale = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer) && !isJumping)
        {
            if (!isGrounded)
                isJumping = false;

            isGrounded = true;
        }
        else
            isGrounded = false;
    }

    private void FixedUpdate()
    {
        Move();
        AddFriction();
        JumpGravity();
    }

    private void OnMove(InputValue inputValue)
    {
        moveX = inputValue.Get<float>();
    }

    private void OnJump()
    {
        if (!isGrounded)
        {
            return;
        }

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isJumping = true;
    }

    private void OnJumpReleased()
    {
        if (rb.velocity.y > 0)
        {
            rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
        }
    }

    private void Move()
    {
        float targetSpeed = moveX * moveSpeed;
        float speedDiff = targetSpeed - rb.velocity.x;
        float accelrate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelrate, velPower) * Mathf.Sign(speedDiff);

        rb.AddForce(movement * Vector2.right);
    }

    private void AddFriction()
    {
        //Don't add friction when trying to move
        if (Mathf.Abs(moveX) > 0)
        {
            return;
        }

        float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmount));
        amount *= Mathf.Sign(rb.velocity.x);

        rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
    }

    private void JumpGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale * fallGravityMultiplier;

            if (isJumping)
                isJumping = false;
        }
        else
        {
            if (rb.velocity.y == 0)
                isJumping = false;

            rb.gravityScale = gravityScale;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
    }
}
