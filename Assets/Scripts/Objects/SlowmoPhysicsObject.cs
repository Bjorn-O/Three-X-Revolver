using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowmoPhysicsObject : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float checkOffsetY = 0.5f;
    [SerializeField] private float reverseGravity = -3.5f;
    [SerializeField] private float timeStopGravityScale = 0.1f;
    [SerializeField] private LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        print(layerMask.value);
        rb = GetComponent<Rigidbody2D>();
        TimeManager.instance.OnTimeStop += () => { rb.velocity = Vector2.zero; rb.gravityScale = timeStopGravityScale; };
        TimeManager.instance.OnTimeResume += () => { rb.gravityScale = 1; };

        LevelManager.Instance.AddActiveObject(gameObject);
    }

    private void FixedUpdate()
    {
        if (TimeManager.instance.TimeScale >= 1)
            return;

        if (rb.velocity.y >= 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        Vector2 appliedForce = rb.velocity * (1 - TimeManager.instance.TimeScale);
        rb.AddForce(-appliedForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Stop physics if hitting the ground in time stop
        if (((1<<collision.gameObject.layer) & layerMask) != 0 && rb.velocity.y <= 0)
        {
            LevelManager.Instance.RemoveActiveObject(gameObject);
            rb.isKinematic = TimeManager.instance.TimeScale < 1;
        }

        if (TimeManager.instance.TimeScale >= 1)
            return;

        if (collision.rigidbody == null || !collision.rigidbody.CompareTag("Player"))
            return;

        Rigidbody2D playerRb = collision.rigidbody;

        if (playerRb.transform.position.y > transform.position.y + checkOffsetY)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = reverseGravity * playerRb.mass;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (TimeManager.instance.TimeScale >= 1)
            return;

        if (collision.rigidbody == null || !collision.rigidbody.CompareTag("Player"))
            return;

        rb.gravityScale = timeStopGravityScale;
    }
}
