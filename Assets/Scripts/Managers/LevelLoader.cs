using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;

    public delegate void LoadingLevel();
    public LoadingLevel OnLoadingLevel;

    public delegate void LevelLoaded();
    public LevelLoaded OnLevelLoaded;

    public float DelayLoad { get; set; }

    private bool _alreadyLoading = false;

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

    public void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadScene(string sceneName)
    {
        if (_alreadyLoading)
            return;

        OnLoadingLevel?.Invoke();
        _alreadyLoading = true;

        StartCoroutine(LoadAyncScene(sceneName));
    }

    private IEnumerator LoadAyncScene(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            //scene has loaded as much as possible,
            // the last 10% can't be multi-threaded
            if (asyncOperation.progress >= 0.9f)
            {
                OnLevelLoaded?.Invoke();

                yield return null;
                Debug.Log(DelayLoad);
                yield return new WaitForSecondsRealtime(DelayLoad);
                OnLoadingLevel = null;
                OnLevelLoaded = null;
                if (SoundManager.instance != null)
                    SoundManager.instance.StopSoundEffect();
                
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        SetTimeScale(1);

        _alreadyLoading = false;
    }

    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }
}
