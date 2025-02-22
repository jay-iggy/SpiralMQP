
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FlailChain : MonoBehaviour {
    [SerializeField] private Transform handle;
    [SerializeField] private Transform head;
    
    private LineRenderer lineRenderer;

    private void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update() {
        lineRenderer.SetPosition(0, handle.position);
        lineRenderer.SetPosition(1, head.position);
    }
}
