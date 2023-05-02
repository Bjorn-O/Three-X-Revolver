using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();

        TimeManager.instance.OnTimeStop += () => anim.speed = TimeManager.instance.TimeScale;
        TimeManager.instance.OnTimeResume += () => anim.speed = 1;
    }

    public void Kill()
    {
        anim.SetTrigger("Die");
    }
}
