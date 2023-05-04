using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.iOS;

public class LevelManager : MonoBehaviour
{
    private static readonly int Open = Animator.StringToHash("Open");
    
    public static LevelManager Instance;

    public UnityEvent onLevelStarted;
    public UnityEvent onLevelStopped;
    public UnityEvent onLevelLost;
    public UnityEvent onLevelWon;

    [SerializeField] private float levelTime;
    [SerializeField] private GameObject endGate;
    [SerializeField] private TextMeshProUGUI timerUI;

    private float _timeRemaining;
    private bool _levelStarted;
    private List<GameObject> _activeGameplayObjects;
    private Animator _gateAnimator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        _gateAnimator = endGate.GetComponent<Animator>();
        _levelStarted = true;
        _timeRemaining = levelTime;
    }

    private void Update()
    {
        if (!_levelStarted) return;

        if (_timeRemaining <= 0 )
        {
            timerUI.text = TimeFormatter(0);
            if (_activeGameplayObjects.Count <= 0)
            {
                StopLevel();
            }
        }
        else
        {
            _timeRemaining -= Time.deltaTime;
        }

        timerUI.text = TimeFormatter(_timeRemaining);
    }

    public void StartLevel()
    {
        if (_levelStarted) return;
        onLevelStarted.Invoke();
        
        
        _levelStarted = true;
    }

    private void StopLevel()
    {
        onLevelStopped.Invoke();
        if (ObjectiveManager.Instance.targetRemaining <= 0)
        {
            _gateAnimator.SetTrigger(Open);
            _levelStarted = false;
            onLevelWon.Invoke();
            return;
        }
        onLevelLost.Invoke();
        // Show Reset Button
    }

    public void AddActiveObject(GameObject activeObject)
    {
        _activeGameplayObjects.Add(activeObject);
    }

    public void RemoveActiveObject(GameObject inactiveObject)
    {
        _activeGameplayObjects.Remove(inactiveObject);
    }

    private string TimeFormatter(float time)
    {
        var intTime = (int)time;
        var minutes = intTime / 60;
        var seconds = intTime % 60;
        var fraction = time * 1000;
        fraction -= 0.01f;
        fraction = fraction % 1000;
        return $"{minutes:00} : {seconds:00} : {fraction / 10:00}";
    }
}
