using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    [SerializeField] private float timeScale = 1;
    [SerializeField] private float timeStopScale = 0.1f;
    [SerializeField] private float timeToStopTime = 1f;
    [SerializeField] private float resumeTimeTransitionTime = 1f;
    public float TimeScale { get { return timeScale; } }

    public delegate void TimeStop();
    public TimeStop OnTimeStop;

    public delegate void TimeResume();
    public TimeResume OnTimeResume;

    public delegate void TimeResumeTransition();
    public TimeResumeTransition OnTimeResumeTransition;

    private void Awake()
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
    
    //TODO is only for debug
    private void Start()
    {
        Invoke(nameof(StopTime), timeToStopTime);
    }

    public void StopTime()
    {
        SoundManager.instance.PlayMusic("TimeStop");
        SoundManager.instance.PlaySoundEffect("Time", "TimeStop");

        timeScale = timeStopScale;

        OnTimeStop?.Invoke();
    }

    public void ResumeTime()
    {
        OnTimeResumeTransition?.Invoke();
        Time.timeScale = 0;
        SoundManager.instance.StopSoundEffect();
        SoundManager.instance.PlaySoundEffect("Time", "TimeStop");

        StartCoroutine(nameof(SetTimeToNormal));
    }

    private IEnumerator SetTimeToNormal()
    {
        yield return new WaitForSecondsRealtime(resumeTimeTransitionTime);
        Time.timeScale = 1;
        timeScale = 1;
        SoundManager.instance.StopSoundEffect();
        SoundManager.instance.PlayMusic("Main");

        OnTimeResume?.Invoke();
    }
}
