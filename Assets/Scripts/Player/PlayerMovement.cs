using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    
    public UnityEvent<float> onMoveEvent;
    public UnityEvent<bool> onJumpEvent;
    public UnityEvent<bool> onCrouchEvent;
    public UnityEvent<bool> onGroundEvent;
    [FormerlySerializedAs("onChangeDirection")] public UnityEvent<bool> onMoveDirection;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float acceleration = 7;
    [SerializeField] private float deceleration = 7;
    [SerializeField] private float velPower = 0.9f;
    [Space]
    [SerializeField] private float frictionAmount = 0.2f;
    private float _moveX;

    [Header("Jumping")]
    [SerializeField] private float jumpHeight = 4.5f;
    [SerializeField] private float jumpTimeToApex = 0.45f;
    [SerializeField] private float fallGravityMultiplier = 1.9f;
    [Range(0, 1)] [SerializeField] private float jumpCutMultiplier = 0.1f;
    private float _gravityScale;
    private float _gravityStrength;
    private float _jumpForce;

    [Header("Checks")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundLayer;
    private float _lastInput;
    private bool _isGrounded;
    private bool _isJumping;
    private bool _isCrouching;
    private bool _isMovingRight;

    public delegate void CrouchStarted();
    public CrouchStarted OnCrouchStarted;
    public delegate void CrouchReleased();
    public CrouchReleased OnCrouchReleased;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
        _gravityScale = _gravityStrength / Physics2D.gravity.y;

        _jumpForce = Mathf.Abs(_gravityStrength) * jumpTimeToApex;
    }

    private void Update()
    {
        GroundCheck();
        onMoveDirection?.Invoke(_isMovingRight);
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer) && !_isJumping)
        {
            _isGrounded = true;
            onGroundEvent?.Invoke(_isGrounded);
        }
        else
        {
            _isGrounded = false;
            onGroundEvent?.Invoke(_isGrounded);
        }
    }

    private void FixedUpdate()
    {
        Move();
        AddFriction();
        JumpGravity();
        
    }

    private void OnMove(InputValue inputValue)
    {
        _moveX = inputValue.Get<float>();

        switch (_moveX)
        {
            case > 0:
                _isMovingRight = true;
                break;
            case < 0:
                _isMovingRight = false;
                break;
            case 0:
                break;
        }
    }

    private void OnJump(InputValue inputValue)
    {
        //Cut jump short when releasing the jump button mid-jump
        if (!inputValue.isPressed)
        {
            if (_rb.velocity.y > 0)
            {
                _rb.AddForce(Vector2.down * _rb.velocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
            }
            return;
        }

        if (!_isGrounded) return;

        var force = _jumpForce;
        if (_rb.velocity.y < 0) force -= _rb.velocity.y;

        _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        _isJumping = true;
        onJumpEvent.Invoke(_isJumping);
    }

    private void OnCrouch(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            OnCrouchStarted();
            _isCrouching = true;
            onCrouchEvent?.Invoke(_isCrouching);
        }
        else
        {
            OnCrouchReleased();
            _isCrouching = false;
            onCrouchEvent?.Invoke(_isCrouching);
        }
    }

    private void Move()
    {
        if (_isCrouching && _isGrounded)
        {
            onMoveEvent?.Invoke(0);
            return;
        }

        var targetSpeed = _moveX * moveSpeed;
        var speedDiff = targetSpeed - _rb.velocity.x;
        var accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        var movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);

        _rb.AddForce(movement * Vector2.right);
        onMoveEvent.Invoke(Mathf.Abs(_rb.velocity.x));
    }

    private void AddFriction()
    {
        //Add friction when not moving or when crouching
        if (Mathf.Abs(_moveX) < 0.01f || _isCrouching)
        {
            float amount = Mathf.Min(Mathf.Abs(_rb.velocity.x), Mathf.Abs(frictionAmount));
            amount *= Mathf.Sign(_rb.velocity.x);

            _rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }        
    }

    private void JumpGravity()
    {
        if (_rb.velocity.y < 0)
        {
            _rb.gravityScale = _gravityScale * fallGravityMultiplier;

            if (!_isJumping) return;
            _isJumping = false;
            onJumpEvent?.Invoke(_isJumping);
        }
        else
        {
            if (_rb.velocity.y == 0)
            {
                _isJumping = false;
                onJumpEvent?.Invoke(_isJumping);
            }
            _rb.gravityScale = _gravityScale;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
    }
}
