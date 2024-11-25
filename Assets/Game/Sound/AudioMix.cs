using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMix : MonoBehaviour
{
    public AudioSource m_MyAudioSource;

    bool m_Play = false;

    void Start()
    {
        //Fetch the AudioSource from the GameObject
        m_MyAudioSource = GetComponent<AudioSource>();
        //Ensure the toggle is set to true for the music to play at start-up
        m_Play = true;
    }

    public void playClip()
    {
        if (m_Play)
        {
            m_MyAudioSource.Play();

            m_Play = true;

            print("works");
        }
        else
        {
            m_MyAudioSource.Stop();

            m_Play = false;
        }
    }
}
