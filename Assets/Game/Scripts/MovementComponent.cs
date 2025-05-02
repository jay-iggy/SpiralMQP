using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConstantForce))]
public class MovementComponent : MonoBehaviour {
    public Vector3 moveVelocity;
    private Vector3 externalVelocity;
    private Vector3 personalVelocity;
    [SerializeField] private float externalVelocityDamping = 5;
    [SerializeField] private float personalVelocityDamping = 5;
    [SerializeField] private bool lockExternalVelocity = false;
    
    private Rigidbody _rb;
    private ConstantForce _gravity;
    
    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _gravity = GetComponent<ConstantForce>();
    }

    private void Reset() {
        GetComponent<ConstantForce>().force = new Vector3(0, -9.81f, 0);
    }

    void Update() {
        Vector3 velocity = moveVelocity + externalVelocity + personalVelocity;
        velocity.y = _rb.velocity.y;
        _rb.velocity = velocity;
        externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, externalVelocityDamping * Time.deltaTime);
        personalVelocity = Vector3.Lerp(personalVelocity, Vector3.zero, personalVelocityDamping * Time.deltaTime);
    }
    
    public void AddExternalVelocity(Vector3 velocity) {
        if(lockExternalVelocity) {
            return;
        }
        externalVelocity += velocity;
    }
    public void AddPersonalVelocity(Vector3 velocity) {
        personalVelocity += velocity;
    }
    public void AddVerticalVelocity(float velocity) {
        _rb.velocity += Vector3.up * velocity;
    }
    public void SetGravityEnabled(bool isEnabled) {
        _gravity.enabled = isEnabled;
    }
    public void SetGravityValue(float value) {
        _gravity.force = new Vector3(0, value, 0);
    }
}
