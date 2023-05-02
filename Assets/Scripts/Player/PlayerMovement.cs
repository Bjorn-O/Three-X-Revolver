using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public UnityEvent<string,float> onMoving;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float acceleration = 7;
    [SerializeField] private float decceleration = 7;
    [SerializeField] private float velPower = 0.9f;
    [Space]
    [SerializeField] private float frictionAmount = 0.2f;
    private float moveX;

    [Header("Jumping")]
    [SerializeField] private float jumpHeight = 4.5f;
    [SerializeField] private float jumpTimeToApex = 0.45f;
    [SerializeField] private float fallGravityMultiplier = 1.9f;
    [Range(0, 1)] [SerializeField] private float jumpCutMultiplier = 0.1f;
    private float gravityScale;
    private float gravityStrength;
    private float jumpForce;

    [Header("Checks")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded = false;
    private bool isJumping = false;
    private bool isCrouching = false;
    public delegate void CrouchStarted();
    public CrouchStarted OnCrouchStarted;
    public delegate void CrouchReleased();
    public CrouchReleased OnCrouchReleased;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
        gravityScale = gravityStrength / Physics2D.gravity.y;

        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
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

    private void OnJump(InputValue inputValue)
    {
        //Cut jump short when releasing the jump button mid-jump
        if (!inputValue.isPressed)
        {
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
            }
            return;
        }

        if (!isGrounded)
            return;

        float force = jumpForce;
        if (rb.velocity.y < 0)
            force -= rb.velocity.y;

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        isJumping = true;
    }

    private void OnCrouch(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            OnCrouchStarted();
            isCrouching = true;
        }
        else
        {
            OnCrouchReleased();
            isCrouching = false;
        }
    }

    private void Move()
    {
        if (isCrouching && isGrounded)
        {
            return;
        }

        float targetSpeed = moveX * moveSpeed;
        float speedDiff = targetSpeed - rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);

        rb.AddForce(movement * Vector2.right);
        onMoving.Invoke("Speed" ,rb.velocity.magnitude);
    }

    private void AddFriction()
    {
        //Add friction when not moving or when crouching
        if (Mathf.Abs(moveX) < 0.01f || isCrouching)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmount));
            amount *= Mathf.Sign(rb.velocity.x);

            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }        
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
