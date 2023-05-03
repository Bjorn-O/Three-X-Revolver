using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip audioClip;
    [Range(0, 1)] public float volume = 1;
    public bool loop;
}

[System.Serializable]
public class SoundsArray
{
    public string name;
    public Sound[] sounds;
    public Dictionary<string, Sound> soundsDictionary = new Dictionary<string, Sound>();
}
