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
    [SerializeField] private int trailSubs = 4;
    [SerializeField] private List<Vector2> allHitPos = new List<Vector2>();

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
        _bulletAlreadyHit = false;
        _trail.emitting = true;
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

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dir, 999, raycastLayerMask);

        if (hits.Length > 0)
        {
            List<RaycastHit2D> hitsUntilSolid = new List<RaycastHit2D>();

            foreach (RaycastHit2D hit in hits)
            {
                hitsUntilSolid.Add(hit);

                GameObject hitObj = hit.collider.gameObject;
                if (hit.collider.attachedRigidbody != null)
                    hitObj = hit.collider.attachedRigidbody.gameObject;

                if (!hitObj.CompareTag("Enemy"))
                    break;
            }

            RaycastHit2D[] hitsUntilSolidArray = hitsUntilSolid.ToArray();

            StartCoroutine(PlaceBulletAlongTrail(transform.position, hitsUntilSolidArray[hitsUntilSolidArray.Length - 1].point, hitsUntilSolidArray));
            return;
        }

        Vector3 missPos = dir * missDistance;
        StartCoroutine(PlaceBulletAlongTrail(transform.position, missPos, null));
    }

    private IEnumerator PlaceBulletAlongTrail(Vector3 startPos, Vector3 targetPos, RaycastHit2D[] hits)
    {
        int length = hits.Length > trailSubs ? hits.Length : trailSubs;
        int hitPositionsUsed = 0;

        for (int i = 0; i < length; i++)
        {
            Vector2 hitPos = hits != null ? hits[hitPositionsUsed].point : Vector2.zero;
            Vector2 lerpPos = hits.Length < trailSubs ? Vector2.Lerp(startPos, targetPos, 1 / (float)length * i) : Vector2.zero;

            float hitDist = Vector2.Distance(transform.position, hitPos);
            float lerpDist = hits.Length < trailSubs ?  Vector2.Distance(transform.position, lerpPos) : Mathf.Infinity;

            //Debug.Log(hitPos + " dist: " + hitDist + " lerpPos: " + lerpPos + " lerpDist: "+ lerpDist + " transPos: " + transform.position +  " start: " + startPos +" target: " + targetPos);

            if (hits != null && (length == hits.Length || hitDist <= lerpDist))
            {
                BulletHit(hits[hitPositionsUsed].collider);
                hitPositionsUsed++;
                transform.position = hitPos;
            }
            else
                transform.position = lerpPos;

            allHitPos.Add(transform.position);
            yield return null;
        }

        transform.position = targetPos;
        yield return null;

        if (hits != null)
        {
            for (int i = hitPositionsUsed; i < hits.Length; i++)
            {
                BulletHit(hits[i].collider);
            }
        }

        _trail.emitting = false;

        Invoke(nameof(BulletRelease), onReleaseTimeWhenHit);
    }

    private void BulletHit(Collider2D collider)
    {
        _bulletAlreadyHit = true;
        GameObject hitObj = collider.gameObject;

        if (collider.attachedRigidbody != null)
            hitObj = collider.attachedRigidbody.gameObject;

        //Debug.Log("Bullet hit obj: " + hitObj + " and bullet delay is: " + shootDelayOnResume);

        switch (hitObj.tag)
        {
            case "Enemy":
                SoundManager.instance.PlaySoundEffect("Bullet", "HitEnemy");
                hitObj.GetComponent<Enemy>().Kill();
                break;
            default:
                SoundManager.instance.PlaySoundEffect("Bullet", "Impact");

                if (TimeManager.instance.TimeScale >= 1)
                    Invoke(nameof(BulletRelease), onReleaseTimeWhenHit);
                else
                    OnRelease(this);
                break;
        }

        FreezeFramer.instance.FreezeFrame();
    }

    private void BulletRelease()
    {
        OnRelease(this);
    }
}
