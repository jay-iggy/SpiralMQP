using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Sound")]
public class Sound : ScriptableObject{
    public List<AudioClip> clips = new List<AudioClip>();
    public float baseVolume = 1;
    public AudioMixerGroup mixerGroup;

    private void Reset() {
        mixerGroup = Resources.Load<AudioMixer>("SFX").FindMatchingGroups("Master")[0];
    }

    public void PlaySound(Vector3 position, float volumeScale = 1) {
        PlayClipAtPoint(GetAudioClip(), position, baseVolume * volumeScale, mixerGroup);
    }

    public void PlaySound(float volumeScale = 1) {
        PlaySound(Vector3.zero, volumeScale);
    }
    
    private AudioClip GetAudioClip() {
        if(clips.Count == 0) {
            Debug.LogError("No audio clips in sound: " + name);
            return null;
        }
        return clips[Random.Range(0, clips.Count)];
    }
    
    // Taken from AudioSource.PlayClipAtPoint, modified for my own purposes
    private static void PlayClipAtPoint(AudioClip clip, Vector3 position, [UnityEngine.Internal.DefaultValue("1.0F")] float volume, AudioMixerGroup audioMixerGroup = null)
    {
        GameObject gameObject = new GameObject("One shot audio");
        DontDestroyOnLoad(gameObject);
        gameObject.transform.position = position;
        AudioSource audioSource = (AudioSource) gameObject.AddComponent(typeof (AudioSource));
        audioSource.clip = clip;
        audioSource.spatialBlend = 1f;
        audioSource.volume = volume;
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.Play();
        Destroy(gameObject, clip.length * (Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
    }
}
