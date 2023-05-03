using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource, sfxSource, loopingSfxSource;
    [SerializeField] private string startMusicName;
    [SerializeField] private float timeStopPitchMultiplier = 10;

    public static SoundManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public SoundsArray[] soundsArrays;
    private Dictionary<string, SoundsArray> soundsArrayDictionary = new Dictionary<string, SoundsArray>();

    private void Start()
    {
        foreach (SoundsArray soundsArray in soundsArrays)
        {
            soundsArrayDictionary.Add(soundsArray.name, soundsArray);

            foreach (Sound sound in soundsArray.sounds)
            {
                soundsArray.soundsDictionary.Add(sound.name, sound);
            }
        }

        if (startMusicName != "")
        {
            PlayMusic(startMusicName);
        }
    }

    public void PlayMusic(string songName)
    {
        Play("Music", songName, musicSource);
    }

    public void PlaySoundEffect(string categoryName, string soundEffectName)
    {
        Play(categoryName, soundEffectName, sfxSource);
    }

    private Sound GetSound(string categoryName, string audioName)
    {
        SoundsArray soundsArray;

        if (!soundsArrayDictionary.TryGetValue(categoryName, out soundsArray))
        {
            Debug.LogWarning(categoryName + " category does not exist in SoundManager");
            return null;
        }

        Sound sound;

        if (!soundsArray.soundsDictionary.TryGetValue(audioName, out sound))
        {
            Debug.LogWarning(audioName + " does not exist in " + categoryName + " category");
            return null;
        }

        return sound;
    }

    private void Play(string categoryName, string soundName, AudioSource audioSource)
    {
        Sound sound = GetSound(categoryName, soundName);

        audioSource.volume = sound.volume;

        if (audioSource == sfxSource)
        {
            if (sound.loop)
            {
                audioSource = loopingSfxSource;
                audioSource.Stop();
                audioSource.clip = sound.audioClip;
                audioSource.Play();
                return;
            }

            audioSource.pitch = TimeManager.instance.TimeScale < 1 ? TimeManager.instance.TimeScale * timeStopPitchMultiplier : 1;
            audioSource.PlayOneShot(sound.audioClip);
        }
        else
        {
            audioSource.loop = sound.loop;
            audioSource.Stop();
            audioSource.clip = sound.audioClip;
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void StopSoundEffect()
    {
        loopingSfxSource.Stop();
    }
}
