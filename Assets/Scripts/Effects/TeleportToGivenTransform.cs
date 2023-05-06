using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToGivenTransform : MonoBehaviour
{
    public Transform teleportPoint;
    
    public void TeleportToTranform()
    {
        transform.position = teleportPoint.position;
    }
}
