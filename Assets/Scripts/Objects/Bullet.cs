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
    [SerializeField] private float missTime;
    [SerializeField] private float missDistance;
    [SerializeField] private float onReleaseTimeWhenHit = 0.1f;
    [SerializeField] private int trailSubs = 4;
    [SerializeField] private List<Vector2> allHitPos = new List<Vector2>();

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
        _trail.emitting = true;
        transform.position = startPosition;
        _afterImageEffect.gameObject.SetActive(true);
        _afterImageEffect.StartEffect();
        _shootDelayOnResume = delayOnResume;

        _rb.AddForce(dir * speed, ForceMode2D.Impulse);
        
        LevelManager.Instance.AddActiveObject(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var position = transform.position;
        var point = collision.ClosestPoint(position);
        BulletHit(collision, ((Vector2)position - point).normalized);
    }

    private void ShowTrail()
    {
        _rb.isKinematic = true;
        _trail.enabled = true;
        _afterImageEffect.gameObject.SetActive(false);
        SoundManager.instance.PlaySoundEffect("Bullet", "FlyBy");

        TurnIntoRaycast(_rb.velocity.normalized);
    }

    private void TurnIntoRaycast(Vector3 dir)
    {
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

        var missPos = transform.position + dir * missDistance;
        Invoke(nameof(BulletRelease), missTime);
        StartCoroutine(PlaceBulletAlongTrail(transform.position, missPos, null));
    }

    private IEnumerator PlaceBulletAlongTrail(Vector3 startPos, Vector3 targetPos, RaycastHit2D[] hits)
    {
        int hitsLength = hits != null ? hits.Length : 0;
        int length = hitsLength > trailSubs ? hits.Length : trailSubs;
        int hitPositionsUsed = 0;

        for (int i = 0; i < length; i++)
        {
            Vector2 hitPos = hits != null ? hits[hitPositionsUsed].point : Vector2.zero;
            Vector2 lerpPos = hitsLength < trailSubs ? Vector2.Lerp(startPos, targetPos, 1 / (float)length * i) : Vector2.zero;

            float hitDist = Vector2.Distance(transform.position, hitPos);
            float lerpDist = hitsLength < trailSubs ?  Vector2.Distance(transform.position, lerpPos) : Mathf.Infinity;
            
            if (hits != null && (length == hits.Length || hitDist <= lerpDist))
            {
                BulletHit(hits[hitPositionsUsed].collider, hits[hitPositionsUsed].normal);
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
                BulletHit(hits[i].collider, hits[i].normal);
            }
        }

    }

    private void BulletHit(Collider2D collider, Vector2 normal)
    {
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
            case "Ricochet":
                //TODO Ricochet sound effect
                SoundManager.instance.PlaySoundEffect("Bullet", "Ricochet");
                var ricochetDir = Vector2.Reflect(_rb.velocity.normalized, normal);
                _rb.velocity = ricochetDir * _rb.velocity.magnitude;
                if (TimeManager.instance.TimeScale >= 1)
                {
                    TurnIntoRaycast(ricochetDir);
                }
                //Play soundeffect or something
                break;
            default:
                SoundManager.instance.PlaySoundEffect("Bullet", "Impact");
                _trail.emitting = false;
                if (TimeManager.instance.TimeScale >= 1)
                    Invoke(nameof(BulletRelease), onReleaseTimeWhenHit);
                else
                    BulletRelease();

                break;
        }

        FreezeFramer.instance.FreezeFrame();
    }

    private void BulletRelease()
    {
        LevelManager.Instance.RemoveActiveObject(gameObject);
        OnRelease(this);
    }
}
