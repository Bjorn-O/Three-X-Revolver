using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTriggerAnimOnWin : MonoBehaviour
{
    [SerializeField] private string triggerName;

    // Start is called before the first frame update
    void Start()
    {
        Animator anim = GetComponent<Animator>();
        ObjectiveManager.instance.OnObjectiveComplete += () => anim.SetTrigger(triggerName);
    }
}
