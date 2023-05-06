using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixAudioLevels : MonoBehaviour
{
    public AudioMixer masterMixer;

    public void SetMasterLevel(float masterLvl)
    {
        masterMixer.SetFloat ("MasterVol", masterLvl);

    }
 
    public void SetSfxLevel(float sfxLvl) {
     
        masterMixer.SetFloat ("SFXVol", sfxLvl);
     
    }
 
    public void SetMusicLevel(float musicLvl) {
 
        masterMixer.SetFloat ("MusicVol", musicLvl);
 
    }
}
