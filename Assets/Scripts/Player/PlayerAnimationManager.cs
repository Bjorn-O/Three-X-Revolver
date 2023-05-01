using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void SetAnimFloat(string parameter, float value)
    {
        animator.SetFloat(parameter, value);
    }
}
