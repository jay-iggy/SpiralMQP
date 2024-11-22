using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementComponent : MonoBehaviour {
    public Vector3 moveVelocity;
    private Vector3 externalVelocity;
    private Vector3 personalVelocity;
    private float verticalVelocity;
    public float gravity = -9.8f;
    public float terminalVelocity = -20;
    [SerializeField] private float externalVelocityDamping = 5;
    [SerializeField] private float personalVelocityDamping = 5;
    
    private Rigidbody _rb;
    
    private void Awake() {
        _rb = GetComponent<Rigidbody>();
    }

    void Update() {
        Vector3 velocity = moveVelocity + externalVelocity + personalVelocity;
        velocity.y = verticalVelocity;
        _rb.velocity = velocity;
        externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, externalVelocityDamping * Time.deltaTime);
        personalVelocity = Vector3.Lerp(personalVelocity, Vector3.zero, personalVelocityDamping * Time.deltaTime);
        if(verticalVelocity > terminalVelocity) {
            float deltaGravity = gravity * Time.deltaTime;
            Mathf.Clamp(verticalVelocity + deltaGravity, terminalVelocity, Mathf.Infinity);
        }
        // kill vertical velocity if we hit the ground
        if (_rb.velocity.y > -0.01 && verticalVelocity < 0) {
            verticalVelocity = 0;
        }
    }
    
    public void AddExternalVelocity(Vector3 velocity) {
        externalVelocity += velocity;
    }
    public void AddPersonalVelocity(Vector3 velocity) {
        personalVelocity += velocity;
    }
    public void AddVerticalVelocity(float velocity) {
        verticalVelocity += velocity;
    }
}
