using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] musicSound, sfxSound;
    public AudioSource musiceSource, sfxSource;

    private void Start()
    {
        PlayMusic("dungeon_composition");
    }

    public void PlayMusic(string name) {

        AudioClip s = null;

        foreach (AudioClip clip in musicSound)
        {
            if (clip.name == name) {

                s = clip;
            }
        }

        

        if (s == null)
        {
            //Debug.log("Sound Not Found");
        }

        else
        {
            musiceSource.clip = s;
            musiceSource.Play();
        }
    }
}
 