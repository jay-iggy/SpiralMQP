using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World_Audio : MonoBehaviour
{

    public AudioSource Background;

    // Start is called before the first frame update
    void Start()
    {
        PlayMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayMusic()
    {
        if (!Background.isPlaying)
        {
            Background.Play();
        }
    }

    public void StopMusic()
    {
        if (Background.isPlaying)
        {
            Background.Stop();
        }
    }
}
