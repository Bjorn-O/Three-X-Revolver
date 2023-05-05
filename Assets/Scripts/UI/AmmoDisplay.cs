using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Animator animator;
    
    private int _currentAmmo = 3;
    private static readonly int TurnCylinder = Animator.StringToHash("TurnCylinder");
    private static readonly int CylinderAppear = Animator.StringToHash("CylinderAppear");
    private static readonly int CylinderDisappear = Animator.StringToHash("CylinderDisappear");

    private void Awake()
    {
        ammoText = GetComponentInChildren<TextMeshProUGUI>();
        _currentAmmo = 3;
        ammoText.text = _currentAmmo.ToString();
    }

    public void AmmoCountDown()
    {
        animator.SetTrigger(TurnCylinder);
        ammoText.enabled = false;
        if (_currentAmmo == 0) return;
        _currentAmmo -= 1;
        ammoText.text = _currentAmmo.ToString();
    }

    public void ShowCylinder()
    {
        animator.SetTrigger(CylinderAppear);
    }

    public void HideCylinder()
    {
        animator.SetTrigger(CylinderDisappear);
    }

    public void AmmoReveal()
    {
        ammoText.enabled = true;
    }
}
