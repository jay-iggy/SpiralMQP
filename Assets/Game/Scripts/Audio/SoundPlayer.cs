using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {
    public void PlaySound(Sound sound) {
        if(sound == null) {
            Debug.LogError("Sound is null");
            return;
        }
        sound.PlaySound();
    }
}
