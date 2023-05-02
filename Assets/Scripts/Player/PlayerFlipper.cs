using UnityEngine;
using UnityEngine.Events;

public class PlayerFlipper : MonoBehaviour
{
    public UnityEvent<int> OnMovingBackwards;
    [SerializeField] private int tolerance;
    private bool _isLookingRight;
    
    public void LookSideCheck(Vector3 lookPos)
    {
        var playerTrans = transform;
        var targetScale = playerTrans.localScale;
        
        if (playerTrans.position.x - lookPos.x < -tolerance)
        {
            targetScale.x = Mathf.Abs(targetScale.x);
            _isLookingRight = true;
        }
        else
        {
            targetScale.x = Mathf.Abs(targetScale.x) * -1;
            _isLookingRight = false;
        }

        playerTrans.localScale = targetScale;
    }

    public void RunDirection(bool movingRight)
    {
        if (_isLookingRight && !movingRight || !_isLookingRight && movingRight)
        {
            OnMovingBackwards.Invoke(1);
            print("Running backwards");
            return;
        }
        print("Running forwards");
        OnMovingBackwards.Invoke(0);
    }
}
