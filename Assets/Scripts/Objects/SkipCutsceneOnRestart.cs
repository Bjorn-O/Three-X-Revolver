using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SkipCutsceneOnRestart : MonoBehaviour
{
    [SerializeField] private float timelineSkipTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        PlayableDirector director = GetComponent<PlayableDirector>();
        director.initialTime = GameManager.instance.PlayerRestarted ? timelineSkipTime : director.initialTime;


        if (director.playableAsset != null)
        {
            director.Play();
        }
        else
        {
            LevelManager.Instance.StartLevel();
        }
    }
}
