using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRestart : MonoBehaviour
{
    private void OnRestart()
    {
        GameManager.instance.PlayerRestarted = true;
        LevelLoader.instance.ReloadCurrentScene();
    }
}
