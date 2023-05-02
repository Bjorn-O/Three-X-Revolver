using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AfterImageEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private ObjectPool<AfterImage> afterImagePool;
    [SerializeField] private AfterImage afterImagePrefab;
    [SerializeField] private float dissapearTime = 0.6f;
    [SerializeField] private float createDistance = 0.5f;
    [SerializeField] private int orderInLayer = -1;
    [SerializeField] private Color startColor = Color.white;
    private bool startCreating = false;
    private bool setupDone = false;
    private Vector2 prevAfterImagePos;

    // Start is called before the first frame update
    private void Start()
    {
        Setup();
    }

    public void Setup()
    {
        if (setupDone)
            return;

        setupDone = true;
        spriteRenderer = GetComponent<SpriteRenderer>();

        afterImagePool = new ObjectPool<AfterImage>(() => {
            var afterImage = Instantiate(afterImagePrefab);
            afterImage.Setup(spriteRenderer.sortingLayerName, orderInLayer);
            return afterImage;
        }, afterImage => {
            afterImage.gameObject.SetActive(true);
        }, afterImage => {
            afterImage.gameObject.SetActive(false);
        }, afterImage => {
            Destroy(afterImage.gameObject);
        }, false, 10, 20);


        TimeManager.instance.OnTimeStop += StartEffect;
        TimeManager.instance.OnTimeResume += StopEffect;
    }

    private void Update()
    {
        if (!startCreating)
            return;

        if (Vector2.Distance(prevAfterImagePos, transform.position) > createDistance)
        {
            CreateAfterImage();
        }
    }

    public void StartEffect()
    {
        CreateAfterImage();
        startCreating = true;
    }

    public void StopEffect()
    {
        startCreating = false;
    }

    private void CreateAfterImage()
    {
        var afterImage = afterImagePool.Get();

        var targetTransform = transform;
        var afterImageTransform = afterImage.transform;
        
        afterImageTransform.position = targetTransform.position;
        afterImageTransform.rotation = targetTransform.rotation;
        prevAfterImagePos = targetTransform.position;
        afterImage.Show(targetTransform.lossyScale, spriteRenderer.sprite, dissapearTime, startColor);
        afterImage.OnRelease = ReleaseAfterImage;
    }

    private void ReleaseAfterImage(AfterImage afterImage)
    {
        afterImagePool.Release(afterImage);
    }
}
