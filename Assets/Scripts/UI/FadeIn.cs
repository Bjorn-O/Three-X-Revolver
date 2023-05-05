using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        LevelLoader.instance.OnLoadingLevel += () => anim.enabled = true;
        LevelLoader.instance.OnLevelLoaded += GiveLevelLoadDelay;

        anim.enabled = false;
    }

    private void GiveLevelLoadDelay()
    {
        AnimatorStateInfo animState = anim.GetCurrentAnimatorStateInfo(0);

        float remainingTime = animState.length - animState.normalizedTime * animState.length;

        Debug.Log(remainingTime);

        LevelLoader.instance.DelayLoad = remainingTime;
    }

    public void FreezeGame()
    {
        LevelLoader.instance.SetTimeScale(0);
    }
}
