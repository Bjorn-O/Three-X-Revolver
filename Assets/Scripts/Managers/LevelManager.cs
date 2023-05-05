using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    [SerializeField] private GameObject startGate;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private TextMeshProUGUI timerUI;

    private const float GateTime = 1f;
    private float _timeRemaining;
    private bool _shotsDepleted;
    private bool _levelStarted;

    private List<GameObject> _activeGameplayObjects = new();
    private Animator _endGateAnimator;
    private Animator _startGateAnimator;

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
        
        playerObject = GameObject.FindWithTag("Player");
        PlayerSetUp();
        
        _endGateAnimator = endGate.GetComponent<Animator>();
        _startGateAnimator = startGate.GetComponent<Animator>();
        
        _shotsDepleted = false;
        _levelStarted = false;
        
        _timeRemaining = levelTime;
    }

    private void Update()
    {
        if (!_levelStarted) return;

        if (_timeRemaining <= 0 || _shotsDepleted)
        {
            StopLevel();
            timerUI.text = TimeFormatter(_timeRemaining <= 0 ? 0 : _timeRemaining);
            return;
        }
        _timeRemaining -= Time.deltaTime;
        timerUI.text = TimeFormatter(_timeRemaining);
    }

    public void StartLevel()
    {
        StartCoroutine(StartLevelRoutine());
    }

    private void ShotsDepleted()
    {
        _shotsDepleted = true;
    }

    private IEnumerator StartLevelRoutine()
    {
        if (_levelStarted) yield break;

        _startGateAnimator.SetTrigger(Open);
        yield return new WaitForSeconds(GateTime);
        
        onLevelStarted.Invoke();
        _levelStarted = true;
    }

    private void StopLevel()
    {
        onLevelStopped?.Invoke();
        _levelStarted = false;
        if (_activeGameplayObjects.Count == 0) CalculateResults();
    }

    private void CalculateResults()
    {
        print(ObjectiveManager.Instance.targetRemaining);
        if (ObjectiveManager.Instance.targetRemaining <= 0)
        {
            _endGateAnimator.SetTrigger(Open);
            _levelStarted = false;
            onLevelWon.Invoke();
            return;
        }
        print(onLevelLost);
        onLevelLost.Invoke();
        // Show Reset Button
    }
    
    private void PlayerSetUp()
    {
        if (playerObject.TryGetComponent<PlayerShoot>(out var shoot))
        {
            shoot.onOutOfAmmo.AddListener(ShotsDepleted);
        }
        else
        {
            print("Can't find shooting script");
        }
    }

    public void AddActiveObject(GameObject activeObject)
    {
        _activeGameplayObjects.Add(activeObject);
    }

    public void RemoveActiveObject(GameObject inactiveObject)
    {
        _activeGameplayObjects.Remove(inactiveObject);
        if (_activeGameplayObjects.Count <= 0 && (_shotsDepleted || _timeRemaining <= 0))
        {
            CalculateResults();
        }
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
