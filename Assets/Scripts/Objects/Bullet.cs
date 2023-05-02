using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private LineRenderer line;
    private AfterImageEffect afterImageEffect;
    public delegate void Release(Bullet bullet);
    public Release OnRelease;
    private float shootDelayOnResume;
    [SerializeField] private LayerMask raycastLayerMask;
    [SerializeField] private float missDistance;
    [SerializeField] private float lineDissapearTime = 0.1f;

    public void Setup()
    {
        line = GetComponentInChildren<LineRenderer>(true);
        line.gameObject.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
        afterImageEffect = GetComponentInChildren<AfterImageEffect>();
        afterImageEffect.Setup();

        TimeManager.instance.OnTimeResume += () => Invoke(nameof(TurnIntoRaycast), shootDelayOnResume);
    }

    public void Shoot(Vector2 dir, float speed, Vector2 startPosition, float delayOnResume)
    {
        transform.position = startPosition;
        afterImageEffect.gameObject.SetActive(true);
        afterImageEffect.StartEffect();
        shootDelayOnResume = delayOnResume;

        rb.AddForce(dir * speed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Ignore player and platform collision
        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 7)
            return;

        BulletHit(collision);
    }

    private void TurnIntoRaycast()
    {
        Vector3 dir = rb.velocity.normalized;
        rb.isKinematic = true;
        afterImageEffect.gameObject.SetActive(false);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 999, raycastLayerMask);
        line.transform.SetParent(null, true);
        line.gameObject.SetActive(true);

        if (hit)
        {
            print(hit.collider);
            Vector3[] positions = { transform.position, hit.point };
            line.SetPositions(positions);

            BulletHit(hit.collider);

            Invoke(nameof(DisableBulletLine), lineDissapearTime);
            return;
        }
        
        Vector3 missPos = transform.position + (dir * missDistance);

        Vector3[] misPositions = { transform.position, missPos };

        line.SetPositions(misPositions);

        Invoke(nameof(DisableBulletLine), lineDissapearTime);
    }

    private void BulletHit(Collider2D collider)
    {
        GameObject hitObj = collider.gameObject;

        if (collider.attachedRigidbody != null)
            hitObj = collider.attachedRigidbody.gameObject;

        switch (hitObj.tag)
        {
            case "Enemy":
                print("Hit Enemy");
                break;
        }

        OnRelease(this);
    }

    private void DisableBulletLine()
    {
        line.gameObject.SetActive(false);
    }
}
