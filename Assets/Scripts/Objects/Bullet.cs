using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private TrailRenderer trail;
    private AfterImageEffect afterImageEffect;
    public delegate void Release(Bullet bullet);
    public Release OnRelease;
    private float shootDelayOnResume;
    [SerializeField] private LayerMask raycastLayerMask;
    [SerializeField] private float missDistance;
    [SerializeField] private float onReleaseTimeWhenHit = 0.1f;
    [SerializeField] private float trailSubs = 4;

    private bool bulletAlreadyHit;

    public void Setup()
    {
        trail = GetComponent<TrailRenderer>();
        trail.enabled = false;
        rb = GetComponent<Rigidbody2D>();
        afterImageEffect = GetComponentInChildren<AfterImageEffect>();
        afterImageEffect.Setup();

        TimeManager.instance.OnTimeResume += () => Invoke(nameof(ShowTrail), shootDelayOnResume);
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
        BulletHit(collision);
    }

    private void ShowTrail()
    {
        if (bulletAlreadyHit)
            return;

        rb.isKinematic = true;
        trail.enabled = true;
        afterImageEffect.gameObject.SetActive(false);
        SoundManager.instance.PlaySoundEffect("Bullet", "FlyBy");

        TurnIntoRaycast();
    }

    private void TurnIntoRaycast()
    {
        Vector3 dir = rb.velocity.normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 999, raycastLayerMask);

        if (hit)
        {
            print(hit.collider);

            StartCoroutine(PlaceBulletAlongTrail(transform.position, hit.point, hit.collider));
            return;
        }

        Vector3 missPos = dir * missDistance;
        StartCoroutine(PlaceBulletAlongTrail(transform.position, missPos, null));
    }

    private IEnumerator PlaceBulletAlongTrail(Vector3 startPos, Vector3 targetPos, Collider2D col)
    {
        for (int i = 0; i < trailSubs; i++)
        {
            transform.position = Vector2.Lerp(startPos, targetPos, 1 / trailSubs * i);
            yield return null;
        }

        transform.position = targetPos;
        yield return null;

        if (col != null)
        {
            BulletHit(col);
        }

        Invoke(nameof(BulletRelease), onReleaseTimeWhenHit);
    }

    private void BulletHit(Collider2D collider)
    {
        trail.emitting = false;
        bulletAlreadyHit = true;
        GameObject hitObj = collider.gameObject;

        if (collider.attachedRigidbody != null)
            hitObj = collider.attachedRigidbody.gameObject;

        switch (hitObj.tag)
        {
            case "Enemy":
                SoundManager.instance.PlaySoundEffect("Bullet", "HitEnemy");
                hitObj.GetComponent<Enemy>().Kill();
                break;
            default:
                SoundManager.instance.PlaySoundEffect("Bullet", "Impact");
                break;
        }

        FreezeFramer.instance.FreezeFrame();

        if (TimeManager.instance.TimeScale >= 1)
            Invoke(nameof(BulletRelease), onReleaseTimeWhenHit);
        else
            OnRelease(this);
    }

    private void BulletRelease()
    {
        OnRelease(this);
    }
}
