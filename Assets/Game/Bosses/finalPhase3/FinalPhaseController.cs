using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class FinalPhaseController : MonoBehaviour
{
    public static FinalPhaseController instance;
    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    [FormerlySerializedAs("StartFinalFight")] public UnityEvent onStartFinalFight = new();
    public void StartFinal() {
        onStartFinalFight.Invoke();
    }
}
