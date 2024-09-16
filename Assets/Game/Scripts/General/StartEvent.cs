using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartEvent : MonoBehaviour {
    public UnityEvent onStart;
    private void Start() {
        onStart.Invoke();
    }
}
