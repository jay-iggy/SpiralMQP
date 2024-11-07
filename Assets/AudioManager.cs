using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] musicSound, sfxSound, slashSound;
    public AudioSource musiceSource, sfxSource, slashSource;

    /*
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    */
    private void Start()
    {
        PlayMusic("dungeon_composition");
        PlayAttack();
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

    public void PlaySFX(string name)
    {

        AudioClip s = null;

        foreach (AudioClip clip in musicSound)
        {
            if (clip.name == name)
            {

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
    public void PlayAttack()
    {
        
        int randomAttack = Random.Range(0, slashSound.Length);

            slashSource.clip = slashSound[randomAttack];
            slashSource.Play();
    }
}
 