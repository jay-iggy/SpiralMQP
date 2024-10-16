using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPause : MonoBehaviour
{
    public bool waiting = false;
    public static HitPause instance;
    
    
    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    public void Pause(float duration)
    {
        if (waiting)
            return;

        Time.timeScale = 0.0f;
        StartCoroutine(Wait(duration));
    }

    IEnumerator Wait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
        waiting = false;
    }

    private void OnDestroy() {
        if(Time.timeScale != 1.0f) {
            Time.timeScale = 1.0f;
        }
    }
}
