using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Sound")]
public class Sound : ScriptableObject{
    public List<AudioClip> clips = new List<AudioClip>();
    public float baseVolume = 1;

    public void PlaySound(Vector3 position, float volumeScale = 1) {
        AudioSource.PlayClipAtPoint(GetAudioClip(), position, baseVolume * volumeScale);
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
}
