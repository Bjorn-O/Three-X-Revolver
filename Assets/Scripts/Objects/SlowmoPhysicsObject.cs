using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowmoPhysicsObject : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float checkOffsetY = 0.5f;
    [SerializeField] private float reverseGravity = -2;
    [SerializeField] private float timeStopGravityScale = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        TimeManager.instance.OnTimeStop += () => { rb.velocity = Vector2.zero; rb.gravityScale = timeStopGravityScale; };
        TimeManager.instance.OnTimeResume += () => { rb.gravityScale = 1; };
    }

    private void FixedUpdate()
    {
        if (TimeManager.instance.TimeScale >= 1)
            return;

        Vector2 appliedForce = rb.velocity * (1 - TimeManager.instance.TimeScale);
        rb.AddForce(-appliedForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (TimeManager.instance.TimeScale >= 1)
            return;

        //Stop physics if hitting the ground
        if (collision.gameObject.layer == 3 && rb.velocity.y < 0)
            rb.isKinematic = true;

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
