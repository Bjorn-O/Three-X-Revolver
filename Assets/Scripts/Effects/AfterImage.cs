using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AfterImage : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public delegate void Release(AfterImage afterImage);
    public Release OnRelease;
    private float startOpacity;

    public void Setup(string sortingLayerName, int orderInLayer)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = sortingLayerName;
        spriteRenderer.sortingOrder = orderInLayer;

        startOpacity = spriteRenderer.color.a;
        TimeManager.instance.OnTimeResume += () => OnRelease(this);
    }

    public void Show(Vector3 scale, Sprite sprite, float timeToDissapear, Color color)
    {
        transform.localScale = scale;
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = new Color(color.r, color.g, color.b, startOpacity);

        StartCoroutine(WaitToRelease(timeToDissapear));
        StartCoroutine(FadeOut(timeToDissapear));
    }

    private IEnumerator WaitToRelease(float timeToDissapear)
    {
        yield return new WaitForSeconds(timeToDissapear);

        OnRelease(this);
    }

    private IEnumerator FadeOut(float timeToDissapear)
    {
        float currentTime = 0;
        Color startColor = spriteRenderer.color;
        Color endColor = startColor;
        endColor.a = 0;

        while (currentTime < timeToDissapear)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / timeToDissapear;
            Color currentColor = Color.Lerp(startColor, endColor, t);
            spriteRenderer.color = currentColor;
            yield return null;
        }

    }
}
