using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private Enemy[] targets;
    [SerializeField] private int targetsKilled;
    public int targetRemaining;

    public delegate void ObjectiveComplete();
    public ObjectiveComplete OnObjectiveComplete;

    public static ObjectiveManager Instance;

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
    }

    // Start is called before the first frame update
    private void Start()
    {
        foreach (Enemy enemy in targets)
        {
            enemy.onDeath += TargetKilled;
        }
    }

    private void TargetKilled()
    {
        targetsKilled++;
        targetRemaining = targets.Length - targetsKilled;
        if (targetsKilled >= targets.Length)
        {
            OnObjectiveComplete?.Invoke();
        }
    }
}
