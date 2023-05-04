using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeFramer : MonoBehaviour
{
    public static FreezeFramer instance;
    [SerializeField] private int defaultFreezeFrames = 3;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void FreezeFrame()
    {
        FreezeFrame(defaultFreezeFrames);
    }

    public void FreezeFrame(int freezeFrames)
    {
        StartCoroutine(StartFreezeFraming(freezeFrames));
    }

    private IEnumerator StartFreezeFraming(int framesToFreeze)
    {
        Time.timeScale = 0;

        for (int i = 0; i < framesToFreeze; i++)
        {
            yield return null;
        }

        Time.timeScale = 1;
    }
}
