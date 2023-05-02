using UnityEngine;

public class SpriteScaler : MonoBehaviour
{
    [SerializeField] private Transform spriteTransform;

    public void FlipSprite(bool facingRight)
    {
        var tempScale = spriteTransform.localScale; 
        
        if (facingRight)
        {
            tempScale.x = Mathf.Abs(tempScale.x);
        }
        else
        {
            tempScale.x = Mathf.Abs(tempScale.x) * -1;
        }
        spriteTransform.localScale = tempScale;
    }
}
