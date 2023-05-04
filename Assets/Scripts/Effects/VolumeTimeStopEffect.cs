using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeTimeStopEffect : MonoBehaviour
{
    private Animator _anim;

    // Start is called before the first frame update
    private void Start()
    {
        _anim = GetComponent<Animator>();

        TimeManager.instance.OnTimeStop += () => _anim.SetTrigger("StopTime");
        TimeManager.instance.OnTimeResumeTransition += () => _anim.SetTrigger("ResumeTime");
    }
}
