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

    //TODO is only for debug
    private void Start()
    {
        Invoke(nameof(StopTime), timeToStopTime);
    }

    public void StopTime()
    {
        timeScale = timeStopScale;

        OnTimeStop?.Invoke();
    }

    public void ResumeTime()
    {
        OnTimeResumeTransition?.Invoke();
        Time.timeScale = 0;

        StartCoroutine(nameof(SetTimeToNormal));
    }

    private IEnumerator SetTimeToNormal()
    {
        yield return new WaitForSecondsRealtime(resumeTimeTransitionTime);
        Time.timeScale = 1;
        timeScale = 1;

        OnTimeResume?.Invoke();
    }
}
