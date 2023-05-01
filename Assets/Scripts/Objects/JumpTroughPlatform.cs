using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTroughPlatform : MonoBehaviour
{
    private Collider2D playerCol;
    private Collider2D platformCol;

    private bool playerIsCrouching = false;
    private bool ignoringPlayerCol = false;

    // Start is called before the first frame update
    void Start()
    {
        platformCol = GetComponent<Collider2D>();

        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        player.OnCrouchStarted += OnPlayerCrouch;
        player.OnCrouchReleased += OnPlayerCrouchRelease;
    }

    private void Update()
    {
        if (!playerIsCrouching || playerCol == null || ignoringPlayerCol)
            return;

        StartCoroutine(nameof(PlayerPassTrough));
    }

    private IEnumerator PlayerPassTrough()
    {
        ignoringPlayerCol = true;

        Physics2D.IgnoreCollision(playerCol, platformCol);

        yield return new WaitForSeconds(0.5f);

        Physics2D.IgnoreCollision(playerCol, platformCol, false);

        playerCol = null;
        ignoringPlayerCol = false;
    }

    private void OnPlayerCrouch()
    {
        playerIsCrouching = true;
    }

    private void OnPlayerCrouchRelease()
    {
        playerIsCrouching = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.rigidbody.gameObject.CompareTag("Player"))
        {
            return;
        }

        playerCol = collision.collider;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.rigidbody.gameObject.CompareTag("Player"))
        {
            return;
        }

        playerCol = null;
    }
}
