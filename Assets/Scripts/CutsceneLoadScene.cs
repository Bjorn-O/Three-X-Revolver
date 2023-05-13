using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneLoadScene : MonoBehaviour
{
    public void LoadMainMenu()
    {
        GameManager.instance.completionTime = 0;
        LevelLoader.instance.LoadScene("Main Menu");
    }

    public void StopMusic()
    {
        SoundManager.instance.StopMusic();
    }
}
