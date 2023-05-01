using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTroughPlatformScriptAdder : MonoBehaviour
{
    void Awake()
    {
        PlatformEffector2D[] allPlatformEffectors = FindObjectsOfType<PlatformEffector2D>();

        foreach (PlatformEffector2D effector in allPlatformEffectors)
        {
            effector.gameObject.AddComponent<JumpTroughPlatform>();
        }
    }
}
