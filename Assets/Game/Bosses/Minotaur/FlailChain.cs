
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
        lineRenderer.positionCount = 2;
    }

    private void Update() {
        lineRenderer.SetPosition(1, handle.position);
        lineRenderer.SetPosition(0, head.position);
    }
}
