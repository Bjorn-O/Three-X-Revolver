using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExitGame : MonoBehaviour
{
    [SerializeField] private float exitTimer;
    [SerializeField] private Animator animator;
    
    private bool _tryToEscape = false;
    private static readonly int ShowExitGame = Animator.StringToHash("ShowExitGame");
    private static readonly int HideExitGame = Animator.StringToHash("HideExitGame");


    private void Update()
    {
        var keyboard = Keyboard.current;
        var gamepad = Gamepad.current;

        if (keyboard != null && (keyboard.escapeKey.wasPressedThisFrame || gamepad != null && gamepad.selectButton.wasPressedThisFrame))
        {
            if (_tryToEscape)
            {
                Application.Quit();
            }
            else
            {
                _tryToEscape = true;
                animator.SetTrigger(ShowExitGame);
                //Display UI Element
            }
        }

        if (!_tryToEscape || !(exitTimer > 0)) return;
        exitTimer -= Time.deltaTime;

        if (!(exitTimer <= 0)) return;
        _tryToEscape = false;
        exitTimer = 0; 
        animator.SetTrigger(HideExitGame);
    }
}
