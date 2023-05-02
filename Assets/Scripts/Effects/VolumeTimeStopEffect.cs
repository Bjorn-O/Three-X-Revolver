using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeTimeStopEffect : MonoBehaviour
{
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        TimeManager.instance.OnTimeStop += () => anim.SetTrigger("StopTime");
        TimeManager.instance.OnTimeResumeTransition += () => anim.SetTrigger("ResumeTime");
    }
}
