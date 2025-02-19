using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
    [SerializeField] private bool isRotating = true;
    [SerializeField] private float rotationSpeed = 100f;
    private void Update() {
        if (!isRotating) {
            return;
        }
        transform.RotateAround(transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
