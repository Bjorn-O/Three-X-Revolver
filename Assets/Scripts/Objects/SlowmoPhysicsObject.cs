using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowmoPhysicsObject : MonoBehaviour
{
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector2 appliedForce = rb.velocity * (1 - TimeManager.instance.TimeScale);
        rb.AddForce(-appliedForce, ForceMode2D.Impulse);
    }
}
