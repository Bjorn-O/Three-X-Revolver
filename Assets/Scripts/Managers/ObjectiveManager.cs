using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private Enemy[] targets;
    [SerializeField] private int targetsKilled;

    public delegate void ObjectiveComplete();
    public ObjectiveComplete OnObjectiveComplete;

    public static ObjectiveManager instance;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Enemy enemy in targets)
        {
            enemy.onDeath += TargetKilled;
        }
    }

    private void TargetKilled()
    {
        targetsKilled++;

        if (targetsKilled >= targets.Length)
        {
            OnObjectiveComplete?.Invoke();
        }
    }
}
