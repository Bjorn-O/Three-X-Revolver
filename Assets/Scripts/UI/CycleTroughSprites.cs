using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CycleTroughSprites : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float cycleDelay = 0.5f;
    [SerializeField] private float startDelay = 0;
    private int spriteIndex;

    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();

        StartCoroutine(Cycle());
    }

    private IEnumerator Cycle()
    {
        yield return new WaitForSecondsRealtime(startDelay);

        while (true)
        {
            image.sprite = sprites[spriteIndex];

            yield return new WaitForSecondsRealtime(cycleDelay);

            spriteIndex++;

            if (spriteIndex >= sprites.Length)
            {
                spriteIndex = 0;
            }
        }
    }
}
