using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;

    public void Setup()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot(Vector2 dir, float speed, Vector2 startPosition)
    {
        transform.position = startPosition;

        rb.AddForce(dir * speed, ForceMode2D.Impulse);
    }
}
