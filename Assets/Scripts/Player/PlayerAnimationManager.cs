using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");

    public void SetAnimSpeed(float value)
    {
        animator.SetFloat(Speed, value);
    }

    public void SetAnimGrounded(bool value)
    {
        animator.SetBool(IsGrounded, value);
    }

    public void SetAnimJumping(bool value)
    {
        animator.SetBool(IsJumping, value);
    }
}
