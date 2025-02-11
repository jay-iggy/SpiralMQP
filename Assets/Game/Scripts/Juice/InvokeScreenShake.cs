using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InvokeScreenShake : MonoBehaviour {
    [SerializeField] private float length = .5f;
    [SerializeField] private float power = .5f;
        
    public void Shake() {
        ScreenShake.instance.StartShake(length, power);
    }
}
