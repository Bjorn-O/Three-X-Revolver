using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D _rb;
    private TrailRenderer _trail;
    private AfterImageEffect _afterImageEffect;
    public delegate void Release(Bullet bullet);
    public Release OnRelease;
    private float _shootDelayOnResume;
    [SerializeField] private LayerMask raycastLayerMask;
    [SerializeField] private float missDistance;
    [SerializeField] private float onReleaseTimeWhenHit = 0.1f;
    [SerializeField] private float trailSubs = 4;

    private bool _bulletAlreadyHit;

    public void Setup()
    {
        _trail = GetComponent<TrailRenderer>();
        _trail.enabled = false;
        _rb = GetComponent<Rigidbody2D>();
        _afterImageEffect = GetComponentInChildren<AfterImageEffect>();
        _afterImageEffect.Setup();

        TimeManager.instance.OnTimeResume += () => Invoke(nameof(ShowTrail), _shootDelayOnResume);
    }

    public void Shoot(Vector2 dir, float speed, Vector2 startPosition, float delayOnResume)
    {
        transform.position = startPosition;
        _afterImageEffect.gameObject.SetActive(true);
        _afterImageEffect.StartEffect();
        _shootDelayOnResume = delayOnResume;

        _rb.AddForce(dir * speed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BulletHit(collision);
    }

    private void ShowTrail()
    {
        if (_bulletAlreadyHit)
            return;

        _rb.isKinematic = true;
        _trail.enabled = true;
        _afterImageEffect.gameObject.SetActive(false);
        SoundManager.instance.PlaySoundEffect("Bullet", "FlyBy");

        TurnIntoRaycast();
    }

    private void TurnIntoRaycast()
    {
        Vector3 dir = _rb.velocity.normalized;

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
        _trail.emitting = false;
        _bulletAlreadyHit = true;
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
